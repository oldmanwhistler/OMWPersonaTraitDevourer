using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace OMWPersonaDevouringPawn
{
    public sealed class PersonaDevouringTraitRuleDef : Def
    {
        public List<string> blacklistedTraits = new List<string>();
        public List<PersonaDevouringTraitReplacement> replacementRules = new List<PersonaDevouringTraitReplacement>();
    }

    public sealed class PersonaDevouringTraitReplacement
    {
        public string replacementTrait;
        public List<string> replacedTraits = new List<string>();
    }

    public static class PersonaDevouringTraitRules
    {
        public static bool IsBlacklisted(WeaponTraitDef trait)
        {
            return trait != null && IsBlacklisted(trait.defName);
        }

        public static bool IsBlacklisted(string traitDefName)
        {
            if (traitDefName.NullOrEmpty())
            {
                return false;
            }

            return DefDatabase<PersonaDevouringTraitRuleDef>.AllDefsListForReading
                .Any(rule => rule.blacklistedTraits?.Contains(traitDefName) == true);
        }

        public static bool IsReplacedByOwnedTrait(Pawn pawn, WeaponTraitDef incomingTrait)
        {
            if (pawn == null || incomingTrait == null)
            {
                return false;
            }

            return AllReplacementsFor(incomingTrait.defName)
                .Any(replacement => PersonaDevouring.IsOwned(pawn, replacement.replacementTrait));
        }

        public static IEnumerable<string> TraitsOverriddenBy(string replacementTraitDefName)
        {
            return AllReplacementsFor(replacementTraitDefName)
                .SelectMany(replacement => replacement.replacedTraits ?? Enumerable.Empty<string>())
                .Distinct();
        }

        public static IEnumerable<string> TraitsThatOverride(string replacedTraitDefName)
        {
            if (replacedTraitDefName.NullOrEmpty())
            {
                return Enumerable.Empty<string>();
            }

            return DefDatabase<PersonaDevouringTraitRuleDef>.AllDefsListForReading
                .SelectMany(rule => rule.replacementRules ?? Enumerable.Empty<PersonaDevouringTraitReplacement>())
                .Where(replacement => replacement != null
                    && !replacement.replacementTrait.NullOrEmpty()
                    && replacement.replacedTraits?.Contains(replacedTraitDefName) == true)
                .Select(replacement => replacement.replacementTrait)
                .Distinct();
        }

        private static IEnumerable<PersonaDevouringTraitReplacement> AllReplacementsFor(string replacedTraitDefName)
        {
            if (replacedTraitDefName.NullOrEmpty())
            {
                return Enumerable.Empty<PersonaDevouringTraitReplacement>();
            }

            return DefDatabase<PersonaDevouringTraitRuleDef>.AllDefsListForReading
                .SelectMany(rule => rule.replacementRules ?? Enumerable.Empty<PersonaDevouringTraitReplacement>())
                .Where(replacement => replacement != null
                    && !replacement.replacementTrait.NullOrEmpty()
                    && replacement.replacedTraits?.Contains(replacedTraitDefName) == true);
        }
    }
}
