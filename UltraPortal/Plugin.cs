using System.IO;
using AUU;
using BepInEx;
using BepInEx.Logging;
using Configgy;
using HarmonyLib;
using UltraPortal.Projectiles;
using UnityEngine;
using UnityEngine.SceneManagement;

using static UltraPortal.Constants;

namespace UltraPortal {
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("com.averyocean65.utils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("Hydraxous.ULTRAKILL.Configgy", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("ULTRAKILL.exe")]
    public class Plugin : BaseUnityPlugin {
        public static ConfigBuilder ConfiggyConfig;
        
        private static class PluginInfo {
            public const string Name = "ULTRAPORTAL";
            public const string Guid = "com.ultraportal";
            public const string Version = "0.2.2";
        }
        
        public static ManualLogSource LogSource { get; private set; }
        
        private void Awake() {
            LogSource = Logger;

            Harmony harmony = new Harmony(PluginInfo.Guid);
            harmony.PatchAll();

            ConfiggyConfig = new ConfigBuilder(PluginInfo.Guid, PluginInfo.Name);
            ConfiggyConfig.BuildAll();
            ConfiggyConfig.Rebuild();

            if (!Directory.Exists(AssetPaths.BundlePath)) {
                Logger.LogError($"Path for ULTRAPORTAL bundles does not exist! Looked for: {AssetPaths.BundlePath}; Trying other bundle path.");
                AssetPaths.UseAltBundlePath = true; // why, r2modman, why??
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            ConfiggyConfig.OnConfigElementsChanged += elements => {
                // configgy autosave no longer works, so this'll do.
                ConfiggyConfig.SaveData();
            };
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
            PortalGunManager.UsedPortalGun = false;
            PortalProjectileHelper.PortalScaleSceneStart = ModConfig.PortalScaleMod.GetValue();

            if (!SceneUtils.IsInLevel()) {
                return;
            }

            try {
                Logger.LogInfo($"Currently playing: {SceneHelper.CurrentScene}");
                
                GameObject manager = new GameObject("Portal Gun Manager");
                manager.AddComponent<PortalGunManager>();

                GameObject audioManager = new GameObject("Audio Manager");
                audioManager.AddComponent<AudioManager>();
                
                // Register styles
                StyleHUD.Instance.RegisterStyleItem(StyleSafetyHazardId, StyleSafetyHazardName);
                StyleHUD.Instance.RegisterStyleItem(StylePortalProjectileId, StylePortalProjectileName);
            }
            catch {
                Logger.LogError("Scene is not compatible! Failed to spawn portal spawner!");
            }
        }
    }
}