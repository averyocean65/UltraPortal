using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Controls", "Portal Gun Slot")] 
		public static KeyCode PortalGunKeybind = KeyCode.Alpha7;

		// note: i do not have a better name for this
		
		[Configgable("Gameplay")]
		public static bool UseOtherPortalForProjectileTeleport = true;
		
		[Configgable("Advanced/Projectiles", "Projectile Speed")]
		public static float PortalProjectileSpeed = 107.5f;

		[Configgable(displayName: "Enabled")]
		public static bool IsGunEnabled = true;

		// note: i also do not have a better name for these
		
		[Configgable("Advanced/Projectiles")]
		public static float ProjectileEnemyGroundPortalBoostMultiplier = 0.5f;
		
		[Configgable("Advanced/Projectiles")]
		public static float ProjectileEnemyNormalPortalBoostMultiplier = 2f;

		[Configgable("Advanced/Portals", "Minimum Entry/Exit Speed (scene reload required)")]
		public static float MinimumEntryExitSpeed = 20f;
	}
}