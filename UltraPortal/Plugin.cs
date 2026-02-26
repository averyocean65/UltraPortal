using BepInEx;
using ULTRAKILL.Portal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraPortal {
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin {
        private static class PluginInfo {
            public const string Name = "ULTRAPORTAL";
            public const string Guid = "com.ultraportal";
            public const string Version = "0.0.1";
        }

        private bool isInLevel = false;

        private void Awake() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update() {
            if (!isInLevel) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                Transform mainCam = Camera.main.transform;

                if (Physics.Raycast(mainCam.position, mainCam.forward, out var hit,  100, LayerMask.NameToLayer("Default"))) {
                    SpawnMirror(hit.point, hit.normal);
                }
            }
        }

        private void SpawnMirror(Vector3 position, Vector3 forward) {
            HudMessageReceiver.Instance.SendHudMessage($"Spawning portal at {position}, facing {forward}");
            
            GameObject mirrorObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            mirrorObject.transform.position = position;
            mirrorObject.transform.forward = forward;
            
            Portal portal = mirrorObject.AddComponent<Portal>();

            portal.mirror = true;
            portal.supportInfiniteRecursion = false;
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

            // scene test complete
            isInLevel = true;
        }
    }
}