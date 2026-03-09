using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal.Colorizers {
	public class ProjectileColorManager : MonoBehaviour {
		private const float AltParticleColorMultiplier = 0.784f;
		private const float GlowColorMultiplier = 1.2f;
		
		private const string GlowParticlesName = "Glow";
		private const string LightningParticles = "Lightning Particles";

		public bool FirstColorDone;
		
		private Color DesiredColor {
			get {
				if (side == PortalSide.Enter) {
					return ModConfig.PrimaryPortalColor.GetValue();
				}

				return ModConfig.SecondaryPortalColor.GetValue();
			}
		}
		
		private Color DesiredParticleColor {
			get {
				if (side == PortalSide.Enter) {
					return ModConfig.PrimaryPortalParticleColor.GetValue();
				}

				return ModConfig.SecondaryPortalParticleColor.GetValue();
			}
		}
		
		private Renderer _renderer;
		private ParticleSystem _lightningParticles;
		private ParticleSystem _glowParticles;
		private Light _light;

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
			
			_renderer.material.SetColor("_Color", DesiredColor);
			_renderer.material.SetColor("_EmissionColor", DesiredColor);

			if (_light) {
				_light.color = DesiredColor;
			}

			_lightningParticles.Stop();
			ParticleSystem.MainModule lightningModule = _lightningParticles.main;
			lightningModule.startColor = new ParticleSystem.MinMaxGradient(DesiredParticleColor,
				DesiredParticleColor * AltParticleColorMultiplier);
			_lightningParticles.Play();
			
			_glowParticles.Stop();
			ParticleSystem.MainModule glowModule = _glowParticles.main;
			glowModule.startColor = new ParticleSystem.MinMaxGradient(DesiredParticleColor * GlowColorMultiplier);
			_glowParticles.Play();
		}
	}
}