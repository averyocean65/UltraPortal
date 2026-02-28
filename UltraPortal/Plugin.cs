using System;
using BepInEx;
using BepInEx.Logging;
using Configgy;
using GameConsole.pcon;
using ULTRAKILL.Portal;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
            LogSource = Logger;

            config = new ConfigBuilder("com.ultraportal", "ULTRAPORTAL");
            config.BuildAll();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) {
            if (loadMode != LoadSceneMode.Single) {
                return;
            }

            try {
                if (SceneHelper.CurrentLevelNumber < 1) {
                    return;
                }
                
                HudMessageReceiver.Instance.SendHudMessage($"Loaded {SceneHelper.CurrentScene} [index: {SceneHelper.CurrentLevelNumber}]");
                GameObject spawner = new GameObject("Custom Portal Spawner (mod)");
                spawner.AddComponent<PortalSpawner>();
            }
            catch {
                Logger.LogError("Scene is not compatible!");
            }
        }
    }
}