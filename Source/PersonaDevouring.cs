using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OMWPersonaDevouringPawn
{
    [DefOf]
    public static class OMWDefOf
    {
        public static TraitDef OMW_PersonaDevourer;
        public static HediffDef OMW_DevouredPersonaTrait;
        public static ThoughtDef OMW_DevouredPersonaMemory;
        public static JobDef OMW_DevourPersona;

        static OMWDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OMWDefOf));
        }
    }

    [StaticConstructorOnStartup]
    public static class OMWStartup
    {
        static OMWStartup()
        {
            Harmony harmony = new Harmony("oldmanwhistler.PersonaDevouringPawn");
            harmony.PatchAll();
            Patch_MorePersonaTraits_FemaleThought.TryPatch(harmony);
        }
    }

    public sealed class DevourPlan
    {
        public readonly List<WeaponTraitDef> supported = new List<WeaponTraitDef>();
        public readonly List<WeaponTraitDef> ignored = new List<WeaponTraitDef>();
        public string failureReason;
    }

    public static class PersonaDevouring
    {
        private const int QuadrumTicks = 15 * GenDate.TicksPerDay;
        private const string DevouredLabelKey = "OMW_DevouredPersonaLabel";
        private const string DevourKey = "OMW_Devour";
        private const string UnsupportedKey = "OMW_DevourUnsupported";
        private const string AlreadyOwnedKey = "OMW_DevourAlreadyOwned";
        private const string NoAbilitiesKey = "OMW_DevourNoAbilities";
        private const string NotEligibleKey = "OMW_DevourNotEligible";
        private const string LetterBodyKey = "OMW_DevourLetterBody";
        private const string FailureMoteKey = "OMW_DevourFailureMote";

        public static bool IsDevourer(Pawn pawn)
        {
            return pawn != null && pawn.story?.traits?.HasTrait(OMWDefOf.OMW_PersonaDevourer) == true;
        }

        public static bool IsEligiblePawn(Pawn pawn)
        {
            return pawn != null && pawn.IsColonistPlayerControlled && pawn.RaceProps.Humanlike && IsDevourer(pawn);
        }

        public static bool IsSupportedPersonaWeapon(Thing thing)
        {
            return thing?.TryGetComp<CompBladelinkWeapon>() != null;
        }

        public static string GetWeaponName(Thing weapon)
        {
            if (weapon == null)
            {
                return string.Empty;
            }
            if (weapon.StyleSourcePrecept != null)
            {
                return weapon.StyleSourcePrecept.Label;
            }
            CompGeneratedNames compGeneratedNames = weapon.TryGetComp<CompGeneratedNames>();
            if (compGeneratedNames != null)
            {
                return compGeneratedNames.Name;
            }
            return weapon.LabelNoCount;
        }

        public static string GetDevouredBondedWeaponName(Pawn pawn, ThoughtDef thoughtDef)
        {
            Hediff_DevouredPersonaTrait hediff = pawn?.health?.hediffSet?.hediffs
                .OfType<Hediff_DevouredPersonaTrait>()
                .FirstOrDefault(x => x.HasBondedThought(thoughtDef));
            if (hediff == null)
            {
                return null;
            }
            return hediff.BondedWeaponName.NullOrEmpty()
                ? "OMW_DevouredPersonaWeaponFallback".Translate().ToString()
                : hediff.BondedWeaponName;
        }

        public static bool HasDevouredBondedThought(Pawn pawn, ThoughtDef thoughtDef)
        {
            return pawn?.health?.hediffSet?.hediffs
                .OfType<Hediff_DevouredPersonaTrait>()
                .Any(hediff => hediff.HasBondedThought(thoughtDef)) == true;
        }

        public static bool IsOwned(Pawn pawn, WeaponTraitDef trait)
        {
            return trait != null && IsOwned(pawn, trait.defName);
        }

        public static bool IsOwned(Pawn pawn, string traitDefName)
        {
            if (pawn?.health?.hediffSet == null || traitDefName.NullOrEmpty())
            {
                return false;
            }

            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_DevouredPersonaTrait devoured && devoured.TraitDefName == traitDefName)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInventoryTarget(Pawn pawn, Thing thing)
        {
            return pawn?.inventory?.innerContainer != null && thing != null && pawn.inventory.innerContainer.Contains(thing);
        }

        public static bool IsMapTarget(Thing thing)
        {
            return thing != null && thing.Spawned && thing.Map != null;
        }

        public static bool IsEquippedTarget(Pawn pawn, Thing thing)
        {
            return pawn?.equipment != null && pawn.equipment.AllEquipmentListForReading.Contains(thing);
        }

        public static bool IsExcludedApparel(Pawn pawn, Thing thing)
        {
            return pawn?.apparel != null && pawn.apparel.WornApparel.Contains(thing);
        }

        public static DevourPlan Evaluate(Pawn pawn, Thing weapon)
        {
            var plan = new DevourPlan();
            if (!IsEligiblePawn(pawn))
            {
                plan.failureReason = "OMW_DevourNotEligible".Translate().ToString();
                return plan;
            }
            if (!IsSupportedPersonaWeapon(weapon))
            {
                plan.failureReason = "OMW_DevourNotPersonaWeapon".Translate().ToString();
                return plan;
            }
            if (IsExcludedApparel(pawn, weapon))
            {
                plan.failureReason = "OMW_DevourApparelExcluded".Translate().ToString();
                return plan;
            }
            if (!IsInventoryTarget(pawn, weapon) && !IsEquippedTarget(pawn, weapon) && !IsMapTarget(weapon))
            {
                plan.failureReason = "OMW_DevourInvalidLocation".Translate().ToString();
                return plan;
            }

            CompBladelinkWeapon comp = weapon.TryGetComp<CompBladelinkWeapon>();
            if (comp == null || comp.TraitsListForReading.NullOrEmpty())
            {
                plan.failureReason = "OMW_DevourNoAbilities".Translate().ToString();
                return plan;
            }

            bool hasOwnedTrait = false;
            bool hasNegativeOrNeutralTrait = false;
            bool hasOtherIgnoredTrait = false;
            foreach (WeaponTraitDef trait in comp.TraitsListForReading)
            {
                if (IsOwned(pawn, trait))
                {
                    hasOwnedTrait = true;
                    plan.ignored.Add(trait);
                    continue;
                }
                if (PersonaDevouringTraitRules.IsBlacklisted(trait))
                {
                    hasNegativeOrNeutralTrait |= trait.marketValueOffset <= 0f || trait.neverBond;
                    hasOtherIgnoredTrait |= trait.marketValueOffset > 0f && !trait.neverBond;
                    plan.ignored.Add(trait);
                    continue;
                }
                if (PersonaDevouringTraitRules.IsReplacedByOwnedTrait(pawn, trait))
                {
                    hasOtherIgnoredTrait = true;
                    plan.ignored.Add(trait);
                    continue;
                }

                TraitDisposition disposition = TraitAdapters.Classify(trait, out string reason);
                if (disposition == TraitDisposition.Ignored)
                {
                    hasNegativeOrNeutralTrait = true;
                    plan.ignored.Add(trait);
                    continue;
                }
                if (disposition == TraitDisposition.Unsupported)
                {
                    plan.failureReason = "OMW_DevourUnsupported".Translate(trait.LabelCap, reason).ToString();
                    return plan;
                }
                plan.supported.Add(trait);
            }

            if (plan.supported.Count == 0)
            {
                if (hasNegativeOrNeutralTrait)
                {
                    plan.failureReason = "OMW_DevourNegativeOrNeutral".Translate().ToString();
                }
                else if (hasOwnedTrait && !hasOtherIgnoredTrait)
                {
                    plan.failureReason = "OMW_DevourAlreadyOwned".Translate().ToString();
                }
                else if (hasOtherIgnoredTrait)
                {
                    plan.failureReason = "OMW_DevourNoTransferableTraits".Translate().ToString();
                }
                else
                {
                    plan.failureReason = "OMW_DevourNoAbilities".Translate().ToString();
                }
            }
            return plan;
        }

        public static FloatMenuOption MakeOption(Pawn pawn, Thing weapon)
        {
            DevourPlan plan = Evaluate(pawn, weapon);
            if (plan.failureReason.NullOrEmpty())
            {
                return new FloatMenuOption("OMW_Devour".Translate().ToString(),
                    () => StartDevourJob(pawn, weapon),
                    MenuOptionPriority.Default,
                    null,
                    weapon);
            }

            var disabled = new FloatMenuOption("OMW_Devour".Translate().ToString(), null, MenuOptionPriority.DisabledOption);
            disabled.tooltip = plan.failureReason;
            return disabled;
        }

        public static void StartDevourJob(Pawn pawn, Thing weapon)
        {
            DevourPlan plan = Evaluate(pawn, weapon);
            if (!plan.failureReason.NullOrEmpty())
            {
                Log.Warning($"[Persona Devouring Pawn] Could not start devour job for {pawn?.LabelShort ?? "unknown pawn"} and {weapon?.Label ?? "unknown item"}: {plan.failureReason}");
                ThrowFailureMote(pawn);
                return;
            }

            Job job = JobMaker.MakeJob(OMWDefOf.OMW_DevourPersona, weapon);
            if (pawn?.jobs == null || !pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
            {
                Log.Warning($"[Persona Devouring Pawn] Could not start devour job for {pawn?.LabelShort ?? "unknown pawn"} and {weapon?.Label ?? "unknown item"}.");
                ThrowFailureMote(pawn);
            }
        }

        public static bool CanContinueDevouring(Pawn pawn, Thing weapon)
        {
            return Evaluate(pawn, weapon).failureReason.NullOrEmpty();
        }

        public static bool TryAddDevouredTrait(Pawn pawn, WeaponTraitDef trait, out Hediff_DevouredPersonaTrait addedHediff, List<Hediff> overriddenHediffs = null, string sourceWeaponName = null)
        {
            addedHediff = null;
            if (pawn?.health == null || trait == null || IsOwned(pawn, trait)
                || PersonaDevouringTraitRules.IsBlacklisted(trait)
                || PersonaDevouringTraitRules.IsReplacedByOwnedTrait(pawn, trait))
            {
                return false;
            }

            if (TraitAdapters.Classify(trait, out _) != TraitDisposition.Supported)
            {
                return false;
            }

            addedHediff = HediffMaker.MakeHediff(OMWDefOf.OMW_DevouredPersonaTrait, pawn) as Hediff_DevouredPersonaTrait;
            if (addedHediff == null)
            {
                throw new InvalidOperationException("The devoured persona hediff could not be created.");
            }
            addedHediff.SetTrait(trait.defName, sourceWeaponName);
            pawn.health.AddHediff(addedHediff);

            foreach (Hediff existing in pawn.health.hediffSet.hediffs.ToList())
            {
                if (existing is Hediff_DevouredPersonaTrait devoured
                    && existing != addedHediff
                    && PersonaDevouringTraitRules.TraitsOverriddenBy(trait.defName).Contains(devoured.TraitDefName))
                {
                    pawn.health.RemoveHediff(existing);
                    overriddenHediffs?.Add(existing);
                }
            }
            return true;
        }

        public static int ApplyAllDevourableTraits(Pawn pawn)
        {
            if (pawn?.health == null)
            {
                return 0;
            }

            List<WeaponTraitDef> traits = DefDatabase<WeaponTraitDef>.AllDefsListForReading
                .Where(trait => !IsOwned(pawn, trait)
                    && !PersonaDevouringTraitRules.IsBlacklisted(trait)
                    && !PersonaDevouringTraitRules.IsReplacedByOwnedTrait(pawn, trait)
                    && TraitAdapters.Classify(trait, out _) == TraitDisposition.Supported)
                .OrderBy(_ => Rand.Value)
                .ToList();

            int added = 0;
            foreach (WeaponTraitDef trait in traits)
            {
                try
                {
                    if (TryAddDevouredTrait(pawn, trait, out _))
                    {
                        added++;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[Persona Devouring Pawn] Debug trait application failed for {trait.defName}: {ex}");
                }
            }
            return added;
        }

        public static bool Devour(Pawn pawn, Thing weapon)
        {
            DevourPlan plan = Evaluate(pawn, weapon);
            if (!plan.failureReason.NullOrEmpty())
            {
                Log.Warning($"[Persona Devouring Pawn] Devour failed for {pawn?.LabelShort ?? "unknown pawn"} and {weapon?.Label ?? "unknown item"}: {plan.failureReason}");
                ThrowFailureMote(pawn);
                return false;
            }

            var added = new List<Hediff>();
            var overridden = new List<Hediff>();
            try
            {
                string weaponName = GetWeaponName(weapon);
                foreach (WeaponTraitDef trait in plan.supported)
                {
                    if (!TryAddDevouredTrait(pawn, trait, out Hediff_DevouredPersonaTrait hediff, overridden, weaponName))
                    {
                        throw new InvalidOperationException("The devoured persona trait could not be applied.");
                    }
                    added.Add(hediff);
                }

                string weaponLabel = weapon.LabelCap;
                weapon.Destroy(DestroyMode.Vanish);
                ApplyMoodMemory(pawn);
                ApplyVisual(pawn);
                SendLetter(pawn, weaponLabel, plan.supported);
                return true;
            }
            catch (Exception ex)
            {
                for (int i = added.Count - 1; i >= 0; i--)
                {
                    if (!added[i].ShouldRemove)
                    {
                        pawn.health.RemoveHediff(added[i]);
                    }
                }
                for (int i = overridden.Count - 1; i >= 0; i--)
                {
                    if (!overridden[i].ShouldRemove)
                    {
                        pawn.health.AddHediff(overridden[i]);
                    }
                }
                Log.Error($"[Persona Devouring Pawn] Devour transaction rolled back: {ex}");
                ThrowFailureMote(pawn);
                return false;
            }
        }

        private static void ApplyMoodMemory(Pawn pawn)
        {
            if (pawn?.needs?.mood == null)
            {
                return;
            }

            Thought_Memory existing = pawn.needs.mood.thoughts.memories.Memories
                .OfType<Thought_Memory>()
                .FirstOrDefault(x => x.def == OMWDefOf.OMW_DevouredPersonaMemory);
            if (existing != null)
            {
                existing.moodOffset += 5;
                existing.Renew();
                return;
            }

            Thought_Memory memory = ThoughtMaker.MakeThought(OMWDefOf.OMW_DevouredPersonaMemory) as Thought_Memory;
            if (memory == null)
            {
                return;
            }
            memory.moodOffset = 10;
            memory.durationTicksOverride = QuadrumTicks;
            pawn.needs.mood.thoughts.memories.TryGainMemory(memory);
        }

        private static void SendLetter(Pawn pawn, string weaponLabel, IEnumerable<WeaponTraitDef> traits)
        {
            string body = string.Join("\n", traits.Select(x => "OMW_DevourLetterBody".Translate(pawn.Named("PAWN"), x.LabelCap.Named("ABILITY")).ToString()));
            string title = "OMW_DevourLetterTitle".Translate(pawn.Named("PAWN"), weaponLabel.Named("WEAPON")).ToString();
            Find.LetterStack.ReceiveLetter(title, body, LetterDefOf.PositiveEvent, pawn);
        }

        private static void ApplyVisual(Pawn pawn)
        {
            if (pawn?.Spawned != true)
            {
                return;
            }
            FleckMaker.ThrowLightningGlow(pawn.DrawPos, pawn.Map, 0.8f);
            FleckMaker.ThrowMicroSparks(pawn.DrawPos, pawn.Map);
            FleckMaker.ThrowMicroSparks(pawn.DrawPos, pawn.Map);
        }

        public static void ThrowFailureMote(Pawn pawn)
        {
            if (pawn?.Spawned == true)
            {
                FleckMaker.ThrowDustPuffThick(pawn.DrawPos, pawn.Map, 0.8f, Color.gray);
            }
        }

        public static void NotifyPawnKilled(Pawn attacker, Pawn victim, DamageInfo? dinfo)
        {
            if (attacker == null || victim == null || !IsDevourer(attacker))
            {
                return;
            }
            foreach (Hediff hediff in attacker.health.hediffSet.hediffs.ToList())
            {
                if (hediff is Hediff_DevouredPersonaTrait devoured)
                {
                    devoured.NotifyDevouredKill(victim, dinfo);
                }
            }
        }
    }

    public enum TraitDisposition
    {
        Ignored,
        Supported,
        Unsupported
    }

    public static class TraitAdapters
    {
        public static TraitDisposition Classify(WeaponTraitDef trait, out string reason)
        {
            reason = null;
            if (trait != null && PersonaDevouringTraitRules.IsExplicitlyUnsupported(trait))
            {
                reason = "OMW_DevourExplicitlyUnsupported".Translate().ToString();
                return TraitDisposition.Unsupported;
            }
            if (trait == null || trait.neverBond || trait.marketValueOffset < 0f)
            {
                return TraitDisposition.Ignored;
            }

            bool hasKnownData = trait.equippedStatOffsets != null || trait.equippedHediffs != null || trait.bondedHediffs != null || trait.bondedThought != null || trait.killThought != null;
            string workerName = trait.workerClass?.FullName ?? string.Empty;
            bool knownWorker = workerName == "RimWorld.WeaponTraitWorker_PsyfocusOnKill" || workerName.Contains("WeaponTraitWorker_FoodFilledOnKill") || workerName.Contains("WeaponTraitWorker_ComfortFilledOnKill") || workerName.Contains("WeaponTraitWorker_JoyFilledOnKill") || workerName.Contains("WeaponTraitWorker_BeautyFilledOnKill") || workerName.Contains("WeaponTraitWorker_RestFilledOnKill") || workerName.Contains("WeaponTraitWorker_InOutdoorsFilledOnKill") || workerName.Contains("WeaponTraitWorker_ChemicalFilledOnKill") || workerName.Contains("WeaponTraitWorker_InvisibilityOnKill");

            if (trait.defName == "PsyfocusMeditationBonus" || trait.defName == "OnKill_PsyfocusGain")
            {
                return TraitDisposition.Supported;
            }
            if (hasKnownData || knownWorker || HasRecognizedExtension(trait))
            {
                if (!ValidateSourceHediffs(trait, out reason))
                {
                    return TraitDisposition.Unsupported;
                }
                return TraitDisposition.Supported;
            }

            if (trait.marketValueOffset > 0f)
            {
                reason = trait.workerClass == null
                    ? "OMW_DevourNoAdapter".Translate().ToString()
                    : "OMW_DevourUnsupportedWorker".Translate(trait.workerClass.FullName).ToString();
                return TraitDisposition.Unsupported;
            }
            return TraitDisposition.Ignored;
        }

        private static bool ValidateSourceHediffs(WeaponTraitDef trait, out string reason)
        {
            reason = null;
            IEnumerable<HediffDef> defs = (trait.equippedHediffs ?? Enumerable.Empty<HediffDef>()).Concat(trait.bondedHediffs ?? Enumerable.Empty<HediffDef>());
            foreach (HediffDef def in defs)
            {
                if (def == null || def.stages.NullOrEmpty())
                {
                    reason = "OMW_DevourNoTransferableStage".Translate().ToString();
                    return false;
                }
            }
            return true;
        }

        public static bool HasRecognizedExtension(WeaponTraitDef trait)
        {
            if (trait?.modExtensions.NullOrEmpty() != false)
            {
                return false;
            }
            foreach (DefModExtension extension in trait.modExtensions)
            {
                string name = extension?.GetType().FullName ?? string.Empty;
                if (name == "MorePersonaTraits.Extensions.WeaponTraitOnHitExtension" || name == "MorePersonaTraits.Extensions.WeaponTraitOnKillExtension")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
