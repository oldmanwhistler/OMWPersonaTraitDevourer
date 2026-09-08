using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace OMWPersonaDevouringPawn
{
    [HarmonyPatch(typeof(ThoughtWorker_WeaponTrait), "PostProcessLabel")]
    public static class Patch_ThoughtWorker_WeaponTrait_PostProcessLabel
    {
        public static void Postfix(ThoughtWorker_WeaponTrait __instance, Pawn p, string label, ref string __result)
        {
            if (__instance == null || p == null || p.equipment?.bondedWeapon != null)
            {
                return;
            }

            string weaponName = PersonaDevouring.GetDevouredBondedWeaponName(p, __instance.def);
            if (!weaponName.NullOrEmpty())
            {
                __result = label.Formatted(p.Named("PAWN"), weaponName.Named("WEAPON")).ToString();
            }
        }
    }

    [HarmonyPatch(typeof(ThoughtWorker_WeaponTrait), "PostProcessDescription")]
    public static class Patch_ThoughtWorker_WeaponTrait_PostProcessDescription
    {
        public static void Postfix(ThoughtWorker_WeaponTrait __instance, Pawn p, string description, ref string __result)
        {
            if (__instance == null || p == null || p.equipment?.bondedWeapon != null)
            {
                return;
            }

            string weaponName = PersonaDevouring.GetDevouredBondedWeaponName(p, __instance.def);
            if (!weaponName.NullOrEmpty())
            {
                __result = description.Formatted(p.Named("PAWN"), weaponName.Named("WEAPON")).ToString();
            }
        }
    }

    [HarmonyPatch(typeof(ThoughtWorker_WeaponTraitBonded), "CurrentStateInternal")]
    public static class Patch_ThoughtWorker_WeaponTraitBonded
    {
        public static void Postfix(ThoughtWorker_WeaponTraitBonded __instance, Pawn p, ref ThoughtState __result)
        {
            if (__result.Active || p == null || __instance?.def == null)
            {
                return;
            }

            if (PersonaDevouring.HasDevouredBondedThought(p, __instance.def))
            {
                __result = ThoughtState.ActiveAtStage(0);
            }
        }
    }

    public static class Patch_MorePersonaTraits_FemaleThought
    {
        public static void Postfix(ThoughtWorker __instance, Pawn p, ref ThoughtState __result)
        {
            if (__result.Active || p == null || p.gender != Gender.Female || __instance?.def == null)
            {
                return;
            }

            if (PersonaDevouring.HasDevouredBondedThought(p, __instance.def))
            {
                __result = ThoughtState.ActiveAtStage(0);
            }
        }

        public static void TryPatch(Harmony harmony)
        {
            Type workerType = AccessTools.TypeByName("MorePersonaTraits.WorkerClasses.ThoughtWorkerClasses.ThoughtWorker_WeaponTraitGenderFemale");
            if (workerType == null)
            {
                return;
            }

            System.Reflection.MethodInfo target = AccessTools.Method(workerType, "CurrentStateInternal", new[] { typeof(Pawn) });
            System.Reflection.MethodInfo postfix = AccessTools.Method(typeof(Patch_MorePersonaTraits_FemaleThought), nameof(Postfix));
            if (target != null && postfix != null)
            {
                harmony.Patch(target, postfix: new HarmonyMethod(postfix));
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Patch_Pawn_Kill
    {
        public static void Prefix(Pawn __instance, DamageInfo? dinfo)
        {
            try
            {
                if (__instance == null || __instance.Dead || !dinfo.HasValue || !(dinfo.Value.Instigator is Pawn attacker))
                {
                    return;
                }
                PersonaDevouring.NotifyPawnKilled(attacker, __instance, dinfo);
            }
            catch (Exception ex)
            {
                Log.Error($"[Persona Devouring Pawn] Kill effect failed: {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
    public static class Patch_InventoryRightClick
    {
        public sealed class State
        {
            public float y;
        }

        public static void Prefix(float y, out State __state)
        {
            __state = new State { y = y };
        }

        public static void Postfix(ITab_Pawn_Gear __instance, float width, Thing thing, bool inventory, State __state)
        {
            if (thing == null || __state == null || Event.current == null || Event.current.type != EventType.MouseDown || Event.current.button != 1)
            {
                return;
            }
            if (!Mouse.IsOver(new Rect(0f, __state.y, width, 28f)))
            {
                return;
            }

            Pawn pawn = Traverse.Create(__instance).Property("SelPawnForGear").GetValue<Pawn>();
            if (pawn == null || !PersonaDevouring.IsEligiblePawn(pawn) || PersonaDevouring.IsExcludedApparel(pawn, thing) || (!PersonaDevouring.IsInventoryTarget(pawn, thing) && !PersonaDevouring.IsEquippedTarget(pawn, thing)))
            {
                return;
            }

            FloatMenuOption option = PersonaDevouring.MakeOption(pawn, thing);
            Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption> { option }));
            Event.current.Use();
        }
    }
}
