using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Controls", "Portal Gun Slot")] 
		public static ConfigKeybind PortalGunKeybind = new ConfigKeybind(KeyCode.Alpha7);

		// note: i do not have a better name for this
		
		[Configgable("Debugging", "Verbose Logging (may cause performance issues)")]
		public static ConfigToggle VerboseLogging = new ConfigToggle(true);
		
		[Configgable("Gameplay")]
		public static ConfigToggle UseOtherPortalForProjectileTeleport = new ConfigToggle(true);

		[Configgable("Visuals")]
		public static ConfigColor PrimaryPortalColor = new ConfigColor(new Color(0.9882352941f, 0.0117647059f, 0.2705882353f));
		
		[Configgable("Visuals")]
		public static ConfigColor PrimaryPortalParticleColor = new ConfigColor(new Color(0.5803921569f, 0.0941176471f, 0.2235294118f));
		
		[Configgable("Visuals")]
		public static ConfigColor SecondaryPortalColor = new ConfigColor(new Color(0.1803921569f, 0.2352941176f, 1f));
		
		[Configgable("Visuals")]
		public static ConfigColor SecondaryPortalParticleColor = new ConfigColor(new Color(0.1019607843f, 0.1176470588f, 0.3607843137f));

		[Configgable("Visuals")]
		public static ConfigColor ExplosionColor = new ConfigColor(new Color(0.35686f, 0.02745f, 0.45098f));

		[Configgable("Visuals")]
		public static IntegerSlider ExplosionEmissionIntensity = new IntegerSlider(-1, -5, 5);
		
		[Configgable("Visuals")]
		public static ConfigToggle UseEmission = new ConfigToggle(true);
		
		[Configgable("Visuals")]
		public static ConfigToggle ShowPortalSpawnParticles = new ConfigToggle(true);
		
		[Configgable("Visuals/Style")]
		public static ConfigColor SafetyHazardColor = new ConfigColor(new Color(1, 0, 0));

		[Configgable]
		public static ConfigToggle UsePortalBorders = new ConfigToggle(true);
		
		[Configgable("Gameplay/Projectiles/Experimental", "Projectile Speed")]
		public static ConfigInputField<float> PortalProjectileSpeed = new ConfigInputField<float>(95.0f);

		[Configgable("Gameplay/Explosions")]
		public static ConfigToggle AreExplosionsUltraboosters = new ConfigToggle(false);

		[Configgable(displayName: "Enabled")]
		public static ConfigToggle IsEnabled = new ConfigToggle(true);

		// note: i also do not have a better name for these
		
		[Configgable("Gameplay/Projectiles/Experimental")]
		public static ConfigInputField<float> ProjectileEnemyGroundPortalBoostMultiplier = new ConfigInputField<float>(0.5f);
		
		[Configgable("Gameplay/Projectiles/Experimental")]
		public static ConfigInputField<float> ProjectileEnemyNormalPortalBoostMultiplier = new ConfigInputField<float>(2f);

		[Configgable("Gameplay/Portals/Experimental", "Minimum Entry/Exit Speed (scene reload required)")]
		public static ConfigInputField<float> MinimumEntryExitSpeed = new ConfigInputField<float>(20f);
		
		[Configgable("Gameplay/Portals/Experimental", "Portal Wall Offset")]
		public static ConfigInputField<float> PortalWallOffset = new ConfigInputField<float>(0.45f);

		[Configgable("Audio", "Hear Ambiance SFX")]
		public static ConfigToggle CanHearAmbiance = new ConfigToggle(true);
		
		[Configgable("Audio", "Hear Portal SFX", description: "This means sound effects such as portals opening and closing.")]
		public static ConfigToggle CanHearSFX = new ConfigToggle(true);
		
		[Configgable("Audio", "Portal Ambiance Minimum Distance", description: "The radius from the portal where the ambiance is loudest.")]
		public static ConfigInputField<float> PortalAmbianceMinDistance = new ConfigInputField<float>(10.0f);
		
		[Configgable("Audio", "Portal Ambiance Maximum Distance", description: "The radius from the portal where the ambiance is no longer audible.")]
		public static ConfigInputField<float> PortalAmbianceMaxDistance = new ConfigInputField<float>(30.0f);
	}
}