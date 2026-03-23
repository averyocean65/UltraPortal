using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public class AudioManager : MonoSingleton<AudioManager> {
		public AudioSource PlayAudioFromAsset(string assetName, Vector3 emitterPositon, bool loop = false,
			float minDistance = 1.0f, float maxDistance = 100.0f, float spatialBlend = 1.0f) {
			AssetBundle bundle = AssetBundleHelpers.LoadAssetBundle(AssetPaths.Sfx.BundleName);
			AudioClip clip = bundle.LoadAsset<AudioClip>(assetName);

			if (!clip) {
				return null;
			}

			GameObject emitter = new GameObject($"Temporary Audio Source ({assetName})") {
				transform = {
					position = emitterPositon
				}
			};

			AudioSource source = emitter.AddComponent<AudioSource>();
			source.outputAudioMixerGroup = AudioMixerController.Instance.goreGroup;
			source.volume = AudioMixerController.Instance.sfxVolume;

			source.minDistance = minDistance;
			source.maxDistance = maxDistance;
			source.rolloffMode = AudioRolloffMode.Linear;

			source.spatialBlend = spatialBlend;

			source.clip = clip;
			source.loop = loop;

			source.Play(tracked: true);

			if (!loop) {
				Destroy(emitter, clip.length + 0.2f);
			}

			return source;
		}
	}
}