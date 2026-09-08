using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace OMWPersonaDevouringPawn
{
    public sealed class JobDriver_DevourPersona : JobDriver
    {
        private const TargetIndex WeaponIndex = TargetIndex.A;
        private const int DevourTicks = 120;

        private Thing Weapon => job.GetTarget(WeaponIndex).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Thing weapon = Weapon;
            if (weapon?.Spawned == true)
            {
                return pawn.Reserve(weapon, job, 1, -1, null, errorOnFailed);
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (Weapon?.Spawned == true)
            {
                Toil gotoToil = Toils_Goto.GotoThing(WeaponIndex, PathEndMode.ClosestTouch);
                gotoToil.FailOn(() => !PersonaDevouring.CanContinueDevouring(pawn, Weapon));
                yield return gotoToil;
            }

            Toil devourToil = Toils_General.Wait(DevourTicks, WeaponIndex);
            devourToil.FailOn(() => !PersonaDevouring.CanContinueDevouring(pawn, Weapon));
            yield return devourToil;

            yield return Toils_General.Do(delegate
            {
                if (!PersonaDevouring.Devour(pawn, Weapon))
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            });
        }
    }
}
