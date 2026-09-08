using System;
using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using RimWorld;
using Verse;

namespace OMWPersonaDevouringPawn
{
    public static class PersonaDevouringDebugActions
    {
        [DebugAction("Persona Devouring Pawn", "Apply all devourable persona traits", requiresRoyalty: true, displayPriority: 1001)]
        public static void ApplyAllDevourableTraits()
        {
            Pawn pawn = Find.Selector.SingleSelectedThing as Pawn
                ?? Find.CurrentMap?.mapPawns.FreeColonistsSpawned.FirstOrDefault();
            if (pawn?.health == null)
            {
                Log.Warning("[Persona Devouring Pawn] Select a pawn before applying devourable persona traits.");
                return;
            }

            HashSet<string> beforeTraitDefNames = pawn.health.hediffSet.hediffs
                .OfType<Hediff_DevouredPersonaTrait>()
                .Select(hediff => hediff.TraitDefName)
                .Where(defName => !defName.NullOrEmpty())
                .ToHashSet();

            int added = PersonaDevouring.ApplyAllDevourableTraits(pawn);
            Log.Message($"[Persona Devouring Pawn] Applied {added} devourable persona traits to {pawn.LabelShort} in random order.");
            ValidateDebugTraitApplication(pawn, beforeTraitDefNames);
        }

        private static void ValidateDebugTraitApplication(Pawn pawn, HashSet<string> beforeTraitDefNames)
        {
            List<Hediff_DevouredPersonaTrait> devoured = pawn.health.hediffSet.hediffs
                .OfType<Hediff_DevouredPersonaTrait>()
                .ToList();

            foreach (Hediff_DevouredPersonaTrait hediff in devoured)
            {
                if (beforeTraitDefNames.Contains(hediff.TraitDefName))
                {
                    continue;
                }

                WeaponTraitDef trait = DefDatabase<WeaponTraitDef>.GetNamedSilentFail(hediff.TraitDefName);
                if (trait == null)
                {
                    Log.Error($"[Persona Devouring Pawn] Debug validation: added hediff references missing persona trait '{hediff.TraitDefName}'.");
                    continue;
                }

                TraitDisposition disposition = TraitAdapters.Classify(trait, out string reason);
                if (disposition != TraitDisposition.Supported
                    || PersonaDevouringTraitRules.IsBlacklisted(trait)
                    || PersonaDevouringTraitRules.IsReplacedByOwnedTrait(pawn, trait))
                {
                    Log.Error($"[Persona Devouring Pawn] Debug validation: added hediff '{trait.defName}' should not have been added. Disposition={disposition}, reason={reason ?? "none"}.");
                }
            }

            ValidateStageOverlaps(devoured);
        }

        private static void ValidateStageOverlaps(IEnumerable<Hediff_DevouredPersonaTrait> hediffs)
        {
            var statOffsets = new Dictionary<StatDef, List<string>>();
            var statFactors = new Dictionary<StatDef, List<string>>();
            var capacityModifiers = new Dictionary<PawnCapacityDef, List<string>>();
            var immunities = new Dictionary<HediffDef, List<string>>();
            var scalarModifiers = new Dictionary<string, List<string>>();

            foreach (Hediff_DevouredPersonaTrait hediff in hediffs)
            {
                string source = hediff.TraitDefName ?? "unknown";
                HediffStage stage = hediff.CurStage;
                foreach (StatModifier modifier in stage.statOffsets ?? Enumerable.Empty<StatModifier>())
                {
                    if (modifier?.stat != null && Math.Abs(modifier.value) > 0.0001f)
                    {
                        AddModifier(statOffsets, modifier.stat, source);
                    }
                }
                foreach (StatModifier modifier in stage.statFactors ?? Enumerable.Empty<StatModifier>())
                {
                    if (modifier?.stat != null && Math.Abs(modifier.value - 1f) > 0.0001f)
                    {
                        AddModifier(statFactors, modifier.stat, source);
                    }
                }
                foreach (PawnCapacityModifier modifier in stage.capMods ?? Enumerable.Empty<PawnCapacityModifier>())
                {
                    if (modifier?.capacity != null && (Math.Abs(modifier.offset) > 0.0001f || Math.Abs(modifier.postFactor - 1f) > 0.0001f || modifier.SetMaxDefined || modifier.statFactorMod != null))
                    {
                        AddModifier(capacityModifiers, modifier.capacity, source);
                    }
                }
                foreach (HediffDef immunity in stage.makeImmuneTo ?? Enumerable.Empty<HediffDef>())
                {
                    if (immunity != null)
                    {
                        AddModifier(immunities, immunity, source);
                    }
                }

                AddScalar(scalarModifiers, "painFactor", stage.painFactor != 1f, source);
                AddScalar(scalarModifiers, "painOffset", stage.painOffset != 0f, source);
                AddScalar(scalarModifiers, "totalBleedFactor", stage.totalBleedFactor != 1f, source);
                AddScalar(scalarModifiers, "naturalHealingFactor", stage.naturalHealingFactor >= 0f, source);
                AddScalar(scalarModifiers, "fertilityFactor", stage.fertilityFactor != 1f, source);
                AddScalar(scalarModifiers, "hungerRateFactor", stage.hungerRateFactor != 1f, source);
                AddScalar(scalarModifiers, "hungerRateFactorOffset", stage.hungerRateFactorOffset != 0f, source);
                AddScalar(scalarModifiers, "restFallFactor", stage.restFallFactor != 1f, source);
                AddScalar(scalarModifiers, "restFallFactorOffset", stage.restFallFactorOffset != 0f, source);
                AddScalar(scalarModifiers, "socialFightChanceFactor", stage.socialFightChanceFactor != 1f, source);
                AddScalar(scalarModifiers, "vomitMtbDays", stage.vomitMtbDays > 0f, source);
            }

            LogOverlaps("stat offset", statOffsets);
            LogOverlaps("stat factor", statFactors);
            LogOverlaps("capacity modifier", capacityModifiers);
            LogOverlaps("immunity", immunities);
            LogOverlaps("stage modifier", scalarModifiers);
        }

