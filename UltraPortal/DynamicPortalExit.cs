using System;
using System.Collections.Generic;
using ULTRAKILL.Portal;
using UltraPortal.Colorizers;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	// i sincerely apologize for the code inside of this class, i will clean it up someday, i promise.
	public class DynamicPortalExit : MonoBehaviour {
		private static bool _playerNearEntry;
		private static bool _playerNearExit;
		
		private const string ExpectedPassableName = "Passable";

		private const float SphereCheckRadius = 0.2f;

		private static Action<PortalSide, Collider, bool> _toggleColliderAction;

		public Action OnInitialized;
		
		public bool IsEntityNear {
			get {
				if (side == PortalSide.Enter) {
					return _playerNearEntry;
				}

				return _playerNearExit;
			}
			set {
				if (side == PortalSide.Enter) {
					_playerNearEntry = value;
					return;
				}

				_playerNearExit = value;
			}
		}
		
		// PORTAL
		public Portal hostPortal;
		public PortalSide side;
		public bool AssistedPortalTravel { get; private set; } = false;

		// COLLIDERS
		private BoxCollider _portalTrigger;

		private GameObject _passableBlockage;
		
		public Vector3 PortalCenter {
			get {
				if (side == PortalSide.Enter) {
					return hostPortal.entryCenter;
				}

				return hostPortal.exitCenter;
			}
		}

		private List<Collider> _colliders = new List<Collider>();
		private readonly List<Collider> _currentTravellers = new List<Collider>();
		
		private ParticleSystem _particles;
		private PortalColorManager _colorManager;

		private void Awake() {
			gameObject.layer = PortalLayer;
			_particles = GetComponentInChildren<ParticleSystem>();
			_passableBlockage = transform.Find(ExpectedPassableName).gameObject;

			_colorManager = gameObject.AddComponent<PortalColorManager>();
			_colorManager.associated = this;
			
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
			
			_colliders.Add(c);
		}
		
		private void GetNearbyCollider() {
			Collider[] sphereCheck = Physics.OverlapSphere(transform.position, SphereCheckRadius, EnvironmentLayer, QueryTriggerInteraction.Ignore);
			_colliders.AddRange(sphereCheck);
			
			// Raycast to ensure some slopes (like at the start of 8-2) work
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

		public void Initialize(Portal portal, PortalSide portalSide, RaycastHit hit) {
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
			
			_particles.Play();

			transform.forward = -hit.normal;
			transform.position = hit.point + hit.normal.normalized * 0.05f;
			
			// Check if portal is facing upwards
			float dot = Vector3.Dot(transform.forward, NewMovement.Instance.rb.GetGravityVector());
			AssistedPortalTravel = dot > 0.6f;
			
			hostPortal = portal;
			side = portalSide;

			AddCollider(hit.collider);
			GetNearbyCollider();
			
			// Spawn the portal trigger
			_portalTrigger = gameObject.GetComponent<BoxCollider>();

			if (OnInitialized != null) {
				OnInitialized.Invoke();
			}
			
			_colorManager.ColorPortal();
		}

		private void OnTriggerEnter(Collider other) {
			if (_passableBlockage) {
				if(_passableBlockage.activeSelf)
					return;
			}
			
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
			
			// make sure player can't call _toggleColliderAction from behind portal
			// issue is that objects going through the portals temporarily achieve a negative value, so
			// this is a big difficult to figure out
			Vector3 direction = (transform.position - other.transform.position);
			float dot = Vector3.Dot(transform.forward, direction.normalized);
			
			if(dot < -0.5f && !AssistedPortalTravel) {
				return;
			}
			
			_toggleColliderAction.Invoke(side, other, true);
			
			if (!_currentTravellers.Contains(other)) {
				_currentTravellers.Add(other);
			}
		}

		public void SetPassable(bool canPass) {
			if(!_passableBlockage) {
				Plugin.LogSource.LogInfo(
					$"{name} doesn't have a portal blockage defined! Ensure your blockage is called: \"{ExpectedPassableName}\"");
				return;
			}
			
			_passableBlockage.SetActive(!canPass);
		}

		private void OnTriggerExit(Collider other) {
			if (_passableBlockage) {
				if(_passableBlockage.activeSelf)
					return;
			}
			
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

				if (!AssistedPortalTravel || !other.CompareTag("Player")) {
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