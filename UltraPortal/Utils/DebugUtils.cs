using BepInEx.Logging;

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
    }
}