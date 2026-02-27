using System;
using System.Collections.Generic;
using ULTRAKILL.Portal;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public class DynamicPortalExit : MonoBehaviour {
		// PORTAL
		public Portal hostPortal;
		public PortalSide side;

		public Vector3 PortalCenter {
			get {
				if (side == PortalSide.Enter) {
					return hostPortal.entryCenter;
				}

				return hostPortal.exitCenter;
			}
		}

		private List<Collider> _colliders = new List<Collider>();

		private void GetNearbyCollider(Vector3 position) {
			bool success = Physics.Raycast(position + transform.forward,
				-transform.forward,
				out var hit,
				1f,
				EnvironmentLayer,
				QueryTriggerInteraction.Ignore);

			if (!success) {
				return;
			}

			if (!hit.collider) {
				return;
			}

			if (_colliders.Contains(hit.collider)) {
				return;
			}
			
			_colliders.Add(hit.collider);
			Plugin.LogSource.LogInfo($"Collider found: {hit.collider.name}");
		}
		
		private void GetNearbyCollider(Vector2 portalSize) {
			Vector3 offset = (transform.forward.normalized * portalSize) / 2;
			Vector3 topLeft = PortalCenter + new Vector3(offset.x, offset.y, offset.z);
			Vector3 topRight = PortalCenter + new Vector3(-offset.x, offset.y, -offset.z);
			Vector3 bottomLeft = PortalCenter - new Vector3(offset.x, offset.y, offset.z);
			Vector3 bottomRight = PortalCenter - new Vector3(-offset.x, offset.y, -offset.z);
			
			GetNearbyCollider(topLeft);
			GetNearbyCollider(topRight);
			GetNearbyCollider(bottomLeft);
			GetNearbyCollider(bottomRight);
		}
		
		public void Initialize(Portal portal, PortalSide portalSide, Vector2 portalSize, RaycastHit hit) {
			if (!portal) {
				Plugin.LogSource.LogError("Portal is invalid!");
				return;
			}

			_colliders = new List<Collider>();

			transform.forward = -hit.normal;
			transform.position = hit.point + hit.normal.normalized * 0.01f;

			hostPortal = portal;
			side = portalSide;

			if (hit.collider) {
				_colliders.Add(hit.collider);
			}
			
			GetNearbyCollider(portalSize);
		}

		private void Update() {
			float distance = Vector3.Distance(PortalCenter, MainCamera.transform.position);
			foreach (Collider c in _colliders) {
				c.gameObject.SetActive(distance >= 5);
			}
		}
	}
}