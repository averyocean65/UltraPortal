using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal {
	[HarmonyPatch]
	public static class EnemyPatches {
		public static List<EnemyIdentifier> AlreadyAppliedStyle = new List<EnemyIdentifier>();

		public static void ApplyStyleBonus(EnemyIdentifier eid, string id, int points, Color color) {
			ApplyStyleBonus(id, points, color);
			AlreadyAppliedStyle.Add(eid);
		}
		
		public static void ApplyStyleBonus(string id, int points, Color color) {
			LogVerboseInfo($"using style: {id}");
			StyleHUD.Instance.AddPoints(points, id,
				prefix: $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>",
				postfix: "</color>");
		}

		private static IEnumerator IStyleCooldown(EnemyIdentifier eid, float cooldown) {
			yield return new WaitForSecondsRealtime(cooldown);
			AlreadyAppliedStyle.Remove(eid);
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
				ApplyStyleBonus(__instance, StylePortalHitId, StylePortalHitPoints,
					ModConfig.HitBonusColor.GetValue());

				__instance.StartCoroutine(IStyleCooldown(__instance, 0.1f));
			}
		}
	}
}