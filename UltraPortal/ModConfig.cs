using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Controls", "Portal Gun Slot")] 
		public static KeyCode PortalGunKeybind = KeyCode.Alpha7;
		
		[Configgable("Controls", "Despawn Portals")]
		public static KeyCode DespawnPortalsKeybind = KeyCode.T;

		// note: i do not have a better name for this
		
		[Configgable("Gameplay")]
		public static bool UseOtherPortalForProjectileTeleport = true;

		[Configgable("Visuals")]
		public static Color PrimaryPortalColor = new Color(0.9882352941f, 0.0117647059f, 0.2705882353f);
		
		[Configgable("Visuals")]
		public static Color PrimaryPortalParticleColor = new Color(0.5803921569f, 0.0941176471f, 0.2235294118f);
		
		[Configgable("Visuals")]
		public static Color SecondaryPortalColor = new Color(0.1803921569f, 0.2352941176f, 1f);
		
		[Configgable("Visuals")]
		public static Color SecondaryPortalParticleColor = new Color(0.1019607843f, 0.1176470588f, 0.3607843137f);

		[Configgable("Visuals")]
		public static bool UseEmission = true;
		
		[Configgable("Visuals")]
		public static bool ShowPortalSpawnParticles = true;

		[Configgable("Gameplay/Projectiles/Advanced", "Projectile Speed")]
		public static float PortalProjectileSpeed = 95.0f;

		[Configgable(displayName: "Enabled")]
		public static bool IsEnabled = true;

		// note: i also do not have a better name for these
		
		[Configgable("Gameplay/Projectiles/Advanced")]
		public static float ProjectileEnemyGroundPortalBoostMultiplier = 0.5f;
		
		[Configgable("Gameplay/Projectiles/Advanced")]
		public static float ProjectileEnemyNormalPortalBoostMultiplier = 2f;

		[Configgable("Gameplay/Portals/Advanced", "Minimum Entry/Exit Speed (scene reload required)")]
		public static float MinimumEntryExitSpeed = 20f;
		
		[Configgable("Gameplay/Portals/Advanced", "Portal Wall Offset")]
		public static float PortalWallOffset = 0.45f;
	}
}