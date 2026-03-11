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

			GameObject emitter = new GameObject("Temporary Audio Source") {
				transform = {
					position = emitterPositon
				}
			};

			AudioSource source = emitter.AddComponent<AudioSource>();
			source.outputAudioMixerGroup = AudioMixerController.Instance.allGroup;
			source.volume = AudioMixerController.Instance.sfxVolume;
			source.minDistance = 1.0f;
			source.maxDistance = 10.0f;
			source.clip = clip;
			source.loop = false;
			
			source.Play();
			
			Destroy(emitter, clip.length + 0.2f);
		}
	}
}