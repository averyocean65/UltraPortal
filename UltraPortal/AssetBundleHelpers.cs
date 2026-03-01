using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public static class AssetBundleHelpers {
		private static Dictionary<string, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>();
		
		public static AssetBundle LoadAssetBundle(string bundleName) {
			string path = Path.Combine(AssetPaths.BundlePath, bundleName);

			if (!LoadedBundles.ContainsKey(bundleName)) {
				AssetBundle bundle = AssetBundle.LoadFromFile(path);
				if (bundle == null) {
					string errorMessage = $"Failed to load Asset Bundle: {bundleName}";
					HudMessageReceiver.Instance.SendHudMessage($"<color=red>{errorMessage}</color>");
					Plugin.LogSource.LogError(errorMessage);
					return null;
				}
				
				LoadedBundles.Add(bundleName, bundle);
			}

			return LoadedBundles[bundleName];
		}
	}
}