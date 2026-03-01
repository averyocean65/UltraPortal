using System.IO;
using BepInEx;
using BepInEx.Logging;
using Configgy;
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
            public const string Version = "0.0.1";
        }
        
        public static ManualLogSource LogSource { get; private set; }
        

        private void Awake() {
            LogSource = Logger;

            config = new ConfigBuilder("com.ultraportal", "ULTRAPORTAL");
            config.BuildAll();

            if (!Directory.Exists(AssetPaths.BundlePath)) {
                Logger.LogError($"Path for ULTRAPORTAL bundles does not exist! Looked for: {AssetPaths.BundlePath}");
                Destroy(this);
                return;
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
            if (loadMode != LoadSceneMode.Single) {
                return;
            }

            try {
                GameObject spawner = new GameObject("Custom Portal Spawner (mod)");
                spawner.AddComponent<PortalSpawner>();
            }
            catch {
                Logger.LogError("Scene is not compatible! Failed to spawn portal spawner!");
            }
        }
    }
}