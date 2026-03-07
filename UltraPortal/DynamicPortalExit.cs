using System;
using System.Collections.Generic;
using System.Linq;
using ULTRAKILL.Portal;
using UltraPortal.Colorizers;
using UltraPortal.Extensions;
using UnityEngine;
using UnityEngine.Animations;
using static UltraPortal.Constants;

namespace UltraPortal {
	// i sincerely apologize for the code inside of this class, i will clean it up someday, i promise.
	public class DynamicPortalExit : MonoBehaviour {
		private const string TooCloseTrigger = "Too Close Trigger";
		
		private static bool _playerNearEntry;
		private static bool _playerNearExit;
		
		private const string ExpectedPassableName = "Passable";

		private const float SphereCheckRadius = 0.2f;

		private static Action<PortalSide, Collider, bool, bool> _toggleColliderAction;

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
		public PortalGunBase hostGun;
		
		public bool AssistedPortalTravel { get; private set; } = false;

		// COLLIDERS
		private DynamicPortalTooClose _tooClose;

		private GameObject _passableBlockage;
		
		public Vector3 PortalCenter {
			get {
				if (side == PortalSide.Enter) {
					return hostPortal.entryCenter;
				}

				return hostPortal.exitCenter;
			}
		}

		public bool IsBlocked {
			get {
				if (!_passableBlockage) {
					return false;
				}

				return _passableBlockage.activeSelf;
			}
		}

		private List<Collider> _colliders = new List<Collider>();
		private List<Collider> _currentTravellers = new List<Collider>();
		
		private ParticleSystem _particles;
		private PortalColorManager _colorManager;

		private KeepActive _keepActive;

		private void Awake() {
			gameObject.layer = PortalLayer;
			_particles = GetComponentInChildren<ParticleSystem>();
			_passableBlockage = transform.Find(ExpectedPassableName).gameObject;

			_colorManager = gameObject.AddComponent<PortalColorManager>();
			_colorManager.associated = this;

			CalculateAssistance();

			GameObject keepActive = new GameObject($"{name} Keep Active");
			_keepActive = keepActive.AddComponent<KeepActive>();
			
			// Get too close trigger
			Transform tooCloseTransform = transform.Find(TooCloseTrigger);
			if (!tooCloseTransform) {
				Plugin.LogSource.LogError($"Failed to find {TooCloseTrigger} on {name}.");
				return;
			}

			_tooClose = tooCloseTransform.gameObject.AddComponent<DynamicPortalTooClose>();
			
			_toggleColliderAction += (portalSide, collider, toggle, assistance) => {
				// if (assistedPortalTravel && portalSide != side) {
				// 	return;
				// }
				
				ToggleColliders(toggle, collider, assistance);
			};
		}

		private void AddCollider(Collider c) {
			if (!c) {
				return;
			}
			
			Plugin.LogSource.LogInfo($"Adding collider: {c.name}");
			
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

		private void CalculateAssistance() {
			// Check if portal is facing upwards
			float dot = Mathf.Abs(Vector3.Dot(transform.forward, NewMovement.Instance.rb.GetGravityVector()));
			AssistedPortalTravel = dot > 0.6f;
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
					ToggleColliders(false, leftover, true);
					ToggleColliders(false, leftover, false);
				}
			}

			_colliders = new List<Collider>();
			
			if(ModConfig.ShowPortalSpawnParticles && _particles)
				_particles.Play();

			transform.forward = -hit.normal;
			transform.position = hit.point + hit.normal.normalized * ModConfig.PortalWallOffset;
			
			hostPortal = portal;
			side = portalSide;

			AddCollider(hit.collider);
			GetNearbyCollider();

			if (OnInitialized != null) {
				OnInitialized.Invoke();
			}
			
			_colorManager.ColorPortal();
			_keepActive.target = gameObject;
		}

		private void OnTriggerEnter(Collider other) {
			if (_currentTravellers.Contains(other)) {
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
			
			if (other.attachedRigidbody.isKinematic ||
			    other.attachedRigidbody.constraints == RigidbodyConstraints.FreezeAll) {
				return;
			}

			// it is 2AM and I am tired, so I'm just hardcoding this edge-case
			if (other.name == "Projectile Parry Zone") {
				return;
			}
			
			Collider attachedCollider = other.attachedRigidbody.GetComponent<Collider>();

			// if (_tooClose) {
			// 	if (!_tooClose.travellers.Contains(other)) {
			// 		Vector3 direction = (transform.position - other.transform.position);
			// 		float dot = Vector3.Dot(transform.forward, direction.normalized);
			//
			// 		if (dot < 0.0f && !AssistedPortalTravel) {
			// 			return;
			// 		}
			// 	}
			// }

			if (!_currentTravellers.Contains(other)) {
				// i know hardcoding is bad, but i can't find anything else to identify environment chunks by.
				// so until someone finds something noteworthy about these stupid little things, this will stay.
				if (other.name.Contains("EnvironmentChunk")) {
					return;
				}

				if (attachedCollider.GetComponent<EnemyIdentifier>()) {
					_currentTravellers.Add(attachedCollider);
				}
				
				_currentTravellers.Add(other);
			}
			
			if (_passableBlockage) {
				if(_passableBlockage.activeSelf)
					return;
			}
			
			CalculateAssistance();
			
			_toggleColliderAction.Invoke(side, other, true, AssistedPortalTravel);
		}

		private void OnDestroy() {
			Destroy(_keepActive.gameObject);
			
			if (!hostGun || !hostPortal) {
				return;
			}

			if (hostGun is PortalGun portalGun) {
				switch (side) {
					case PortalSide.Enter:
						portalGun.SpawnEntry(true);
						break;
					case PortalSide.Exit:
						portalGun.SpawnExit(true);
						break;
				}
			} else if (hostGun is MirrorGun mirrorGun) {
				mirrorGun.SpawnPrimaryMirror(true);
			}
		}

		public bool ShouldBeDisabled() {
			if (_passableBlockage) {
				if (_passableBlockage.activeInHierarchy) {
					return true;
				}
			}
			
			// filter
			_currentTravellers = _currentTravellers.Where(x => x != null && x.gameObject.activeInHierarchy).ToList();

			_currentTravellers.ForEach(c => {
				Plugin.LogSource.LogInfo($"{name} has traveller: {c.name}");
			});
			return _currentTravellers.Count < 1;
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
			if (_currentTravellers.Contains(other)) {
				_currentTravellers.Remove(other);
			}
			
			if (_colliders.Contains(other)) {
				return;
			}

			if (!other.attachedRigidbody) {
				return;
			}
			
			if (_passableBlockage) {
				if(_passableBlockage.activeSelf)
					return;
			}
			
			_toggleColliderAction.Invoke(side, other, false, AssistedPortalTravel);
		}

		public void Reset() {
			foreach (var traveller in _currentTravellers) {
				// just in case
				ToggleColliders(false, traveller, false);
				ToggleColliders(false, traveller, true);
			}

			_currentTravellers = new List<Collider>();
		}

		private void ToggleColliders(bool value, Collider other, bool requiredAssistance) {
			if (_colliders == null || !other) {
				return;
			}
			
			foreach (Collider c in _colliders) {
				if (!c) {
					continue;
				}

				if (!requiredAssistance || !other.CompareTag("Player")) {
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