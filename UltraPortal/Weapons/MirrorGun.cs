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

			PassthroughEntry.OnInitialized += () => {
				void Error() {
					HudMessageReceiver.Instance.SendHudMessage("<color=red>Failed to spawn passthrough exit!</color>");
					PassthroughEntry.Reset();
					PassthroughExit.Reset();
				}

				if (Mathf.Approximately(PassthroughEntry.transform.position.y, DefaultPortalPosition.y)) {
					Error();
					return;
				}

				Vector3 forwardPos = PassthroughEntry.transform.position + PassthroughEntry.transform.forward * 5.0f;
				bool success = Physics.Raycast(forwardPos, -PassthroughEntry.transform.forward, out var hit, 5.5f,
					EnvironmentLayer, QueryTriggerInteraction.Ignore);

				if (!hit.collider || !success) {
					Error();
					return;
				}

				bool isInRoom = Physics.Raycast(hit.point, hit.normal, out var info, 100f, EnvironmentLayer,
					QueryTriggerInteraction.Ignore);

				if (!isInRoom) {
					Error();
					return;
				}

				PassthroughExit.Initialize(_passthroughPortal, PortalSide.Exit, hit);
			};
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
				void Error() {
					HudMessageReceiver.Instance.SendHudMessage("<color=red>Failed to spawn passthrough portal!</color>");
				}
				
				_animator.Play(SecondaryFireAnimHash);
				FireProjectile(PassthroughEntry, _passthroughPortal);
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
			if (!PrimaryMirror || !PassthroughEntry || !PassthroughExit)
				return;
			
			PrimaryMirror.Reset();
			PrimaryMirror.SetPassable(false);
			PrimaryMirror.transform.position = DefaultPortalPosition;
			
			PassthroughEntry.Reset();
			PassthroughEntry.SetPassable(false);
			PassthroughEntry.transform.position = DefaultPortalPosition;
			
			PassthroughExit.Reset();
			PassthroughExit.SetPassable(false);
			PassthroughExit.transform.position = DefaultPortalPosition;
		}
	}
}