using System;
using System.Collections.Generic;
using ULTRAKILL.Portal;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public class DynamicPortalExit : MonoBehaviour {
		public static bool PlayerNearEntry;
		public static bool PlayerNearExit;

		private static Action<PortalSide, Collider, bool> _toggleColliderAction; 
		
		public bool IsEntityNear {
			get {
				if (side == PortalSide.Enter) {
					return PlayerNearEntry;
				}

				return PlayerNearExit;
			}
			set {
				if (side == PortalSide.Enter) {
					PlayerNearEntry = value;
					return;
				}

				PlayerNearExit = value;
			}
		}
		
		// PORTAL
		public Portal hostPortal;
		public PortalSide side;

		// COLLIDERS
		private BoxCollider portalTrigger;
		
		public Vector3 PortalCenter {
			get {
				if (side == PortalSide.Enter) {
					return hostPortal.entryCenter;
				}

				return hostPortal.exitCenter;
			}
		}

		private List<Collider> _colliders = new List<Collider>();

		private void Awake() {
			gameObject.layer = PortalLayer;
			
			_toggleColliderAction += (portalSide, collider, toggle) => {
				ToggleColliders(toggle, collider);
			};
		}


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
			
			// Spawn the portal trigger
			portalTrigger = gameObject.AddComponent<BoxCollider>();
			portalTrigger.isTrigger = true;
			portalTrigger.center = PortalCenter;
			portalTrigger.size = new Vector3(portalSize.x, portalSize.y, portalSize.x);
		}

		private void OnTriggerEnter(Collider other) {
			if (_colliders.Contains(other)) {
				return;
			}
			
			if (!other.attachedRigidbody) {
				if (!other.isTrigger) {
					_colliders.Add(other);
				}
				
				return;
			}

			_toggleColliderAction.Invoke(side, other, true);
		}

		private void OnTriggerExit(Collider other) {
			if (_colliders.Contains(other)) {
				return;
			}

			if (!other.attachedRigidbody) {
				return;
			}

			_toggleColliderAction.Invoke(side, other, false);
		}

		private void ToggleColliders(bool value, Collider other) {
			Plugin.LogSource.LogInfo($"{Enum.GetName(typeof(PortalSide), side)}: setting portals enabled to {value}");
			foreach (Collider c in _colliders) {
				if (!c) {
					continue;
				}
				Physics.IgnoreCollision(c, other, value);
			}
		}
	}
}