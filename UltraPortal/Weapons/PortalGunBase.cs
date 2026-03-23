using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
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

		protected virtual void OnEnable() {
			if (LastProjectileColors && LastProjectileColors.FirstColorDone) {
				LastProjectileColors.ColorProjectile();
			}
		}

		protected DynamicPortalExit SpawnPortalExit(string objectName, PortalSide side, Portal hostPortal,
			string prefabName = AssetPaths.PortalExit) {
			AssetBundle portals = AssetBundleHelpers.LoadAssetBundle(AssetPaths.PortalBundle);
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

		private bool triggeredPortalReset = false;
		protected virtual void FireProjectile(DynamicPortalExit exit, Portal portal) {
			Projectile projectile = SpawnProjectileFromAsset(AssetPaths.Projectile, ModConfig.PortalProjectileSpeed.GetValue());
			PortalProjectileHelper helper = projectile.gameObject.AddComponent<PortalProjectileHelper>();
			helper.exit = exit;
			helper.portal = portal;

			ProjectileColorManager colorManager = projectile.gameObject.AddComponent<ProjectileColorManager>();
			colorManager.side = exit.side;
			colorManager.variant = variant;
			colorManager.ColorProjectile();
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
					_animator.Play(_info.CloseAnimation);
					triggeredPortalReset = true;
				}
				
				return;
			}
			else {
				triggeredPortalReset = false;
				WantsToReset = false;
			}
			
			HandleFiring();
		}

		protected virtual Portal CreatePortal(string objectName, Transform entry, Transform exit, Vector2 size) {
			GameObject portalObject = new GameObject(objectName) {
				layer = PortalLayer
			};

			Portal portal = portalObject.AddComponent<Portal>();
			portal.allowCameraTraversals = true;
			portal.appearsInRecursions = true;
			portal.maxRecursions = ModConfig.MaxPortalRecursions.GetValue();
			portal.supportInfiniteRecursion = ModConfig.InfiniteRecursions.GetValue();
			
			portal.canSeeItself = true;
			portal.canSeePortalLayer = true;

			portal.consumeAudio = false;
			portal.canHearAudio = false;
			
			portal.entry = entry;
			portal.minimumEntrySideSpeed = ModConfig.MinimumEntryExitSpeed.GetValue();
			
			portal.exit = exit;
			portal.minimumExitSideSpeed = ModConfig.MinimumEntryExitSpeed.GetValue();

			portal.shape = new PlaneShape {
				width = size.x,
				height = size.y
			};

			return portal;
		}
	}
}