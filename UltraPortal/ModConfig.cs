using AUU;
using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		private const string RequiresLevelReload = "Requires restart of current level!";
		
		[Configgable("Controls", "Portal Gun Slot")] 
		public static ConfigKeybind PortalGunKeybind = new ConfigKeybind(KeyCode.Alpha7);
		
		[Configgable("Controls", "Close Portals With Mouse", description: "By default, you can close portals by pressing the primary and alternative fire at once, toggling this option off changes it to a keybind on your keyboard.")] 
		public static ConfigToggle CloseWithMouse = new ConfigToggle(true);
		
		[Configgable("Controls")]
		public static ConfigKeybind AltCloseKeybind = new ConfigKeybind(KeyCode.None); 

		// note: i do not have a better name for this
		
		[Configgable("Debugging", "Verbose Logging", description: "Toggling this option allows the mod to log extra, often excessive, information to the BepInEx console. It may lead to severe performance issues. So only toggle it if you're debugging!")]
		public static ConfigToggle VerboseLogging = new ConfigToggle(false);

		[Configgable("Debugging", "Unload Asset Bundles",
			description: "Only use if you intend to swap out asset bundles in real time!")]
		public static ConfigButton UnloadAssetBundles = new ConfigButton(AssetBundleUtils.UnloadAllAssetBundles);
    
		[Configgable("Debugging", "New Gravity")]
		private static ConfigVector3 _newGravity = new ConfigVector3(Vector3.down * -40);

		[Configgable("Debugging", "Draw Debug Arrows")]
		public static ConfigToggle DrawDebugObjects = new ConfigToggle(false);
		
		[Configgable("Debugging", "Set Gravity")]
		private static ConfigButton _setGravity = new ConfigButton(() => {
			PortalGunManager.UsedPortalGun = true; // you manipulated gravity, cheater!!
			NewMovement.Instance.SwitchGravity(_newGravity.GetValue(), true);
		});
		
		[Configgable("Gameplay")]
		public static ConfigToggle UseOtherPortalForProjectileTeleport = new ConfigToggle(true);
		
		#region Portal Gun
		[Configgable("Visuals/Guns/Portal Gun")]
		public static ConfigColor PrimaryPortalColor = new ConfigColor(new Color(0.9882352941f, 0.0117647059f, 0.2705882353f));
		
		[Configgable("Visuals/Guns/Portal Gun/Particles")]
		public static ConfigColor PrimaryPortalParticleColor = new ConfigColor(new Color(0.5803921569f, 0.0941176471f, 0.2235294118f));
		
		[Configgable("Visuals/Guns/Portal Gun")]
		public static ConfigColor SecondaryPortalColor = new ConfigColor(new Color(0.1803921569f, 0.2352941176f, 1f));
		
		[Configgable("Visuals/Guns/Portal Gun/Particles")]
		public static ConfigColor SecondaryPortalParticleColor = new ConfigColor(new Color(0.1019607843f, 0.1176470588f, 0.3607843137f));
		#endregion
		
		#region Mirror Gun
		[Configgable("Visuals/Guns/Mirror Gun")]
		public static ConfigColor PrimaryMirrorColor = new ConfigColor(new Color(0, 1, 0.369f));
		
		[Configgable("Visuals/Guns/Mirror Gun/Particles")]
		public static ConfigColor PrimaryMirrorParticleColor = new ConfigColor(new Color(0, 0.639f, 0.235f));
		
		[Configgable("Visuals/Guns/Mirror Gun")]
		public static ConfigColor FlippedMirrorColor = new ConfigColor(new Color(1, 0.78f, 0));
		
		[Configgable("Visuals/Guns/Mirror Gun/Particles")]
		public static ConfigColor FlippedMirrorParticleColor = new ConfigColor(new Color(0.741f, 0.58f, 0));
		#endregion

		#region Twist Gun
		[Configgable("Visuals/Guns/Twist Gun")]
		public static ConfigColor PrimaryTwistColor = new ConfigColor(new Color(1, 0, 0));
		
		[Configgable("Visuals/Guns/Twist Gun/Particles")]
		public static ConfigColor PrimaryTwistParticleColor = new ConfigColor(new Color(0.8f, 0, 0));
		
		[Configgable("Visuals/Guns/Twist Gun")]
		public static ConfigColor SecondaryTwistColor = new ConfigColor(new Color(1, 1, 1));
		
		[Configgable("Visuals/Guns/Twist Gun/Particles")]
		public static ConfigColor SecondaryTwistParticleColor = new ConfigColor(new Color(0.8f, 0.8f, 0.8f));
		#endregion
		
		[Configgable("Visuals")]
		public static ConfigColor ExplosionColor = new ConfigColor(new Color(0.35686f, 0.02745f, 0.45098f));

		[Configgable("Visuals")]
		public static IntegerSlider ExplosionEmissionIntensity = new IntegerSlider(-1, -5, 5);
		
		[Configgable("Visuals/Portals", orderInList: 1)]
		public static ConfigToggle CanSeePortalBorders = new ConfigToggle(true);
		
		[Configgable("Visuals/Portals", "Emission On Portal Border")]
		public static ConfigToggle UseEmission = new ConfigToggle(true);
		
		[Configgable("Visuals/Portals")]
		public static ConfigToggle ShowPortalSpawnParticles = new ConfigToggle(true);
		
		[Configgable("Visuals/Portals")]
		public static ConfigToggle ShowPortalAmbianceParticles = new ConfigToggle(true);

		[Configgable("Visuals/Portals", "Infinite Portal Recursions?", description: RequiresLevelReload)]
		public static ConfigToggle InfiniteRecursions = new ConfigToggle(false);
		
		[Configgable("Visuals/Portals", "Maximum Portal Recursions", description: RequiresLevelReload)]
		public static IntegerSlider MaxPortalRecursions = new IntegerSlider(3, 0, 10);
		
		[Configgable("Visuals/Style")]
		public static ConfigColor SafetyHazardColor = new ConfigColor(new Color(1, 0, 0));
		
		[Configgable("Visuals/Style")]
		public static ConfigColor ProjectileBonusColor = new ConfigColor(new Color(0, 1, 0));
		
		[Configgable("Gameplay/Guns/Experimental", "Projectile Speed")]
		public static ConfigInputField<float> PortalProjectileSpeed = new ConfigInputField<float>(95.0f);

		[Configgable("Gameplay/Guns")]
		public static ConfigToggle UseBeamForProjectiles = new ConfigToggle(false);
		
		[Configgable("Gameplay")]
		public static ConfigToggle AreExplosionsUltraboosters = new ConfigToggle(false);

		[Configgable(displayName: "Enabled")]
		public static ConfigToggle IsEnabled = new ConfigToggle(true);

		// note: i also do not have a better name for these

		[Configgable("Gameplay/Portals", "Portal Scale Modifier", description: RequiresLevelReload)]
		public static ConfigInputField<float> PortalScaleMod = new ConfigInputField<float>(1.0f);
		
		[Configgable("Gameplay/Guns/Experimental")]
		public static ConfigInputField<float> ProjectileEnemyGroundPortalBoostMultiplier = new ConfigInputField<float>(0.5f);
		
		[Configgable("Gameplay/Guns/Experimental")]
		public static ConfigInputField<float> ProjectileEnemyNormalPortalBoostMultiplier = new ConfigInputField<float>(2f);

		[Configgable("Gameplay/Portals/Experimental", "Minimum Entry/Exit Speed", description: RequiresLevelReload)]
		public static ConfigInputField<float> MinimumEntryExitSpeed = new ConfigInputField<float>(20f);
		
		[Configgable("Gameplay/Portals/Experimental", description: "The required dot product threshold for a portal to be determined perpendicular to a collider.")]
		public static ConfigInputField<float> PerpendicularThreshold = new ConfigInputField<float>(0.01f);
		
		[Configgable("Gameplay/Portals/Experimental", description: "An assisted portal is a portal with special checks to ensure functionality for ground portals.")]
		public static ConfigInputField<float> AssistedPortalThreshold = new ConfigInputField<float>(0.6f);
		
		[Configgable("Gameplay/Portals/Experimental", description: "The size of a sphere, which checks colliders around a portal")]
		public static ConfigInputField<float> PortalSphereCheckRadius = new ConfigInputField<float>(1.5f);
		
		[Configgable("Gameplay/Portals/Experimental", "Portal Wall Offset")]
		public static ConfigInputField<float> PortalWallOffset = new ConfigInputField<float>(0.5f);
		
		[Configgable("Gameplay/Portals/Experimental", "Enemy Velocity Threshold", description: "The threshold an enemy's velocity has to cross for the portals to disable the associated rigidbody. To be fully transparent: this is a temporary solution to make +TERMINAL VELOCITY function reliably for this update. We are working on fixing it in future versions of ULTRAPORTAL.")]
		public static ConfigInputField<float> EnemyMaxVelocity = new ConfigInputField<float>(80.0f);

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