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

            int added = PersonaDevouring.ApplyAllDevourableTraits(pawn);
            Log.Message($"[Persona Devouring Pawn] Applied {added} devourable persona traits to {pawn.LabelShort} in random order.");
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
