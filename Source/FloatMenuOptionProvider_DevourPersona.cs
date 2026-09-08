using System.Collections.Generic;
using RimWorld;
using Verse;

namespace OMWPersonaDevouringPawn
{
    public sealed class FloatMenuOptionProvider_DevourPersona : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;
        protected override bool Undrafted => true;
        protected override bool Multiselect => false;
        protected override bool RequiresManipulation => true;

        public override bool SelectedPawnValid(Pawn pawn, FloatMenuContext context)
        {
            return base.SelectedPawnValid(pawn, context) && PersonaDevouring.IsEligiblePawn(pawn);
        }

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            Pawn pawn = context.FirstSelectedPawn;
            if (pawn == null || !PersonaDevouring.IsSupportedPersonaWeapon(clickedThing))
            {
                return null;
            }
            return PersonaDevouring.MakeOption(pawn, clickedThing);
        }
    }
}
