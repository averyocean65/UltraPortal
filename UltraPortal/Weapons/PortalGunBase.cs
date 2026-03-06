using System;
using ULTRAKILL.Portal;
using UltraPortal.Colorizers;
using UltraPortal.Projectiles;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public abstract class PortalGunBase : GunBase {
		// funny typo
		private const string LastProjectileVisual = "UltraPortalGun/PoralGunRig/RootPortal/Last Projectile";

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
		}
		
		protected virtual void UpdateLastProjectile(PortalSide side) {
			if (!LastProjectileColors) {
				return;
			}

			LastProjectileColors.side = side;
			LastProjectileColors.ColorProjectile();
		}

		public abstract bool ShouldBeReset();

		protected virtual void OnEnable() {
			if (LastProjectileColors && LastProjectileColors.FirstColorDone) {
				LastProjectileColors.ColorProjectile();
			}
		}

		protected virtual void FireProjectile(DynamicPortalExit exit, Portal portal) {
			Projectile projectile = SpawnProjectileFromAsset(AssetPaths.Projectile, ModConfig.PortalProjectileSpeed);
			PortalProjectileHelper helper = projectile.gameObject.AddComponent<PortalProjectileHelper>();
			helper.exit = exit;
			helper.portal = portal;

			ProjectileColorManager colorManager = projectile.gameObject.AddComponent<ProjectileColorManager>();
			colorManager.side = exit.side;
			colorManager.ColorProjectile();
		}
	}
}