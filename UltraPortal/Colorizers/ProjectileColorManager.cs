using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal.Colorizers {
	public class ProjectileColorManager : MonoBehaviour {
		private const float AltParticleColorMultiplier = 0.784f;
		private const float GlowColorMultiplier = 1.2f;
		
		private const string GlowParticlesName = "Glow";
		private const string LightningParticles = "Lightning Particles";

		public bool FirstColorDone;
		
		private Renderer _renderer;
		private ParticleSystem _lightningParticles;
		private ParticleSystem _glowParticles;
		private Light _light;

		public WeaponVariant variant;
		public PortalSide side;

		private void GetParticlesInChild(string childName, out ParticleSystem system) {
			Transform particleTransform = transform.Find(childName);
			system = null;
			
			if (!particleTransform) {
				Plugin.LogSource.LogError($"{childName} not found on {name}");
				enabled = false;
				return;
			}

			system = particleTransform.GetComponent<ParticleSystem>();
			if (!system) {
				Plugin.LogSource.LogError($"No particle system on {childName}");
				enabled = false;
				return;
			}
		}

		private void Awake() {
			_renderer = GetComponent<Renderer>();
			GetParticlesInChild(LightningParticles, out _lightningParticles);
			GetParticlesInChild(GlowParticlesName, out _glowParticles);
			_light = GetComponentInChildren<Light>();
		}

		public void ColorProjectile() {
			FirstColorDone = true;

			Color desiredColor = ColorHelpers.GetPortalColor(variant, side);
			Color desiredParticleColor = ColorHelpers.GetPortalParticleColor(variant, side);
			_renderer.material.SetColor("_Color", desiredColor);
			_renderer.material.SetColor("_EmissionColor", desiredColor);

			if (_light) {
				_light.color = desiredColor;
			}

			_lightningParticles.Stop();
			ParticleSystem.MainModule lightningModule = _lightningParticles.main;
			lightningModule.startColor = new ParticleSystem.MinMaxGradient(desiredParticleColor,
				desiredParticleColor * AltParticleColorMultiplier);
			_lightningParticles.Play();
			
			_glowParticles.Stop();
			ParticleSystem.MainModule glowModule = _glowParticles.main;
			glowModule.startColor = new ParticleSystem.MinMaxGradient(desiredParticleColor * GlowColorMultiplier);
			_glowParticles.Play();
		}
	}
}