using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal {
	public static class AssetBundleHelpers {
		private static Dictionary<string, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>();

		private static AssetBundle LoadAssetBundleFromFile(string path) {
			LogVerboseInfo($"Loading fresh asset bundle: {path}");
			
			AssetBundle bundle = AssetBundle.LoadFromFile(path);
			if (bundle == null) {
				string errorMessage = $"Failed to load Asset Bundle: {path}";
				HudMessageReceiver.Instance.SendHudMessage($"<color=red>{errorMessage}</color>");
				Plugin.LogSource.LogError(errorMessage);
				return null;
			}

			return bundle;
		}
		
		public static AssetBundle LoadAssetBundle(string bundleName) {
			string path = Path.Combine(AssetPaths.BundlePath, bundleName);
			if (!LoadedBundles.ContainsKey(bundleName)) {
				AssetBundle bundle = LoadAssetBundleFromFile(path);
				LoadedBundles.Add(bundleName, bundle);
			}

			return LoadedBundles[bundleName];
		}

		public static void UnloadAssetBundle(string key, bool removeFromList = true) {
			if (!LoadedBundles.ContainsKey(key)) {
				return;
			}
			
			LogVerboseInfo($"Unloading asset bundle: {key}");

			AssetBundle b = LoadedBundles[key];
			b.Unload(false);

			if (removeFromList) {
				LoadedBundles.Remove(key);
			}
		}
		
		public static void UnloadAllAssetBundles() {
			foreach (var kvp in LoadedBundles) {
				UnloadAssetBundle(kvp.Key, false);
			}
			
			LoadedBundles.Clear();
		}
	}
}