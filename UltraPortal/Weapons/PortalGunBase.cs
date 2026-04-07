using AUU;
using AUU.Portals;
using ULTRAKILL.Portal;
using UltraPortal.Colorizers;
using UltraPortal.Projectiles;
using UltraPortal.Shared;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public abstract class PortalGunBase : GunBase {
		public static readonly Vector3 DefaultPortalPosition = new Vector3(0, -1e6f, 0);
		protected PortalGunInfo _info;
        protected Animator _animator;

		public bool WantsToReset { get; protected set; }

		protected ProjectileColorManager LastProjectileColors;
		
		protected override void Start() {
			base.Start();

			_info = GetComponent<PortalGunInfo>();
			LastProjectileColors = _info.lastProjectile.AddComponent<ProjectileColorManager>();
			
			OnPrimaryFire += UpdateUsedPortalGun;
			OnSecondaryFire += UpdateUsedPortalGun;

			_animator = _info.animator;
		}

		private void UpdateUsedPortalGun() {
			PortalGunManager.UsedPortalGun = true;
		}

		protected virtual void UpdateLastProjectile(PortalSide side) {
			if (!LastProjectileColors) {
				return;
			}

			LastProjectileColors.side = side;
			LastProjectileColors.variant = variant;
			LastProjectileColors.ColorProjectile();
		}

		public abstract bool ShouldBeReset();
		public abstract bool ShouldPlayReset();

		protected virtual void OnEnable() {
			if (LastProjectileColors && LastProjectileColors.FirstColorDone) {
				LastProjectileColors.ColorProjectile();
			}
		}

		protected DynamicPortalExit SpawnPortalExit(string objectName, PortalSide side, Portal hostPortal,
			string prefabName = AssetPaths.PortalExit) {
			AssetBundle portals = AssetBundleUtils.LoadAssetBundle(AssetPaths.BundlePath, AssetPaths.PortalBundle);
			GameObject portalPrefab = portals.LoadAsset<GameObject>(prefabName);
			
			if (!portalPrefab) {
				Plugin.LogSource.LogError("Failed to load portal prefab!");
				return null;
			}
			
			Vector3 spawnPos = DefaultPortalPosition;
			
			GameObject portalEntryObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			
			portalEntryObject.name = objectName;
			DynamicPortalExit exit = portalEntryObject.AddComponent<DynamicPortalExit>();
			exit.side = side;
			exit.hostPortal = hostPortal;
			exit.hostGun = this;

			return exit;
		}

		protected void FirePhysicsProjectile(DynamicPortalExit exit, Portal portal) {
			Projectile projectile = SpawnProjectileFromAsset(AssetPaths.Projectile, ModConfig.PortalProjectileSpeed.GetValue());
			PortalProjectileHelper helper = projectile.gameObject.AddComponent<PortalProjectileHelper>();
			helper.exit = exit;
			helper.portal = portal;
			
			ProjectileColorManager colorManager = projectile.gameObject.AddComponent<ProjectileColorManager>();
			colorManager.side = exit.side;
			colorManager.variant = variant;
			colorManager.ColorProjectile();
		}

		protected void FireBeamProjectile(DynamicPortalExit exit, Portal portal) {
			bool success = PortalPhysicsV2.Raycast(MainCamera.transform.position, MainCamera.transform.forward,
				out PhysicsCastResult result, Mathf.Infinity,
				EnvironmentLayer, QueryTriggerInteraction.Ignore);

			if (!success) {
				return;
			}

			PortalGunManager.SummonPortalExit(exit, portal, result.point, -result.normal, result.transform,
				result.collider);
		}

		private bool triggeredPortalReset = false;
		protected virtual void FireProjectile(DynamicPortalExit exit, Portal portal) {
			if (ModConfig.UseBeamForProjectiles.GetValue()) {
				FireBeamProjectile(exit, portal);
				return;
			}
			
			FirePhysicsProjectile(exit, portal);
		}

		protected virtual void Update() {
			if (OptionsManager.Instance.paused) {
				return;
			}

			bool wantsToClose = ModConfig.CloseWithMouse.GetValue()
				? (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed &&
				   MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed)
				: Input.GetKeyDown(ModConfig.AltCloseKeybind.GetValue());
				
				
			if (wantsToClose) {
				WantsToReset = !triggeredPortalReset;
				if (!triggeredPortalReset) {
					triggeredPortalReset = true;

					if (ShouldPlayReset()) {
						_animator.Play(_info.CloseAnimation);
					}
				}
				
				return;
			}
	
			triggeredPortalReset = false;
			WantsToReset = false;

			HandleFiring();
		}

		protected Portal CreatePortal(string objectName, Transform entry, Transform exit, Vector2 size) {
			return PortalSpawner.CreatePortal(objectName, entry, exit, new PortalParameters() {
				AllowCameraTraversals = true,
				AppearsInRecursions = true,
				MaxRecursionCount = ModConfig.MaxPortalRecursions.GetValue(),
				InfiniteRecursions = ModConfig.InfiniteRecursions.GetValue(),
				
				CanSeeItself = true,
				CanSeePortals = true,
				
				ConsumeAudio = false,
				CanHearAudio = false,
				
				MinimumEntrySpeed = ModConfig.MinimumEntryExitSpeed.GetValue(),
				MinimumExitSpeed = ModConfig.MinimumEntryExitSpeed.GetValue(),
				
				Size = size
			});
		}
	}
}