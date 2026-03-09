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

		public bool BothPortalsInit {
			get {
				if (!PassthroughEntry || !PassthroughExit) {
					return false;
				}

				return PassthroughEntry.transform.position.y > DefaultPortalPosition.y &&
				       PassthroughExit.transform.position.y > DefaultPortalPosition.y;
			}
		}

		private static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
		private static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire"); 
		
		private Animator _animator;

		private readonly Vector2 _portalSize = new Vector2(11f, 11f);
		private Portal _mirrorPortal;
		private Portal _passthroughPortal;
		public DynamicPortalExit PrimaryMirror { get; private set; }
		public DynamicPortalExit PassthroughEntry { get; private set; }
		public DynamicPortalExit PassthroughExit { get; private set; }
		
		private bool _firingPassthroughEntry = true;
		
		public void SpawnPrimaryMirror(bool reinit = false) {
			PrimaryMirror = SpawnPortalExit("Primary Mirror", PortalSide.Enter, _mirrorPortal, AssetPaths.Mirror);
			if (PrimaryMirror) {
				PrimaryMirror.OnInitialized += () => PrimaryMirror.SetPassable(true);
			}
			
			if (reinit) {
				InitMirror();
			}
		}
		
		public void SpawnPassthroughEntry(bool reinit = false) {
			PassthroughEntry = SpawnPortalExit("Passthrough Entry", PortalSide.Enter, _passthroughPortal, AssetPaths.Mirror);
			if (PassthroughEntry) {
				PassthroughEntry.OnInitialized += UpdatePassthroughPassable;
			}
			
			if (reinit) {
				InitMirror();
			}
		}
		
		public void SpawnPassthroughExit(bool reinit = false) {
			PassthroughExit = SpawnPortalExit("Passthrough Exit", PortalSide.Exit, _passthroughPortal, AssetPaths.Mirror);
			if (PassthroughExit) {
				PassthroughExit.OnInitialized += UpdatePassthroughPassable;
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
			SpawnPassthroughEntry();
			SpawnPassthroughExit();
			
			OnPrimaryFire += () => {
				FireProjectile(PrimaryMirror, _mirrorPortal);
				_animator.Play(PrimaryFireAnimHash);
			};

			OnSecondaryFire += () => {
				FireProjectile(_firingPassthroughEntry ? PassthroughEntry : PassthroughExit, _passthroughPortal);
				_firingPassthroughEntry = !_firingPassthroughEntry;
				
				_animator.Play(SecondaryFireAnimHash);
			};
			
			UpdateLastProjectile(PrimaryMirror.side);
			InitMirror();
		}

		private void UpdatePassthroughPassable() {
			if (PassthroughEntry)
				PassthroughEntry.SetPassable(BothPortalsInit);

			if (PassthroughExit)
				PassthroughExit.SetPassable(BothPortalsInit);
		}

		private void InitMirror() {
			_mirrorPortal = CreatePortal("Mirror Head", PrimaryMirror.transform, PrimaryMirror.transform, _portalSize);
			_passthroughPortal = CreatePortal("Passthrough Portal", PassthroughEntry.transform, PassthroughExit.transform, _portalSize);
		}

		public override bool ShouldBeReset() {
			if (!PrimaryMirror || !PassthroughEntry || !PassthroughExit) {
				return true;
			}

			return PrimaryMirror.ShouldBeDisabled() &&
			       PassthroughEntry.ShouldBeDisabled() &&
			       PassthroughExit.ShouldBeDisabled();
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