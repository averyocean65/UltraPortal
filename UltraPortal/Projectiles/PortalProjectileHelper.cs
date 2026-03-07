using System;
using ULTRAKILL.Portal;
using UltraPortal.Extensions;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal.Projectiles {
	public class PortalProjectileHelper : MonoBehaviour {
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

		private void OnTriggerEnter(Collider other) {
			EnemyIdentifier id = other.GetComponentInParent<EnemyIdentifier>();
			
			if (id) {
				if (!portal) {
					return;
				}

				if (exit.IsBlocked) {
					return;
				}
				
				Rigidbody rb = id.gameObject.GetComponent<Rigidbody>();
				Transform desiredExitTransform = ModConfig.UseOtherPortalForProjectileTeleport
					? OtherExitTransform
					: exit.transform;
				
				DynamicPortalExit desiredExit = ModConfig.UseOtherPortalForProjectileTeleport
					? desiredExitTransform.GetComponent<DynamicPortalExit>()
					: exit;
				
				id.transform.position = desiredExitTransform.position - desiredExitTransform.forward;
				
				float multiplier = desiredExit.AssistedPortalTravel
					? ModConfig.ProjectileEnemyGroundPortalBoostMultiplier
					: ModConfig.ProjectileEnemyNormalPortalBoostMultiplier;
				
				rb.velocity = -desiredExitTransform.forward * ModConfig.PortalProjectileSpeed * multiplier;
				return;
			}

			if (!other) {
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

				exit.transform.parent = hit.transform;
				exit.Initialize(portal, exit.side, hit);
			}
		}
	}
}