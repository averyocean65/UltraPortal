using System;
using System.Collections.Generic;
using System.Linq;
using Interop.std;
using ULTRAKILL.Portal;
using UltraPortal.Colorizers;
using UltraPortal.Extensions;
using UltraPortal.Shared;
using UnityEngine;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal {
	// i sincerely apologize for the code inside of this class, i will clean it up someday, i promise.
	public class DynamicPortalExit : MonoBehaviour {
		private const string TooCloseTrigger = "Too Close Trigger";
		
		private static bool _playerNearEntry;
		private static bool _playerNearExit;
		
		private const string ExpectedPassableName = "Passable";

		private Action<PortalSide, Collider, bool, bool> _toggleColliderAction;

		public Action OnInitialized;
		public DynamicPortalExit otherExit;
		public PortalInfo info;
		
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

		public bool IsInitialized => (transform.position - PortalGunBase.DefaultPortalPosition).sqrMagnitude > 1;

		// PORTAL
		public Portal hostPortal;
		public PortalSide side;
		public PortalGunBase hostGun;
		
		public bool AssistedPortalTravel { get; private set; } = false;
		
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
			info = GetComponent<PortalInfo>();
			if (!info) {
				LogError($"{nameof(PortalInfo)} not found on {name}!");
			}
			
			gameObject.layer = PortalLayer;
			_particles = info.spawnParticles;
			_passableBlockage = info.passable;

			_colorManager = gameObject.AddComponent<PortalColorManager>();
			_colorManager.associated = this;

			CalculateAssistance();

			GameObject keepActive = new GameObject($"{name} Keep Active");
			_keepActive = keepActive.AddComponent<KeepActive>();
			
			_toggleColliderAction += (portalSide, collider, toggle, assistance) => {
				ToggleColliders(toggle, collider, assistance);
			};
		}

		private void AddCollider(Collider c, bool checkChildren = true) {
			if (!c) {
				return;
			}
			
			if (c.isTrigger) {
				return;
			}
			
			if (info.portalColliders.Contains(c)) {
				return;
			}
			
			Vector3 dir = (c.transform.position - transform.position).normalized;
			float dot = Mathf.Abs(Vector3.Dot(-transform.forward, dir));
			
			LogVerboseInfo($"Dot of {c.name}: {dot}");
			if (Mathf.Abs(dot) < ModConfig.PerpendicularThreshold.GetValue()) {
				LogVerboseInfo($"Rejected: {c.name} (close to perpendicular to exit)");
				return;
			}

			LogInfo($"Adding collider: {c.name}; Checking children: {checkChildren}");
			
			_colliders.SafeAdd(c);

			if (checkChildren) {
				Collider[] children = c.GetComponentsInChildren<Collider>();
				foreach (var child in children) {
					LogVerboseInfo($"Child ({child.name}) layer: {LayerMask.LayerToName(child.gameObject.layer)}");
					
					if (!EnvironmentLayer.Contains(child.gameObject.layer)) {
						LogVerboseWarning($"Rejected: {child.name}");
						continue;
					}
					
					AddCollider(child, false);
				}
			}
		}
		
		private void GetNearbyCollider() {
			Collider[] sphereCheck = Physics.OverlapSphere(transform.position,
				ModConfig.PortalSphereCheckRadius.GetValue(), EnvironmentLayer, QueryTriggerInteraction.Ignore);

			foreach (Collider c in sphereCheck) {
				if (!c) {
					continue;
				}
				
				AddCollider(c);
			}
		}

		private void CalculateAssistance() {
			// Check if portal is facing upwards
			float dot = Mathf.Abs(Vector3.Dot(transform.forward.normalized, NewMovement.Instance.rb.GetGravityVector().normalized));
			LogVerboseInfo($"{name} dot to {NewMovement.Instance.rb.GetGravityVector().normalized}: {dot}");
			AssistedPortalTravel = dot > ModConfig.AssistedPortalThreshold.GetValue();
		}

		private void Cleanup() {
			if (_currentTravellers == null) {
				_currentTravellers = new List<Collider>();
				return;
			}
			
			foreach (Collider leftover in _currentTravellers) {
				if (!leftover) {
					continue;
				}

				LogVerboseInfo($"Resetting: {leftover.name}");
				ToggleColliders(false, leftover, true);
			}
			
			_currentTravellers.Clear();
		}

		public void Initialize(Portal portal, PortalSide portalSide, RaycastHit hit) {
			if (!portal) {
				Plugin.LogSource.LogError("Portal is invalid!");
				return;
			}
			
			Cleanup();
			
			if(ModConfig.ShowPortalSpawnParticles.GetValue() && _particles)
				_particles.Play();

			transform.forward = -hit.normal;
			transform.position = hit.point + hit.normal.normalized * ModConfig.PortalWallOffset.GetValue();
			
			hostPortal = portal;
			side = portalSide;

			AddCollider(hit.collider, true);
			GetNearbyCollider();

			if (OnInitialized != null) {
				OnInitialized.Invoke();
			}
			
			_colorManager.ColorPortal();
			_keepActive.target = gameObject;
			
			ShowForwardArrow(transform.position, -transform.forward, 10f);
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
			
			if (other.attachedRigidbody.isKinematic ||
			    other.attachedRigidbody.constraints == RigidbodyConstraints.FreezeAll) {
				return;
			}

			// it is 2AM and I am tired, so I'm just hardcoding this edge-case
			if (other.name == "Projectile Parry Zone" || other.name.Contains("GroundCheck") || other.name.Contains("Ground Check")) {
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
					_currentTravellers.SafeAdd(attachedCollider);
				}
				
				_currentTravellers.SafeAdd(other);
			}
			
			if (_passableBlockage) {
				if(_passableBlockage.activeSelf)
					return;
			}
			
			CalculateAssistance();
			
			if (otherExit) {
				otherExit.CalculateAssistance();
				otherExit._toggleColliderAction.Invoke(side, other, true, otherExit.AssistedPortalTravel);
			}
			_toggleColliderAction.Invoke(side, other, true, AssistedPortalTravel);
		}

		private void OnDestroy() {
			Cleanup();
			LogInfo("FINISHED CLEANUP!");
			
			Destroy(_keepActive.gameObject);
			
			if (!hostGun || !hostPortal) {
				return;
			}

			switch (hostGun.variant) {
				case WeaponVariant.BlueVariant:
					PortalGun portalGun = hostGun as PortalGun;
					
					if (side == PortalSide.Enter) {
						portalGun.SpawnEntry(true);
					}
					else {
						portalGun.SpawnExit(true);
					}
					break;
				case WeaponVariant.GreenVariant:
					MirrorGun mirrorGun = hostGun as MirrorGun;
					
					if (side == PortalSide.Enter) {
						mirrorGun.SpawnPrimaryMirror(true);
					}
					else {
						mirrorGun.SpawnFlippedMirror(true);
					}
					break;
				case WeaponVariant.RedVariant:
					TwistGun twistGun = hostGun as TwistGun;
					
					if (side == PortalSide.Enter) {
						twistGun.SpawnEntry(true);
					}
					else {
						twistGun.SpawnExit(true);
					}
					break;
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
				LogVerboseInfo($"{name} has traveller: {c.name}");
			});
			return _currentTravellers.Count < 1;
		}

		public void SetPassable(bool canPass) {
			if(!_passableBlockage) {
				LogWarning(
					$"{name} doesn't have a portal blockage defined! Ensure your blockage is called: \"{ExpectedPassableName}\"");
				return;
			}
			
			_passableBlockage.SetActive(!canPass);
		}

		private void OnTriggerExit(Collider other) {
			if (_colliders.Contains(other)) {
				return;
			}
			
			if (_passableBlockage) {
				if(_passableBlockage.activeSelf)
					return;
			}

			if (otherExit) {
				otherExit.CalculateAssistance();
				otherExit._toggleColliderAction.Invoke(side, other, false, true);
			}
			_toggleColliderAction.Invoke(side, other, false, AssistedPortalTravel);
			_currentTravellers.SafeRemove(other);
		}

		public void Reset() {
			Cleanup();
		}

		private void HandleSpecialTraveller(bool value, Collider other, bool assisted) {
			if (other.GetComponent<NewMovement>()) {
				if (assisted) {
					NewMovement.Instance.GetComponent<VerticalClippingBlocker>().enabled = !value;
					NewMovement.Instance.transform.Find("GroundCheck").gameObject.SetActive(!value);
					// NewMovement.Instance.enabled = !value;
				}
				
				NewMovement.Instance.GetComponent<KeepInBounds>().enabled = !value;
				NewMovement.Instance.GetComponent<WallCheckGroup>().enabled = !value;
			}

			EnemyIdentifier eid = other.GetComponent<EnemyIdentifier>();
			if (eid) {
				if (!assisted) {
					value = true;
				}
				other.attachedRigidbody.detectCollisions = value;
			}
		}
		
		private void ToggleColliders(bool value, Collider other, bool assisted) {
			if (_colliders == null || !other) {
				return;
			}

			if (_colliders.Count < 1) {
				return;
			}
			
			foreach (Collider c in _colliders) {
				if (!c) {
					continue;
				}
				
				if (!value) {
					LogVerboseInfo($"Re-enabling collisions for: {other.name} and {c.name}");
				}
				
				HandleSpecialTraveller(value, other, assisted);
				Physics.IgnoreCollision(c, other, value);
			}
		}
	}
}