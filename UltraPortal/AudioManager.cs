using System.Collections.Generic;
using UltraPortal.Extensions;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public class AudioManager : MonoSingleton<AudioManager> {
		public AudioSource PlayAudioFromAsset(string assetName, Vector3 emitterPositon, bool loop = false) {
			AssetBundle bundle = AssetBundleHelpers.LoadAssetBundle(AssetPaths.Sfx.BundleName);
			AudioClip clip = bundle.LoadAsset<AudioClip>(assetName);

			if (!clip) {
				return null;
			}

			GameObject emitter = new GameObject("Temporary Audio Source") {
				transform = {
					position = emitterPositon
				}
			};

			AudioSource source = emitter.AddComponent<AudioSource>();
			source.outputAudioMixerGroup = AudioMixerController.Instance.goreGroup;
			source.volume = AudioMixerController.Instance.sfxVolume;
			source.dopplerLevel = 0.0f;
			source.minDistance = 1.0f;
			source.maxDistance = 100.0f;
			
			source.clip = clip;
			source.loop = loop;
			
			source.Play();

			if (!loop) {
				Destroy(emitter, clip.length + 0.2f);
			}

			return source;
		}
	}
}