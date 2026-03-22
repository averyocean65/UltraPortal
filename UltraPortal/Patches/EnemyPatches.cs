using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal {
	[HarmonyPatch]
	public static class EnemyPatches {
		public static List<EnemyIdentifier> AlreadyAppliedStyle = new List<EnemyIdentifier>();

		private static void ApplyStyleBonus(EnemyIdentifier eid, string id, int points, Color color) {
			LogVerboseInfo($"using style: {id}");
			StyleHUD.Instance.AddPoints(points, id,
				prefix: $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>",
				postfix: "</color>");	
			
			AlreadyAppliedStyle.Add(eid);
		}
		
		[HarmonyPostfix]
		[HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage))]
		public static void DeliverDamagePatch(EnemyIdentifier __instance) {
			if (AlreadyAppliedStyle.Contains(__instance)) {
				return;
			}
			
			LogVerboseInfo($"checking if enemy qualifies for style bonuses; name: {__instance.name}");
			
			if (__instance.hitterWeapons.Contains(PortalExplosionWeapon)) {
				ApplyStyleBonus(__instance, StyleSafetyHazardId, StyleSafetyHazardPoints, ModConfig.SafetyHazardColor.GetValue());
			}

			if (__instance.hitterWeapons.Contains(PortalProjectileWeapon)) {
				ApplyStyleBonus(__instance, StylePortalProjectileId, StylePortalProjectilePoints,
					ModConfig.ProjectileBonusColor.GetValue());
			}
		}
	}
}