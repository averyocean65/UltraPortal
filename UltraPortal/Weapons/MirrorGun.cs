using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public class MirrorGun : GunBase {
		private static ManualLogSource Logger => Plugin.LogSource;
		
		private static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
		private static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire"); 
		
		private Animator _animator;

		private readonly Vector2 _portalSize = new Vector2(9.5f, 11.5f);
		private Portal _portal;
		private DynamicPortalExit _primaryMirror;
		
		protected override void Start() {
			base.Start();
			AssetBundle portals = AssetBundleHelpers.LoadAssetBundle(Constants.AssetPaths.PortalBundleName);
			GameObject portalPrefab = portals.LoadAsset<GameObject>("Mirror");

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

			OnPrimaryFire += () => {
				SpawnMirror();
				_animator.Play(PrimaryFireAnimHash);
			};
			
			InitMirror();
		}

		private void SpawnMirror() {
			PlayerHeadRaycast(out bool success, out var hit);

			if (!success) {
				return;
			}

			_primaryMirror.Initialize(_portal, PortalSide.Enter, _portalSize, hit);
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
			_portal.minimumEntrySideSpeed = 10f;
            
			_portal.exit = _primaryMirror.transform;
			_portal.exitOffset = 1.5f;
			_portal.minimumExitSideSpeed = 10f;
			
			_portal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			_portal.fakeVPMatrix = Matrix4x4.zero;
			_portal.mirror = false; // stuff can't travel through it otherwise :/
			
			_portal.shape = new PlaneShape {
				width = _portalSize.x,
				height = _portalSize.y
			};
		}
	}
}