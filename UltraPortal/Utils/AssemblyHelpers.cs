using System;
using System.IO;
using System.Reflection;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal.Utils {
    public static class AssemblyHelpers {
        private static void PreloadAssembly(string name) {
            if (!name.EndsWith(".dll")) {
                name += ".dll";
            }
            
            string fullPath = Path.Combine(AssetPaths.AssemblyFolderPath, name);
            try {
                Assembly.LoadFrom(fullPath);
            }
            catch (Exception ex) {
                LogFatal($"Failed to load assembly: {fullPath}.\n{ex}");
            }
        }
        
        public static void PreloadAssemblies() {
            PreloadAssembly(AssetPaths.UltraPortalSharedAssembly);
        }
    }
}