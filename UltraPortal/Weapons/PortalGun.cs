using System;
using System.Collections;
using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UltraPortal.Projectiles;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public class PortalGun : GunBase {
		public static readonly Vector3 DefaultPortalPosition = new Vector3(0, -1e6f, 0);

		private static ManualLogSource Logger => Plugin.LogSource;

		private static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
		private static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire"); 

		private Portal _portal;
		private GameObject _portalObject;
		private readonly Vector2 _portalSize = new Vector2(6, 8);

		private DynamicPortalExit _portalEntry;
		private DynamicPortalExit _portalExit;
       
        private Animator _animator;

        private bool BothPortalsInit {
	        get {
		        if (!_portalEntry || !_portalExit) {
			        return false;
		        }

		        return _portalEntry.transform.position.y > DefaultPortalPosition.y &&
		               _portalExit.transform.position.y > DefaultPortalPosition.y;
	        }
        }

        private void FireProjectile(DynamicPortalExit exit, int animHash, bool altProjectile) {
	        string projectileAsset = altProjectile ? AssetPaths.MainPortalProjectile : AssetPaths.AltPortalProjectile;
	        Projectile projectile = SpawnProjectileFromAsset(projectileAsset, ModConfig.PortalProjectileSpeed);
	        PortalProjectileHelper helper = projectile.gameObject.AddComponent<PortalProjectileHelper>();
	        helper.exit = exit;
	        helper.portal = _portal;
	        
	        _animator.Play(animHash);
        }
        
		protected override void Start() {
			base.Start();
			AssetBundle portals = AssetBundleHelpers.LoadAssetBundle(AssetPaths.PortalBundle);
			GameObject portalPrefab = portals.LoadAsset<GameObject>(AssetPaths.PortalExit);

			if (!portalPrefab) {
				Logger.LogError("Failed to load portal prefab!");
				return;
			}

			_animator = GetComponentInChildren<Animator>();
			if (!_animator) {
				HudMessageReceiver.Instance.SendHudMessage("<color=#ff000>Animator is invalid!</color>");
			}

			Vector3 spawnPos = DefaultPortalPosition;
			
			GameObject portalEntryObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			portalEntryObject.name = "Entry";
			_portalEntry = portalEntryObject.AddComponent<DynamicPortalExit>();
			_portalEntry.side = PortalSide.Enter;
			_portalEntry.hostPortal = _portal;
			
			GameObject portalExitObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			portalExitObject.name = "Exit";
			_portalExit = portalExitObject.AddComponent<DynamicPortalExit>();
			_portalExit.OnInitialized += UpdatePortalPassable;
			_portalExit.side = PortalSide.Exit;
			_portalExit.hostPortal = _portal;

			OnPrimaryFire += () => {
				FireProjectile(_portalEntry, PrimaryFireAnimHash, false);
			};
			
			OnSecondaryFire += () => {
				FireProjectile(_portalExit, SecondaryFireAnimHash, true);
			};
			
			InitPortals();
		}

		private void UpdatePortalPassable() {
			_portalEntry.SetPassable(BothPortalsInit);
			_portalExit.SetPassable(BothPortalsInit);
		}

		private void OnDestroy() {
			Destroy(_portal.gameObject);
			Destroy(_portalEntry.gameObject);
			Destroy(_portalExit.gameObject);
		}

		private void InitPortals() {
			_portalObject = new GameObject("Portal") {
				layer = PortalLayer
			};

			_portal = _portalObject.AddComponent<Portal>();
			
			_portal.additionalSampleThreshold = 0;
			_portal.allowCameraTraversals = true;
			_portal.appearsInRecursions = true;
			_portal.canHearAudio = false;
			_portal.canSeeItself = true;
			_portal.canSeePortalLayer = true;
			_portal.clippingMethod = PortalClippingMethod.Default;
			_portal.consumeAudio = false;
			_portal.disableRange = 0;
			_portal.enableOverrideFog = false;
			_portal.enterOffset = 1.5f;
			_portal.entry = _portalEntry.transform;
            _portal.minimumEntrySideSpeed = ModConfig.MinimumEntryExitSpeed;
            
			_portal.exit = _portalExit.transform;
			_portal.exitOffset = 1.5f;
			_portal.minimumExitSideSpeed = ModConfig.MinimumEntryExitSpeed;
			
			_portal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			_portal.fakeVPMatrix = Matrix4x4.zero;
			_portal.mirror = false;
			_portal.shape = new PlaneShape {
				width = _portalSize.x,
				height = _portalSize.y
			};
		}

		public void Reset() {
			if (_portalEntry) {
				_portalEntry.transform.position = DefaultPortalPosition;
			}

			if (_portalExit) {
				_portalExit.transform.position = DefaultPortalPosition;
			}

			UpdatePortalPassable();
		}
	}
}