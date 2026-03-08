using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UltraPortal.Colorizers;
using UltraPortal.Projectiles;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public sealed class MirrorGun : PortalGunBase {
		private static ManualLogSource Logger => Plugin.LogSource;
		
		private static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
		private static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire"); 
		
		private Animator _animator;

		private readonly Vector2 _portalSize = new Vector2(11f, 11f);
		private Portal _mirrorPortal;
		public DynamicPortalExit PrimaryMirror { get; private set; }
		
		public void SpawnPrimaryMirror(bool reinit = false) {
			PrimaryMirror = SpawnPortalExit("Primary Mirror", PortalSide.Enter, _mirrorPortal, AssetPaths.Mirror);
			if (PrimaryMirror) {
				PrimaryMirror.OnInitialized += () => PrimaryMirror.SetPassable(true);
			}
			
			if (reinit) {
				InitMirror();
			}
		}
		
		
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
			
			SpawnPrimaryMirror();
			
			OnPrimaryFire += () => {
				FireProjectile(PrimaryMirror, _mirrorPortal);
				_animator.Play(PrimaryFireAnimHash);
			};

			OnSecondaryFire += () => { };
			
			UpdateLastProjectile(PrimaryMirror.side);
			InitMirror();
		}
		
		private void InitMirror() {
			// GameObject mirrorObject = new GameObject("Mirror Head") {
			// 	layer = PortalLayer
			// };
			//
			// _mirrorPortal = mirrorObject.AddComponent<Portal>();
			//
			// _mirrorPortal.additionalSampleThreshold = 0;
			// _mirrorPortal.allowCameraTraversals = true;
			// _mirrorPortal.appearsInRecursions = true;
			// _mirrorPortal.canHearAudio = false;
			// _mirrorPortal.canSeeItself = true;
			// _mirrorPortal.canSeePortalLayer = true;
			// _mirrorPortal.clippingMethod = PortalClippingMethod.Default;
			// _mirrorPortal.consumeAudio = false;
			// _mirrorPortal.disableRange = 0;
			// _mirrorPortal.enableOverrideFog = false;
			// _mirrorPortal.enterOffset = 1.5f;
			// _mirrorPortal.entry = PrimaryMirror.transform;
			// _mirrorPortal.minimumEntrySideSpeed = ModConfig.MinimumEntryExitSpeed;
   //          
			// _mirrorPortal.exit = PrimaryMirror.transform;
			// _mirrorPortal.exitOffset = 1.5f;
			// _mirrorPortal.minimumExitSideSpeed = ModConfig.MinimumEntryExitSpeed;
			//
			// _mirrorPortal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			// _mirrorPortal.fakeVPMatrix = Matrix4x4.zero;
			// _mirrorPortal.mirror = false; // stuff can't travel through it otherwise :/
			//
			// _mirrorPortal.shape = new PlaneShape {
			// 	width = _portalSize.x,
			// 	height = _portalSize.y
			// };

			_mirrorPortal = CreatePortal("Mirror Head", PrimaryMirror.transform, PrimaryMirror.transform, _portalSize);
		}

		public override bool ShouldBeReset() {
			if (!PrimaryMirror) {
				return true;
			}

			return PrimaryMirror.ShouldBeDisabled();
		}

		public void Reset() {
			if (!PrimaryMirror)
				return;
			
			PrimaryMirror.Reset();
			PrimaryMirror.SetPassable(false);
			PrimaryMirror.transform.position = DefaultPortalPosition;
		}
	}
}