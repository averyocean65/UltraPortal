using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using Configgy;
using HarmonyLib;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;
using UnityEngine.SceneManagement;

using static UltraPortal.Constants;

namespace UltraPortal {
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin {
        private ConfigBuilder config;
        
        private static class PluginInfo {
            public const string Name = "ULTRAPORTAL";
            public const string Guid = "com.ultraportal";
            public const string Version = "0.1.0";
        }
        
        public static ManualLogSource LogSource { get; private set; }
        

        private void Awake() {
            LogSource = Logger;

            Harmony harmony = new Harmony(PluginInfo.Guid);
            harmony.PatchAll();

            config = new ConfigBuilder(PluginInfo.Guid, PluginInfo.Name);
            config.BuildAll();

            if (!Directory.Exists(AssetPaths.BundlePath)) {
                Logger.LogError($"Path for ULTRAPORTAL bundles does not exist! Looked for: {AssetPaths.BundlePath}");
                Destroy(this);
                return;
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
            PortalGunManager.EquippedPortalGun = false;
            
            if (loadMode != LoadSceneMode.Single || SceneHelper.CurrentScene == "Intro" || SceneHelper.CurrentScene == "Main Menu") {
                return;
            }

            try {
                Logger.LogInfo($"Currently playing: {SceneHelper.CurrentScene}");
                
                GameObject manager = new GameObject("Portal Gun Manager");
                manager.AddComponent<PortalGunManager>();
            }
            catch {
                Logger.LogError("Scene is not compatible! Failed to spawn portal spawner!");
            }
        }
    }
}