        private static void AddScalar(Dictionary<string, List<string>> modifiers, string key, bool active, string source)
        {
            if (active)
            {
                AddModifier(modifiers, key, source);
            }
        }

        private static void AddModifier<TKey>(Dictionary<TKey, List<string>> modifiers, TKey key, string source)
        {
            if (!modifiers.TryGetValue(key, out List<string> sources))
            {
                sources = new List<string>();
                modifiers.Add(key, sources);
            }
            sources.Add(source);
        }

        private static void LogOverlaps<TKey>(string category, Dictionary<TKey, List<string>> modifiers)
        {
            foreach (KeyValuePair<TKey, List<string>> pair in modifiers.Where(pair => pair.Value.Distinct().Count() > 1))
            {
                Log.Error($"[Persona Devouring Pawn] Debug validation: devoured hediffs overlap the same {category} '{pair.Key}': {string.Join(", ", pair.Value.Distinct())}.");
            }
        }

        [DebugAction("Persona Devouring Pawn", "Generate supported persona weapon test set", requiresRoyalty: true, displayPriority: 1000)]
        public static void GenerateSupportedPersonaWeaponTestSet()
        {
            Map map = Find.CurrentMap;
            if (map == null)
            {
                Log.Warning("[Persona Devouring Pawn] Cannot generate persona weapons without a map.");
                return;
            }

            Pawn centerPawn = Find.Selector.SingleSelectedThing as Pawn
                ?? map.mapPawns.FreeColonistsSpawned.FirstOrDefault();
            IntVec3 center = centerPawn?.Position ?? map.Center;

            List<ThingDef> weaponDefs = DefDatabase<ThingDef>.AllDefsListForReading
                .Where(IsPersonaWeaponDef)
                .OrderBy(_ => Rand.Value)
                .ToList();
            List<WeaponTraitDef> traitDefs = DefDatabase<WeaponTraitDef>.AllDefsListForReading
                .Where(IsSupportedTrait)
                .OrderBy(_ => Rand.Value)
                .ToList();

            if (weaponDefs.Count == 0 || traitDefs.Count == 0)
            {
                Log.Warning("[Persona Devouring Pawn] No supported persona weapon types or traits were found.");
                return;
            }

            int total = Math.Max(weaponDefs.Count, traitDefs.Count);
            int spawned = 0;
            for (int i = 0; i < total; i++)
            {
                ThingDef weaponDef = weaponDefs[i % weaponDefs.Count];
                Thing thing = ThingMaker.MakeThing(weaponDef, GenStuff.DefaultStuffFor(weaponDef));
                CompBladelinkWeapon comp = thing.TryGetComp<CompBladelinkWeapon>();
                if (comp == null)
                {
                    thing.Destroy();
                    continue;
                }

                comp.TraitsListForReading.Clear();
                comp.TraitsListForReading.Add(traitDefs[i % traitDefs.Count]);
                if (GenPlace.TryPlaceThing(thing, center, map, ThingPlaceMode.Near))
                {
                    spawned++;
                }
                else if (!thing.Destroyed)
                {
                    thing.Destroy();
                }
            }

            Log.Message($"[Persona Devouring Pawn] Generated {spawned} persona weapon test items across {weaponDefs.Count} weapon types and {traitDefs.Count} supported traits.");
        }

        private static bool IsPersonaWeaponDef(ThingDef def)
        {
            if (def == null || !def.IsWeapon || def.comps == null)
            {
                return false;
            }
            return def.comps.Any(x => x?.compClass == typeof(CompBladelinkWeapon));
        }

        private static bool IsSupportedTrait(WeaponTraitDef trait)
        {
            return trait != null && trait.weaponCategory == WeaponCategoryDefOf.BladeLink && TraitAdapters.Classify(trait, out _) == TraitDisposition.Supported;
        }
    }
}
