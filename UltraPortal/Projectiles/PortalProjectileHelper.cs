using System;
using ULTRAKILL.Portal;
using UltraPortal.Extensions;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal.Projectiles {
	public class PortalProjectileHelper : MonoBehaviour {
		public DynamicPortalExit exit;
		public Portal portal;

		private void OnTriggerEnter(Collider other) {
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
				exit.Initialize(portal, exit.side, hit);
			}
		}
	}
}