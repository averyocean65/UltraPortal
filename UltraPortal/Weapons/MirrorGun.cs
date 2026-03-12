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
		private Portal _flippedMirrorPortal;
		public DynamicPortalExit PrimaryMirror { get; private set; }
		public DynamicPortalExit FlippedMirror { get; private set; }
		
		public void SpawnPrimaryMirror(bool reinit = false) {
			PrimaryMirror = SpawnPortalExit("Primary Mirror", PortalSide.Enter, _mirrorPortal, AssetPaths.Mirror);
			if (PrimaryMirror) {
				PrimaryMirror.OnInitialized += () => PrimaryMirror.SetPassable(true);
			}
			
			if (reinit) {
				InitMirror();
			}
		}
		
		public void SpawnFlippedMirror(bool reinit = false) {
			FlippedMirror = SpawnPortalExit("Flipped Mirror", PortalSide.Exit, _flippedMirrorPortal, AssetPaths.Mirror);
			if (FlippedMirror) {
				FlippedMirror.OnInitialized += () => {
					FlippedMirror.SetPassable(true);

					Vector3 eulerAngles = FlippedMirror.transform.localEulerAngles;
					FlippedMirror.transform.localEulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, 90.0f);
				};
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
			SpawnFlippedMirror();
			
			OnPrimaryFire += () => {
				FireProjectile(PrimaryMirror, _mirrorPortal);
				UpdateLastProjectile(PortalSide.Enter);
				_animator.Play(PrimaryFireAnimHash);
			};

			OnSecondaryFire += () => {
				_animator.Play(SecondaryFireAnimHash);
				UpdateLastProjectile(PortalSide.Exit);
				FireProjectile(FlippedMirror, _flippedMirrorPortal);
			};
			
			UpdateLastProjectile(PrimaryMirror.side);
			InitMirror();
		}
		
		private void InitMirror() {
			_mirrorPortal = CreatePortal("Mirror Head", PrimaryMirror.transform, PrimaryMirror.transform, _portalSize);
			
			_flippedMirrorPortal = CreatePortal("Flipped Mirror Portal", FlippedMirror.transform, FlippedMirror.transform,
				_portalSize);
			_flippedMirrorPortal.usePerceivedGravityOnEnter = true;
			_flippedMirrorPortal.usePerceivedGravityOnExit = true;
		}

		public override bool ShouldBeReset() {
			if (!PrimaryMirror || !FlippedMirror) {
				return true;
			}

			return PrimaryMirror.ShouldBeDisabled() &&
			       FlippedMirror.ShouldBeDisabled();
		}

		public void Reset() {
			if (!PrimaryMirror || !FlippedMirror)
				return;
			
			PrimaryMirror.Reset();
			PrimaryMirror.SetPassable(false);
			PrimaryMirror.transform.position = DefaultPortalPosition;
			
			FlippedMirror.Reset();
			FlippedMirror.SetPassable(false);
			FlippedMirror.transform.position = DefaultPortalPosition;
		}
	}
}