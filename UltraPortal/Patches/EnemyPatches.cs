using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	[HarmonyPatch]
	public static class EnemyPatches {
		public static List<EnemyIdentifier> AlreadyDealtWith = new List<EnemyIdentifier>();
		
		[HarmonyPostfix]
		[HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage))]
		public static void DeliverDamagePatch(EnemyIdentifier __instance) {
			if (AlreadyDealtWith.Contains(__instance)) {
				return;
			}
			
			Plugin.LogSource.LogInfo($"checking if enemy qualifies for safety hazard; name: {__instance.name}");
			
			if (__instance.hitterWeapons.Contains(PortalExplosionWeapon)) {
				Plugin.LogSource.LogInfo("using safety hazard style!");
				StyleHUD.Instance.AddPoints(StyleSafetyHazardPoints, StyleSafetyHazardId,
					prefix: $"<color=#{ColorUtility.ToHtmlStringRGB(ModConfig.SafetyHazardColor)}>",
					postfix: "</color>");
			}
			
			AlreadyDealtWith.Add(__instance);
		}
	}
}