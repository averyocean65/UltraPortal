using BepInEx.Logging;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
    public static class DebugUtils {
        private static ManualLogSource Logger => Plugin.LogSource;

        public static void Log(LogLevel level, string message) {
            Logger.Log(level, message);
        }

        public static void LogInfo(string message) => Log(LogLevel.Info, message);
        public static void LogWarning(string message) => Log(LogLevel.Warning, message);
        public static void LogError(string message) => Log(LogLevel.Error, message);
        public static void LogFatal(string message) => Log(LogLevel.Fatal, message);
        
        public static void LogVerbose(LogLevel level, string message) {
            if (!ModConfig.VerboseLogging.GetValue()) {
                return;
            }
            
            Log(level, message);
        }

        public static void LogVerboseInfo(string message) => LogVerbose(LogLevel.Info, message);
        public static void LogVerboseWarning(string message) => LogVerbose(LogLevel.Warning, message);
        public static void LogVerboseError(string message) => LogVerbose(LogLevel.Error, message);
        public static void LogVerboseFatal(string message) => LogVerbose(LogLevel.Fatal, message);

        public static void ShowForwardArrow(Vector3 position, Vector3 forward, float duration = -1.0f) {
            if (!ModConfig.DrawDebugObjects.GetValue()) {
                return;
            }
            
            AssetBundle debug = AssetBundleHelpers.LoadAssetBundle(AssetPaths.DebugBundle);
            GameObject arrowPrefab = debug.LoadAsset<GameObject>(AssetPaths.DebugForwardArrow);
            
            GameObject arrow = Object.Instantiate(arrowPrefab, position, Quaternion.identity);
            arrow.transform.forward = forward;

            if (duration < 0.0f) {
                return;
            }

            Object.Destroy(arrow, duration);
        }
    }
}