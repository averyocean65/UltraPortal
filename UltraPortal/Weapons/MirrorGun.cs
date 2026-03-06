using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UltraPortal.Colorizers;
using UltraPortal.Projectiles;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public class MirrorGun : PortalGunBase {
		private static ManualLogSource Logger => Plugin.LogSource;
		
		private static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
		private static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire"); 
		
		private Animator _animator;

		private readonly Vector2 _portalSize = new Vector2(11f, 11f);
		private Portal _portal;
		private DynamicPortalExit _primaryMirror;
		
		protected override void Start() {
			base.Start();
			AssetBundle portals = AssetBundleHelpers.LoadAssetBundle(AssetPaths.PortalBundle);
			GameObject portalPrefab = portals.LoadAsset<GameObject>(AssetPaths.Mirror);

			if (!portalPrefab) {
				Logger.LogError("Failed to load mirror prefab!");
				return;
			}

			_animator = GetComponentInChildren<Animator>();
			if (!_animator) {
				HudMessageReceiver.Instance.SendHudMessage("<color=#ff000>Animator is invalid!</color>");
			}

			Vector3 spawnPos = Vector3.down * 100000;
			
			GameObject primaryMirrorObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			primaryMirrorObject.name = "Mirror Transform";
			_primaryMirror = primaryMirrorObject.AddComponent<DynamicPortalExit>();
			_primaryMirror.side = PortalSide.Enter;
			_primaryMirror.OnInitialized += () => _primaryMirror.SetPassable(true);

			OnPrimaryFire += () => {
				FireProjectile(_primaryMirror, _portal);
				_animator.Play(PrimaryFireAnimHash);
			};
			
			UpdateLastProjectile(_primaryMirror.side);
			InitMirror();
		}
		
		private void InitMirror() {
			GameObject mirrorObject = new GameObject("Mirror Head") {
				layer = PortalLayer
			};

			_portal = mirrorObject.AddComponent<Portal>();
			
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
			_portal.entry = _primaryMirror.transform;
			_portal.minimumEntrySideSpeed = ModConfig.MinimumEntryExitSpeed;
            
			_portal.exit = _primaryMirror.transform;
			_portal.exitOffset = 1.5f;
			_portal.minimumExitSideSpeed = ModConfig.MinimumEntryExitSpeed;
			
			_portal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			_portal.fakeVPMatrix = Matrix4x4.zero;
			_portal.mirror = false; // stuff can't travel through it otherwise :/
			
			_portal.shape = new PlaneShape {
				width = _portalSize.x,
				height = _portalSize.y
			};
		}

		public override bool ShouldBeReset() {
			if (!_primaryMirror) {
				return true;
			}

			return _primaryMirror.ShouldBeDisabled();
		}

		public void Reset() {
			if (!_primaryMirror)
				return;
			_primaryMirror.transform.position = PortalGun.DefaultPortalPosition;
		}
	}
}