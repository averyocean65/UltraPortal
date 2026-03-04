using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Controls", "Portal Gun Slot")] 
		public static KeyCode PortalGunKeybind = KeyCode.Alpha7;

		// note: i do not have a better name for this
		
		[Configgable("Gameplay")]
		public static bool UseOtherPortalForProjectileTeleport = true;

		[Configgable("Visuals")]
		public static Color PrimaryPortalColor = new Color(252, 3, 69);
		
		[Configgable("Visuals")]
		public static Color PrimaryPortalParticleColor = new Color(148, 24, 57);
		
		[Configgable("Visuals")]
		public static Color SecondaryPortalColor = new Color(46, 60, 255);
		
		[Configgable("Visuals")]
		public static Color SecondaryPortalParticleColor = new Color(26, 30, 92);
		
		[Configgable("Projectiles/Advanced", "Projectile Speed")]
		public static float PortalProjectileSpeed = 107.5f;

		[Configgable(displayName: "Enabled")]
		public static bool IsGunEnabled = true;

		// note: i also do not have a better name for these
		
		[Configgable("Projectiles/Advanced")]
		public static float ProjectileEnemyGroundPortalBoostMultiplier = 0.5f;
		
		[Configgable("Projectiles/Advanced")]
		public static float ProjectileEnemyNormalPortalBoostMultiplier = 2f;

		[Configgable("Portals/Advanced", "Minimum Entry/Exit Speed (scene reload required)")]
		public static float MinimumEntryExitSpeed = 20f;
	}
}