using System;
using BepInEx;
using UnityEngine.SceneManagement;

namespace UltraPortal {
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin {
        private static class PluginInfo {
            public const string Name = "ULTRAPORTAL";
            public const string Guid = "com.ultraportal";
            public const string Version = "0.0.1";
        }

        private void Awake() {
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            }
            catch {
                Logger.LogError("Scene is not compatible!");
                return;
            }
        }
    }
}