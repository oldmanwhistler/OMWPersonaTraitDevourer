using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace OMWPersonaDevouringPawn
{
    public sealed class Hediff_DevouredPersonaTrait : Hediff
    {
        private string traitDefName;
        private string bondedWeaponName;
        private HediffStage cachedStage;
        private string cachedLabel;

        public string TraitDefName => traitDefName;
        public string BondedWeaponName => bondedWeaponName;

        public override int UIGroupKey => ("OMW_DevouredPersonaTrait_" + (traitDefName ?? string.Empty)).GetHashCode();

        public override bool TryMergeWith(Hediff other)
        {
            return false;
        }

        private WeaponTraitDef Trait => traitDefName.NullOrEmpty()
            ? null
            : DefDatabase<WeaponTraitDef>.GetNamedSilentFail(traitDefName);

        public void SetTrait(string defName, string sourceWeaponName = null)
        {
            traitDefName = defName;
            bondedWeaponName = sourceWeaponName;
            cachedStage = null;
            cachedLabel = null;
        }

        public bool HasBondedThought(ThoughtDef thoughtDef)
        {
            return thoughtDef != null && Trait?.bondedThought == thoughtDef;
        }

        public override string LabelBase
        {
            get
            {
                if (cachedLabel.NullOrEmpty())
                {
                    cachedLabel = Trait == null
                        ? "OMW_DevouredPersonaUnknown".Translate().ToString()
                        : "OMW_DevouredPersonaLabel".Translate(Trait.LabelCap).ToString();
                }
                return cachedLabel;
            }
        }

        public override string Description
        {
            get
            {
                return Trait?.description ?? def.description;
            }
        }

        public override HediffStage CurStage
        {
            get
            {
                if (cachedStage == null)
                {
                    cachedStage = BuildStage(Trait);
                }
                return cachedStage;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref traitDefName, "traitDefName");
            Scribe_Values.Look(ref bondedWeaponName, "bondedWeaponName");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                cachedStage = null;
                cachedLabel = null;
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            ApplyPositiveBondedThought();
        }

        public override void Notify_PawnDamagedThing(Thing thing, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            base.Notify_PawnDamagedThing(thing, dinfo, result);
            if (Trait != null && thing != null && !thing.Destroyed)
            {
                TraitEffects.ApplyOnHit(Trait, pawn, thing);
            }
        }

        public void NotifyDevouredKill(Pawn victim, DamageInfo? dinfo)
        {
            if (Trait == null || pawn == null)
            {
                return;
            }
            TraitEffects.ApplyKill(Trait, pawn, victim, dinfo);
        }

        private void ApplyPositiveBondedThought()
        {
            if (Trait?.bondedThought == null || pawn?.needs?.mood == null)
            {
                return;
            }
            ThoughtDef thoughtDef = Trait.bondedThought;
            ThoughtStage stage = thoughtDef.stages?.FirstOrDefault();
            if (stage == null || stage.baseMoodEffect <= 0f)
            {
                return;
            }
            if (thoughtDef.IsMemory)
            {
                Thought_Memory memory = ThoughtMaker.MakeThought(thoughtDef) as Thought_Memory;
                if (memory != null)
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(memory);
                    TraitEffects.LogAdaptorTriggered(Trait, pawn, "bonded memory thought");
                }
            }
            else if (thoughtDef.ThoughtClass == typeof(Thought_Situational))
            {
                pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
                TraitEffects.LogAdaptorTriggered(Trait, pawn, "bonded situational thought");
            }
        }

        private static HediffStage BuildStage(WeaponTraitDef trait)
        {
            var result = new HediffStage
            {
                statOffsets = new List<StatModifier>(),
                statFactors = new List<StatModifier>(),
                capMods = new List<PawnCapacityModifier>()
            };
            if (trait == null)
            {
                return result;
            }

            MergeStatModifiers(result.statOffsets, trait.equippedStatOffsets, multiply: false);
            MergeSourceHediffs(result, trait.equippedHediffs);
            MergeSourceHediffs(result, trait.bondedHediffs);
            return result;
        }

        private static void MergeSourceHediffs(HediffStage destination, IEnumerable<HediffDef> sourceDefs)
        {
            if (sourceDefs == null)
            {
                return;
            }
            foreach (HediffDef sourceDef in sourceDefs)
            {
                if (sourceDef?.stages == null)
                {
                    continue;
                }
                foreach (HediffStage source in sourceDef.stages)
                {
                    MergeStage(destination, source);
                }
            }
        }

        private static void MergeStage(HediffStage destination, HediffStage source)
        {
            if (source == null)
            {
                return;
            }
            MergeStatModifiers(destination.statOffsets, source.statOffsets, multiply: false);
            MergeStatModifiers(destination.statFactors, source.statFactors, multiply: true);
            if (!Mathf.Approximately(source.painFactor, 1f)) destination.painFactor *= source.painFactor;
            destination.painOffset += source.painOffset;
            if (!Mathf.Approximately(source.totalBleedFactor, 1f)) destination.totalBleedFactor *= source.totalBleedFactor;
            if (source.naturalHealingFactor >= 0f) destination.naturalHealingFactor = source.naturalHealingFactor;
            if (!Mathf.Approximately(source.fertilityFactor, 1f)) destination.fertilityFactor *= source.fertilityFactor;
            if (!Mathf.Approximately(source.hungerRateFactor, 1f)) destination.hungerRateFactor *= source.hungerRateFactor;
            destination.hungerRateFactorOffset += source.hungerRateFactorOffset;
            if (!Mathf.Approximately(source.restFallFactor, 1f)) destination.restFallFactor *= source.restFallFactor;
            destination.restFallFactorOffset += source.restFallFactorOffset;
            if (!Mathf.Approximately(source.socialFightChanceFactor, 1f)) destination.socialFightChanceFactor *= source.socialFightChanceFactor;
            if (source.vomitMtbDays > 0f) destination.vomitMtbDays = source.vomitMtbDays;
            if (source.makeImmuneTo != null)
            {
                if (destination.makeImmuneTo == null)
                {
                    destination.makeImmuneTo = new List<HediffDef>();
                }
                foreach (HediffDef immunity in source.makeImmuneTo)
                {
                    if (!destination.makeImmuneTo.Contains(immunity)) destination.makeImmuneTo.Add(immunity);
                }
            }
            if (source.capMods != null)
            {
                foreach (PawnCapacityModifier cap in source.capMods)
                {
                    destination.capMods.Add(new PawnCapacityModifier
                    {
                        capacity = cap.capacity,
                        offset = cap.offset,
                        setMax = cap.setMax,
                        postFactor = cap.postFactor,
                        statFactorMod = cap.statFactorMod,
                        setMaxCurveOverride = cap.setMaxCurveOverride,
                        setMaxCurveEvaluateStat = cap.setMaxCurveEvaluateStat
                    });
                }
            }
        }

        private static void MergeStatModifiers(List<StatModifier> destination, List<StatModifier> source, bool multiply)
        {
            if (source == null)
            {
                return;
            }
            foreach (StatModifier modifier in source)
            {
                StatModifier existing = destination.FirstOrDefault(x => x.stat == modifier.stat);
                if (existing == null)
                {
                    destination.Add(new StatModifier { stat = modifier.stat, value = modifier.value });
                }
                else if (multiply)
                {
                    existing.value *= modifier.value;
                }
                else
                {
                    existing.value += modifier.value;
                }
            }
        }
    }

    public static class TraitEffects
    {
        public static void ApplyKill(WeaponTraitDef trait, Pawn attacker, Pawn victim, DamageInfo? dinfo)
        {
            if (trait == null || attacker == null)
            {
                return;
            }
            if (trait.defName == "OnKill_PsyfocusGain")
            {
                attacker.psychicEntropy?.OffsetPsyfocusDirectly(0.2f);
                LogAdaptorTriggered(trait, attacker, "on-kill psyfocus gain");
            }
            if (trait.killThought != null && attacker.needs?.mood != null)
            {
                Thought thought = ThoughtMaker.MakeThought(trait.killThought);
                if (thought is Thought_WeaponTrait weaponThought)
                {
                    weaponThought.weapon = attacker.equipment?.Primary;
                }
                if (thought is Thought_Memory memory)
                {
                    attacker.needs.mood.thoughts.memories.TryGainMemory(memory);
                    LogAdaptorTriggered(trait, attacker, "on-kill thought");
                }
            }

            string workerName = trait.workerClass?.FullName ?? string.Empty;
            if (workerName.Contains("FoodFilledOnKill"))
            {
                if (FillNeed(attacker, "Food")) LogAdaptorTriggered(trait, attacker, "on-kill food need");
            }
            else if (workerName.Contains("ComfortFilledOnKill"))
            {
                if (FillNeed(attacker, "Comfort")) LogAdaptorTriggered(trait, attacker, "on-kill comfort need");
            }
            else if (workerName.Contains("JoyFilledOnKill"))
            {
                if (FillNeed(attacker, "Joy")) LogAdaptorTriggered(trait, attacker, "on-kill joy need");
            }
            else if (workerName.Contains("BeautyFilledOnKill"))
            {
                if (FillNeed(attacker, "Beauty")) LogAdaptorTriggered(trait, attacker, "on-kill beauty need");
            }
            else if (workerName.Contains("RestFilledOnKill"))
            {
                if (FillNeed(attacker, "Rest")) LogAdaptorTriggered(trait, attacker, "on-kill rest need");
            }
            else if (workerName.Contains("InOutdoorsFilledOnKill"))
            {
                bool filled = FillNeed(attacker, "Indoors") | FillNeed(attacker, "Outdoors");
                if (filled) LogAdaptorTriggered(trait, attacker, "on-kill indoors/outdoors need");
            }
            else if (workerName.Contains("ChemicalFilledOnKill"))
            {
                if (FillRandomChemicalNeed(attacker)) LogAdaptorTriggered(trait, attacker, "on-kill chemical need");
            }
            else if (workerName.Contains("InvisibilityOnKill"))
            {
                if (AddTemporaryHediff(attacker, "PsychicInvisibility", 480)) LogAdaptorTriggered(trait, attacker, "on-kill invisibility");
            }

            ApplyOnKillExtension(trait, attacker, victim);
        }

        public static void ApplyOnHit(WeaponTraitDef trait, Pawn attacker, Thing target)
        {
            if (trait == null || attacker == null || target == null || Rand.Value > 1f)
            {
                return;
            }
            foreach (DefModExtension extension in trait.modExtensions ?? Enumerable.Empty<DefModExtension>())
            {
                if (extension.GetType().FullName != "MorePersonaTraits.Extensions.WeaponTraitOnHitExtension")
                {
                    continue;
                }
                FieldInfoCache.Get(extension, "OnHitWorkers", out object workersObject);
                if (!(workersObject is IEnumerable workers)) continue;
                foreach (object worker in workers)
                {
                    if (worker == null) continue;
                    float chance = FieldInfoCache.GetFloat(worker, "ProcChance", 1f);
                    if (!Rand.Chance(chance)) continue;
                    ApplyOnHitWorker(trait, worker, attacker, target);
                }
            }
        }

        private static void ApplyOnHitWorker(WeaponTraitDef trait, object worker, Pawn attacker, Thing target)
        {
            string name = worker.GetType().Name;
            bool targetSelf = FieldInfoCache.GetBool(worker, "TargetSelf", false);
            Thing effectTarget = targetSelf ? attacker : target;
            bool requiresBothLiving = FieldInfoCache.GetBool(worker, "RequiresBothLiving", false);
            if (requiresBothLiving && (!IsLiving(attacker) || !IsLiving(target))) return;
            if (FieldInfoCache.GetBool(worker, "RequiresBio", false) && !(effectTarget is Pawn p && p.RaceProps.IsFlesh)) return;

            if (name.Contains("ApplyNeed"))
            {
                NeedDef needDef = FieldInfoCache.GetDef<NeedDef>(worker, "NeedDef");
                Need need = (effectTarget as Pawn)?.needs?.TryGetNeed(needDef);
                if (need != null)
                {
                    need.CurLevel += need.MaxLevel * FieldInfoCache.GetFloat(worker, "ProcMagnitude", 0f);
                    LogAdaptorTriggered(trait, attacker, "on-hit need");
                }
            }
            else if (name.Contains("ApplyHediff"))
            {
                HediffDef hediffDef = FieldInfoCache.GetDef<HediffDef>(worker, "HediffDef");
                Pawn pawn = effectTarget as Pawn;
                if (pawn != null && hediffDef != null)
                {
                    Hediff h = HediffMaker.MakeHediff(hediffDef, pawn);
                    h.Severity = FieldInfoCache.GetFloat(worker, "ProcMagnitude", 1f);
                    pawn.health.AddHediff(h);
                    LogAdaptorTriggered(trait, attacker, "on-hit hediff");
                }
            }
            else if (name.Contains("ApplyStun"))
            {
                Pawn stunnedPawn = effectTarget as Pawn;
                if (stunnedPawn?.stances?.stunner != null)
                {
                    stunnedPawn.stances.stunner.StunFor(GenTicks.SecondsToTicks(FieldInfoCache.GetFloat(worker, "StunDuration", 2f)), attacker, true, true, false);
                    LogAdaptorTriggered(trait, attacker, "on-hit stun");
                }
            }
            else if (name.Contains("HealInjury"))
            {
                Pawn pawn = effectTarget as Pawn;
                if (pawn != null)
                {
                    var injuries = new List<Hediff_Injury>();
                    pawn.health.hediffSet.GetHediffs(ref injuries, HediffUtility.CanHealNaturally);
                    if (injuries.Count > 0)
                    {
                        injuries.RandomElement().Heal(FieldInfoCache.GetFloat(worker, "ProcMagnitude", 0.125f) * pawn.HealthScale);
                        LogAdaptorTriggered(trait, attacker, "on-hit injury healing");
                    }
                }
            }
            else if (name.Contains("SpawnFilth"))
            {
                ThingDef filth = FieldInfoCache.GetDef<ThingDef>(worker, "Filth");
                if (filth != null && effectTarget.Spawned)
                {
                    FilthMaker.TryMakeFilth(effectTarget.Position, effectTarget.Map, filth);
                    LogAdaptorTriggered(trait, attacker, "on-hit filth");
                }
            }
        }

        private static void ApplyOnKillExtension(WeaponTraitDef trait, Pawn attacker, Pawn victim)
        {
            foreach (DefModExtension extension in trait.modExtensions ?? Enumerable.Empty<DefModExtension>())
            {
                if (extension.GetType().FullName != "MorePersonaTraits.Extensions.WeaponTraitOnKillExtension") continue;
                HediffDef hediffDef = FieldInfoCache.GetDef<HediffDef>(extension, "HediffDef");
                if (hediffDef != null && victim != null)
                {
                    victim.health.AddHediff(HediffMaker.MakeHediff(hediffDef, victim));
                    LogAdaptorTriggered(trait, attacker, "on-kill victim hediff");
                }
            }
        }

        internal static void LogAdaptorTriggered(WeaponTraitDef trait, Pawn pawn, string effect)
        {
            if (trait == null || pawn == null || OMWPersonaDevouringPawnMod.Settings?.logAdaptorTriggers != true)
            {
                return;
            }

            Log.Message("OMW_AdaptorTriggeredLog".Translate(
                pawn.LabelShortCap.Named("PAWN"),
                trait.LabelCap.Named("TRAIT"),
                effect.Named("EFFECT")));
        }

        private static bool IsLiving(Thing thing)
        {
            return thing is Pawn pawn && !pawn.Dead && pawn.RaceProps.IsFlesh;
        }

        private static bool FillNeed(Pawn pawn, string defName)
        {
            NeedDef def = DefDatabase<NeedDef>.GetNamedSilentFail(defName);
            Need need = pawn.needs?.TryGetNeed(def);
            if (need == null || !Rand.Chance(0.2f)) return false;
            need.CurLevel = need.MaxLevel;
            return true;
        }

        private static bool FillRandomChemicalNeed(Pawn pawn)
        {
            Need need = pawn.needs?.AllNeeds?.FirstOrDefault(x => x is Need_Chemical);
            if (need == null || !Rand.Chance(0.2f)) return false;
            need.CurLevel = need.MaxLevel;
            return true;
        }

        private static bool AddTemporaryHediff(Pawn pawn, string defName, int ticks)
        {
            HediffDef def = DefDatabase<HediffDef>.GetNamedSilentFail(defName);
            if (def == null) return false;
            Hediff h = HediffMaker.MakeHediff(def, pawn);
            pawn.health.AddHediff(h);
            HediffUtility.TryGetComp<HediffComp_Disappears>(h)?.SetDuration(ticks);
            return true;
        }
    }

    internal static class FieldInfoCache
    {
        public static bool Get(object instance, string fieldName, out object value)
        {
            value = instance?.GetType().GetField(fieldName)?.GetValue(instance);
            return value != null;
        }

        public static float GetFloat(object instance, string fieldName, float fallback)
        {
            object value = instance?.GetType().GetField(fieldName)?.GetValue(instance);
            return value is float f ? f : fallback;
        }

        public static bool GetBool(object instance, string fieldName, bool fallback)
        {
            object value = instance?.GetType().GetField(fieldName)?.GetValue(instance);
            return value is bool b ? b : fallback;
        }

        public static T GetDef<T>(object instance, string fieldName) where T : Def
        {
            return instance?.GetType().GetField(fieldName)?.GetValue(instance) as T;
        }
    }
}
