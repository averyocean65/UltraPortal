using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Controls", "Portal Gun Slot")] 
		public static ConfigKeybind PortalGunKeybind = new ConfigKeybind(KeyCode.Alpha7);

		// note: i do not have a better name for this
		
		[Configgable("Debugging", "Verbose Logging (may cause performance issues)")]
		public static ConfigToggle VerboseLogging = new ConfigToggle(false);

		[Configgable("Debugging", "New Gravity")]
		private static ConfigVector3 _newGravity = new ConfigVector3(Vector3.down * -40);
		
		[Configgable("Debugging", "Set Gravity")]
		private static ConfigButton _setGravity = new ConfigButton(() => {
			NewMovement.Instance.SwitchGravity(_newGravity.GetValue(), true);
		});
		
		[Configgable("Gameplay")]
		public static ConfigToggle UseOtherPortalForProjectileTeleport = new ConfigToggle(true);
		
		#region Portal Gun
		[Configgable("Visuals/Portal Gun")]
		public static ConfigColor PrimaryPortalColor = new ConfigColor(new Color(0.9882352941f, 0.0117647059f, 0.2705882353f));
		
		[Configgable("Visuals/Portal Gun/Particles")]
		public static ConfigColor PrimaryPortalParticleColor = new ConfigColor(new Color(0.5803921569f, 0.0941176471f, 0.2235294118f));
		
		[Configgable("Visuals/Portal Gun")]
		public static ConfigColor SecondaryPortalColor = new ConfigColor(new Color(0.1803921569f, 0.2352941176f, 1f));
		
		[Configgable("Visuals/Portal Gun/Particles")]
		public static ConfigColor SecondaryPortalParticleColor = new ConfigColor(new Color(0.1019607843f, 0.1176470588f, 0.3607843137f));
		#endregion
		
		#region Mirror Gun
		[Configgable("Visuals/Mirror Gun")]
		public static ConfigColor PrimaryMirrorColor = new ConfigColor(new Color(0, 1, 0.369f));
		
		[Configgable("Visuals/Mirror Gun/Particles")]
		public static ConfigColor PrimaryMirrorParticleColor = new ConfigColor(new Color(0, 0.639f, 0.235f));
		
		[Configgable("Visuals/Mirror Gun")]
		public static ConfigColor FlippedMirrorColor = new ConfigColor(new Color(1, 0.78f, 0));
		
		[Configgable("Visuals/Mirror Gun/Particles")]
		public static ConfigColor FlippedMirrorParticleColor = new ConfigColor(new Color(0.741f, 0.58f, 0));
		#endregion

		#region Twist Gun
		[Configgable("Visuals/Twist Gun")]
		public static ConfigColor PrimaryTwistColor = new ConfigColor(new Color(1, 0, 0));
		
		[Configgable("Visuals/Twist Gun/Particles")]
		public static ConfigColor PrimaryTwistParticleColor = new ConfigColor(new Color(0.8f, 0, 0));
		
		[Configgable("Visuals/Twist Gun")]
		public static ConfigColor SecondaryTwistColor = new ConfigColor(new Color(1, 1, 1));
		
		[Configgable("Visuals/Twist Gun/Particles")]
		public static ConfigColor SecondaryTwistParticleColor = new ConfigColor(new Color(0.8f, 0.8f, 0.8f));
		#endregion
		
		[Configgable("Visuals")]
		public static ConfigColor ExplosionColor = new ConfigColor(new Color(0.35686f, 0.02745f, 0.45098f));

		[Configgable("Visuals")]
		public static IntegerSlider ExplosionEmissionIntensity = new IntegerSlider(-1, -5, 5);
		
		[Configgable("Visuals/Portals", "Emission On Portal Border")]
		public static ConfigToggle UseEmission = new ConfigToggle(true);
		
		[Configgable("Visuals/Portals")]
		public static ConfigToggle ShowPortalSpawnParticles = new ConfigToggle(true);
		
		[Configgable("Visuals/Portals", "Maximum Portal Recursions", description: "Requires level restart!")]
		public static IntegerSlider MaxPortalRecursions = new IntegerSlider(3, 0, 10);
		
		[Configgable("Visuals/Style")]
		public static ConfigColor SafetyHazardColor = new ConfigColor(new Color(1, 0, 0));
		
		[Configgable("Visuals/Style")]
		public static ConfigColor ProjectileBonusColor = new ConfigColor(new Color(0, 1, 0));

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

		[Configgable("Gameplay/Portals/Experimental", "Minimum Entry/Exit Speed", description: "Requires level restart!")]
		public static ConfigInputField<float> MinimumEntryExitSpeed = new ConfigInputField<float>(20f);
		
		[Configgable("Gameplay/Portals/Experimental", "Portal Wall Offset")]
		public static ConfigInputField<float> PortalWallOffset = new ConfigInputField<float>(0.45f);
		
		[Configgable("Gameplay/Guns/Experimental", "Twist Gun Angle")]
		public static ConfigInputField<float> TwistGunAngle = new ConfigInputField<float>(90.0f);
	}
}