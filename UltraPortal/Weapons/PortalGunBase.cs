using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UltraPortal.Colorizers;
using UltraPortal.Projectiles;
using UnityEngine;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal {
	public abstract class PortalGunBase : GunBase {
		public static readonly Vector3 DefaultPortalPosition = new Vector3(0, -1e6f, 0);
		
		protected static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
        protected static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire");
        protected Animator _animator;
		
		// funny typo
		private const string LastProjectileVisual = "UltraPortalGun/PoralGunRig/RootPortal/Last Projectile";

		public bool WantsToReset { get; protected set; }

		protected ProjectileColorManager LastProjectileColors;
		
		protected override void Start() {
			base.Start();
			
			Transform projectilePreviewTransform = transform.Find(LastProjectileVisual);
			if (projectilePreviewTransform) {
				LastProjectileColors = projectilePreviewTransform.gameObject.AddComponent<ProjectileColorManager>();
			}
			else {
				Plugin.LogSource.LogWarning($"{LastProjectileVisual} is not present on portal gun!");
			}

			OnPrimaryFire += UpdateUsedPortalGun;
			OnSecondaryFire += UpdateUsedPortalGun;
			
			_animator = GetComponentInChildren<Animator>();
			if (!_animator) {
				LogError("Animator wasn't found in portal gun children!");
			}
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

			if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed &&
			    MonoSingleton<InputManager>.Instance.InputSource.Fire2.IsPressed) {
				WantsToReset = !triggeredPortalReset;
				if (!triggeredPortalReset) {
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