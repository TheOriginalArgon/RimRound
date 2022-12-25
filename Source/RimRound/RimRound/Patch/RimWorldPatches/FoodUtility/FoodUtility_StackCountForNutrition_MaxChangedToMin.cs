﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch(nameof(FoodUtility.StackCountForNutrition))]
    public class FoodUtility_StackCountForNutrition_MaxChangedToMin
    {
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

			MethodInfo mi = typeof(Mathf).GetMethod(nameof(Mathf.RoundToInt), BindingFlags.Static | BindingFlags.Public);
			MethodInfo miReplacement = typeof(Mathf).GetMethod(nameof(Mathf.FloorToInt), BindingFlags.Static | BindingFlags.Public);

			for (int jndex = 0; jndex < codeInstructions.Count; ++jndex) 
			{
				if (codeInstructions[jndex].Calls(mi))
				{
					codeInstructions[jndex].operand = miReplacement;
				}
			}


			return codeInstructions;
		}
	}
}
