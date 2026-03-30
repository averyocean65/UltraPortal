using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Arm;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UltraPortal.Colorizers;
using UltraPortal.Extensions;
using UltraPortal.Shared;
using UnityEngine;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal {
	// "Come on down to the other side / Come with us through the gates of hell"
	// - "The Other Side" (2008) by Pendulum.
	public class DynamicPortalExit : MonoBehaviour {
		private static bool _playerNearEntry;
		private static bool _playerNearExit;

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
 
		private AudioSource _ambianceSource;

		private PortalSandboxObject _sandbox;
		
		private void Awake() {
			_sandbox = gameObject.AddComponent<PortalSandboxObject>();
			
			info = GetComponent<PortalInfo>();
			if (!info) {
				LogError($"{nameof(PortalInfo)} not found on {name}!");
			}
			
			_particles = info.spawnParticles;
			_particles.transform.localScale *= ModConfig.PortalScaleMod.GetValue();

			info.ambianceParticles.transform.localScale *= ModConfig.PortalScaleMod.GetValue();
			
			_passableBlockage = info.passable;

			_colorManager = gameObject.AddComponent<PortalColorManager>();
			_colorManager.associated = this;

			CalculateAssistance();

			GameObject keepActive = new GameObject($"{name} Keep Active");
			_keepActive = keepActive.AddComponent<KeepActive>();
			
			_toggleColliderAction += (portalSide, collider, toggle, assistance) => {
				ToggleColliders(toggle, collider, assistance, portalSide);
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
			if (_ambianceSource) {
				_ambianceSource.mute = true;
				_ambianceSource.Stop();
				Destroy(_ambianceSource.gameObject);
			}
			
			if (_currentTravellers == null) {
				_currentTravellers = new List<Collider>();
				return;
			}
			
			foreach (Collider leftover in _currentTravellers) {
				if (!leftover) {
					continue;
				}

				LogVerboseInfo($"Resetting: {leftover.name}");
				ToggleColliders(false, leftover, true, side);
			}
			
			_currentTravellers.Clear();
		}
		
		public void Initialize(Portal portal, PortalSide portalSide, RaycastHit hit) {
			Initialize(portal, portalSide, hit.point, hit.normal, hit.collider);
		}

		public void Initialize(Portal portal, PortalSide portalSide, Vector3 position, Vector3 forward,
			Collider hitCollider = null) {
			if (_sandbox) {
				if (_sandbox.frozen) {
					HudMessageReceiver.Instance.SendHudMessage("The portal you attempted to set is <color=#00FFFF>frozen</color>.");
					return;
				}
			}
			
			if (!portal) {
				Plugin.LogSource.LogError("Portal is invalid!");
				return;
			}

			Cleanup();

			if (ModConfig.ShowPortalSpawnParticles.GetValue() && _particles)
				_particles.Play();

			transform.forward = forward;
			transform.position = position + -forward.normalized * ModConfig.PortalWallOffset.GetValue();

			hostPortal = portal;
			side = portalSide;

			AddCollider(hitCollider, true);
			GetNearbyCollider();

			if (OnInitialized != null) {
				OnInitialized.Invoke();
			}

			_colorManager.ColorPortal();
			_keepActive.target = gameObject;

			ShowForwardArrow(transform.position, -transform.forward, 10f);

			if (!AudioManager.Instance) {
				LogError("Audio Manager is not present in scene!");
				return;
			}

			if (ModConfig.CanHearSFX.GetValue()) {
				AudioManager.Instance.PlayAudioFromAsset(AssetPaths.Sfx.PortalOpen, MainCamera.transform.position,
					spatialBlend: 0.0f);
			}

			if (ModConfig.CanHearAmbiance.GetValue()) {
				Vector3 ambianceSourcePos = transform.position;
				_ambianceSource = AudioManager.Instance.PlayAudioFromAsset(AssetPaths.Sfx.PortalAmbiance,
					ambianceSourcePos, true, minDistance: ModConfig.PortalAmbianceMinDistance.GetValue(),
					maxDistance: ModConfig.PortalAmbianceMaxDistance.GetValue());
			}
		}

		private void OnTriggerEnter(Collider other) {
			if (_colliders.Contains(other)) {
				return;
			}

			if (!other.attachedRigidbody) {
				if (!other.isTrigger) {
					LogVerboseInfo($"ENTRY TRAVEL: Adding {other.name} as collider!");
					AddCollider(other);
				}
				
				return;
			}
			
			if ((other.attachedRigidbody.isKinematic ||
			    other.attachedRigidbody.constraints == RigidbodyConstraints.FreezeAll)
			    && !LayerMaskDefaults.IsMatchingLayer(other.attachedRigidbody.gameObject.layer, LMD.EnemiesAndPlayer)) {
				LogVerboseError($"ENTRY TRAVEL: {other.name} has too many rigidbody constraints!");
				return;
			}

			// it is 2AM and I am tired, so I'm just hardcoding this edge-case
			if (other.name == "Projectile Parry Zone" || other.name.Contains("GroundCheck") || other.name.Contains("Ground Check")) {
				LogVerboseError($"ENTRY TRAVEL: {other.name} is a ground check or projectile parry zone!");
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
				
				// Vector3 dir = (transform.position - other.transform.position).normalized;
				// dir.y = 0.0f;
				// float dot = Vector3.Dot(transform.forward.normalized, dir);
				// HudMessageReceiver.Instance.SendHudMessage($"DOT ({name} & {other.name}): {dot}");
				
				if (attachedCollider.GetComponent<EnemyIdentifier>()) {
					LogVerboseInfo($"ENTRY TRAVEL: {attachedCollider.name} is {nameof(EnemyIdentifier)}");
					_currentTravellers.SafeAdd(attachedCollider);
				}
				
				LogVerboseInfo($"ENTRY TRAVEL: {attachedCollider.name} is traveller!");
				_currentTravellers.SafeAdd(other);
			}
			
			if (_passableBlockage) {
				if(_passableBlockage.activeSelf)
					return;
			}
			
			if (otherExit) {
				otherExit.CalculateAssistance();
				otherExit._toggleColliderAction.Invoke(side, other, true, otherExit.AssistedPortalTravel);
			}
			
			CalculateAssistance();
			_toggleColliderAction.Invoke(side, other, true, AssistedPortalTravel);
		}

		private void Update() {
			if (!PortalGunManager.Instance) {
				return;
			}

			int layer = PortalGunManager.Instance.IsUsingSpawnerArm ? ItemLayer : PortalLayer;
			gameObject.layer = layer;
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
					$"{name} doesn't have a portal blockage defined!");
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
			_toggleColliderAction.Invoke(side, other, false, true);
			_currentTravellers.SafeRemove(other);
		}

		public void Reset() {
			if (_sandbox) {
				if (_sandbox.frozen) {
					return;
				}
			}

			transform.parent = null;
			
			transform.position = PortalGunBase.DefaultPortalPosition;
			Cleanup();
		}

		private void HandleSpecialTraveller(bool value, Collider other, bool assisted, PortalSide inputSide) {
			if (other.GetComponent<NewMovement>()) {
				if (assisted && inputSide == side) {
					NewMovement.Instance.GetComponent<VerticalClippingBlocker>().enabled = !value;
					NewMovement.Instance.transform.Find("GroundCheck").gameObject.SetActive(!value);
					NewMovement.Instance.enabled = !value;
				}
				
				NewMovement.Instance.GetComponent<KeepInBounds>().enabled = !value;
				NewMovement.Instance.GetComponent<WallCheckGroup>().enabled = !value;
			}

			EnemyIdentifierIdentifier eidid = other.GetComponent<EnemyIdentifierIdentifier>();
			EnemyIdentifier eid;
			
			if (eidid) {
				eid = eidid.eid;
			}
			else {
				eid = other.GetComponent<EnemyIdentifier>();
			}

			if (eid) {
				if (!assisted) {
					value = false;
				}
				
				if (value) {
					eid.gce.toIgnore = _colliders;
				}
				else {
					StartCoroutine(IClearEnemyGroundCheck(eid));
				}
			}
		}

		private IEnumerator IClearEnemyGroundCheck(EnemyIdentifier eid) {
			yield return new WaitForSecondsRealtime(0.05f);
			eid.gce.toIgnore = new List<Collider>();
		}
		
		private void ToggleColliders(bool value, Collider other, bool assisted, PortalSide inputSide) {
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
				
				HandleSpecialTraveller(value, other, assisted, inputSide);
				Physics.IgnoreCollision(c, other, value);
			}
		}
	}
}