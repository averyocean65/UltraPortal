using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Keybinds", "Portal Gun Slot")] 
		public static KeyCode PortalGunKeybind = KeyCode.Alpha7;

		[Configgable(displayName: "Projectile Speed")]
		public static float PortalProjectileSpeed = 90f;

		[Configgable(displayName: "Enabled")]
		public static bool IsGunEnabled = true;

		// note: i do not have a better name for this
		
		[Configgable("Advanced/Projectiles")]
		public static float ProjectileEnemyGroundPortalBoostMultiplier = 0.5f;
		
		[Configgable("Advanced/Projectiles")]
		public static float ProjectileEnemyNormalPortalBoostMultiplier = 2f;

		[Configgable("Advanced/Portals", "Minimum Entry/Exit Speed")]
		public static float MinimumEntryExitSpeed = 20f;
	}
}