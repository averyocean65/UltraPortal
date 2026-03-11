using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public class AudioManager : MonoSingleton<AudioManager> {
		public void PlayAudioFromAsset(string assetName, Vector3 emitterPositon) {
			AssetBundle bundle = AssetBundleHelpers.LoadAssetBundle(AssetPaths.Sfx.BundleName);
			AudioClip clip = bundle.LoadAsset<AudioClip>(assetName);

			if (!clip) {
				return;
			}
			
			// TODO: play sfx 
		}
	}
}