using System;
using UnityEngine;

namespace UltraPortal.Colorizers {
	public class ExplosionColorManager : MonoBehaviour {
		private const string LightPath = "Point Light";

		private Renderer _renderer;
		private Light _light;
		
		private void Awake() {
			_renderer = GetComponent<Renderer>();

			Transform lightTransform = transform.Find(LightPath);
			_light = lightTransform.GetComponent<Light>();
		}

		public void ColorExplosion() {
			if (_renderer) {
				float factor = 0.5f; // 2^-1
				_renderer.material.SetColor("_EmissionColor", ModConfig.ExplosionColor * factor);
			}
			else {
				Plugin.LogSource.LogWarning("Renderer was not found on explosion!");
			}

			if (_light) {
				_light.color = ModConfig.ExplosionColor;
			}
			else {
				Plugin.LogSource.LogWarning("Light was not found on explosion!");
			}
		}
	}
}