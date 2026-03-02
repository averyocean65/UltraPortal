using System;
using System.Collections.Generic;
using ULTRAKILL.Portal;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	// i sincerely apologize for the code inside of this class, i will clean it up someday, i promise.
	public class DynamicPortalExit : MonoBehaviour {
		public static bool PlayerNearEntry;
		public static bool PlayerNearExit;

		private static float SphereCheckRadius = 0.2f;

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
		private bool assistedPortalTravel = false;

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
		private List<Collider> _currentTravellers = new List<Collider>();

		private void Awake() {
			gameObject.layer = PortalLayer;
			
			_toggleColliderAction += (portalSide, collider, toggle) => {
				// if (assistedPortalTravel && portalSide != side) {
				// 	return;
				// }
				
				ToggleColliders(toggle, collider);
			};
		}

		private void AddCollider(Collider c) {
			if (!c) {
				return;
			}

			if (c.CompareTag("Portal Exit")) {
				return;
			}
			
			_colliders.Add(c);
		}
		
		private void GetNearbyCollider(Vector2 portalSize) {
			Collider[] sphereCheck = Physics.OverlapSphere(transform.position, SphereCheckRadius, EnvironmentLayer, QueryTriggerInteraction.Ignore);
			_colliders.AddRange(sphereCheck);
			
			// Raycast to ensure some slopes (like at the start of 8-2) work
			// GameObject test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			// test.transform.position = transform.position + transform.forward * 10f;

			RaycastHit[] sphereCastResults = Physics.SphereCastAll(transform.position, SphereCheckRadius,
				transform.forward, 3f, EnvironmentLayer, QueryTriggerInteraction.Ignore);
			if (sphereCastResults.Length < 1) {
				return;
			}

			foreach (RaycastHit hit in sphereCastResults) {
				if (!hit.collider) {
					continue;
				}
				
				AddCollider(hit.collider);
			}
		}

		public void Initialize(Portal portal, PortalSide portalSide, Vector2 portalSize, RaycastHit hit) {
			if (!portal) {
				Plugin.LogSource.LogError("Portal is invalid!");
				return;
			}

			if (_colliders.Count > 0) {
				foreach (Collider leftover in _currentTravellers) {
					if (!leftover) {
						continue;
					}
					
					Plugin.LogSource.LogInfo($"Resetting: {leftover.name}");
					ToggleColliders(false, leftover);
				}
			}

			_colliders = new List<Collider>();

			transform.forward = -hit.normal;
			transform.position = hit.point + hit.normal.normalized * 0.05f;
			
			// Check if portal is facing upwards
			float dot = Vector3.Dot(transform.forward, NewMovement.Instance.rb.GetGravityVector());
			assistedPortalTravel = dot > 0.6f;
			
			hostPortal = portal;
			side = portalSide;

			AddCollider(hit.collider);
			
			GetNearbyCollider(portalSize);
			
			// Spawn the portal trigger
			portalTrigger = gameObject.GetComponent<BoxCollider>();
		}

		private void OnTriggerEnter(Collider other) {
			if (_colliders.Contains(other)) {
				return;
			}
			
			if (!other.attachedRigidbody) {
				if (!other.isTrigger) {
					AddCollider(other);
				}
				
				return;
			}

			// it is 2AM and I am tired, so I'm just hardcoding this edge-case
			if (other.name == "Projectile Parry Zone") {
				return;
			}
			
			_toggleColliderAction.Invoke(side, other, true);
			
			if (!_currentTravellers.Contains(other)) {
				_currentTravellers.Add(other);
			}
		}

		private void OnTriggerExit(Collider other) {
			if (_colliders.Contains(other)) {
				return;
			}

			if (!other.attachedRigidbody) {
				return;
			}

			_toggleColliderAction.Invoke(side, other, false);

			if (_currentTravellers.Contains(other)) {
				_currentTravellers.Remove(other);
			}
		}

		private void ToggleColliders(bool value, Collider other) {
			foreach (Collider c in _colliders) {
				if (!c) {
					continue;
				}

				if (!assistedPortalTravel || !other.CompareTag("Player")) {
					Physics.IgnoreCollision(c, other, value);
				}
				else {
				 	//c.gameObject.SetActive(!value);
				    c.enabled = !value;
				}
			}
		}
	}
}