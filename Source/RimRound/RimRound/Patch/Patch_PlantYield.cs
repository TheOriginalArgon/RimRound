using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.RimRound.Patch
{
    [HarmonyPatch]
    public static class Patch_PlantYield
    {
        public static void SpawnExtra(Plant plant, Pawn actor)
        {
            if (plant.def.HasModExtension<PlantExtension_MultipleYield>())
            {
                PlantExtension_MultipleYield extension = plant.def.GetModExtension<PlantExtension_MultipleYield>();
                int num = plant.YieldNow();
                if (num > 0)
                {
                    foreach (ThingDef thingDef in extension.extraYields)
                    {
                        Thing thing = ThingMaker.MakeThing(thingDef, null);
                        thing.stackCount = num;
                        if (actor.Faction != Faction.OfPlayer)
                        {
                            thing.SetForbidden(true);
                        }
                        GenPlace.TryPlaceThing(thing, actor.Position, actor.Map, ThingPlaceMode.Near);
                    }
                }
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(JobDriver_PlantWork), "MakeNewToils")]
        private static IEnumerable<CodeInstruction> MultipleYieldTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.Calls(AccessTools.Method(typeof(GenPlace), nameof(GenPlace.TryPlaceThing), new Type[]
                {
                    typeof(Thing),
                    typeof(IntVec3),
                    typeof(Map),
                    typeof(ThingPlaceMode),
                    typeof(Action<Thing, int>),
                    typeof(Predicate<IntVec3>),
                    typeof(Rot4)
                })))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_PlantYield), nameof(Patch_PlantYield.SpawnExtra)));
                }
            }
        }
    }
}
