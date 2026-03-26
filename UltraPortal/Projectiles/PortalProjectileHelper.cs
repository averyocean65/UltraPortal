using System;
using ULTRAKILL.Portal;
using UltraPortal.Extensions;
using UnityEngine;

using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal.Projectiles {
	public class PortalProjectileHelper : MonoBehaviour {
		public static float PortalScaleSceneStart = 1.0f;
		public static float ProjectileDamage = 0.49f; // almost filth health
		
		private Transform OtherExitTransform {
			get {
				if (exit.side == PortalSide.Enter) {
					return portal.exit;
				}

				return portal.entry;
			}
		}
		
		public DynamicPortalExit exit;
		public Portal portal;

		private void DamageEnemy(EnemyIdentifier eid, bool wasTeleported = true) {
			if (wasTeleported) {
				eid.hitterWeapons.Add(PortalProjectileWeapon);
			}
			eid.SimpleDamage(ProjectileDamage);
		}
		
		private void OnTriggerEnter(Collider other) {
			if (!other) {
				return;
			}
			
			EnemyIdentifier eid = other.GetComponentInParent<EnemyIdentifier>();
			
			if (eid) {
				if (!portal) {
					return;
				}

				if (exit.IsBlocked) {
					return;
				}

				if (eid.Dead) {
					return;
				}
				
				if (!EnemyHelpers.IsLightEnemy(eid.enemyType)) {
					LogVerboseWarning("Enemy is not a light enemy! Not teleporting!");
					DamageEnemy(eid, false);
					return;
				}
				
				Rigidbody rb = eid.gameObject.GetComponent<Rigidbody>();
				Transform desiredExitTransform = ModConfig.UseOtherPortalForProjectileTeleport.GetValue()
					? OtherExitTransform
					: exit.transform;
				
				DynamicPortalExit desiredExit = ModConfig.UseOtherPortalForProjectileTeleport.GetValue()
					? desiredExitTransform.GetComponent<DynamicPortalExit>()
					: exit;
				
				eid.transform.position = desiredExitTransform.position - desiredExitTransform.forward;
				
				float multiplier = desiredExit.AssistedPortalTravel
					? ModConfig.ProjectileEnemyGroundPortalBoostMultiplier.GetValue()
					: ModConfig.ProjectileEnemyNormalPortalBoostMultiplier.GetValue();
				
				rb.velocity = -desiredExitTransform.forward * 100.0f * multiplier;
				DamageEnemy(eid);
				return;
			}

			if (EnvironmentLayer.Contains(other.gameObject.layer)) {
				// Do a quick raycast
				bool success = Physics.Raycast(transform.position - transform.forward.normalized,
					transform.forward,
					out var hit,
					float.PositiveInfinity,
					EnvironmentLayer,
					QueryTriggerInteraction.Ignore);

				if (!success) {
					Plugin.LogSource.LogError("Failed to find surface that projectile hit!");
					return;
				}

				PortalGunManager.SummonPortalExit(exit, portal, hit.point, -hit.normal, hit.transform, hit.collider);
			}
		}
	}
}