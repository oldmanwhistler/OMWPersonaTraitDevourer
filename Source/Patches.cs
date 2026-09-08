using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace OMWPersonaDevouringPawn
{
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
