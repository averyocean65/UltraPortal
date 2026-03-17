using System;
using ULTRAKILL.Portal;
using UnityEngine;

using static UltraPortal.DebugUtils;

namespace UltraPortal.Colorizers {
    public class PortalColorManager : MonoBehaviour {
        private const string VisualsPath = "Visuals";
        private const string AmbianceParticlesPath = "Portal Ambiance Particles";
        public DynamicPortalExit associated;

        private Renderer[] _renderers;
        private ParticleSystem _ambiancePartiles;
        
        private void Start() {
            Transform visualsRoot = transform.Find(VisualsPath);
            _ambiancePartiles = transform.Find(AmbianceParticlesPath).GetComponent<ParticleSystem>();
            
            if (_ambiancePartiles) {
                ParticleSystem.ColorOverLifetimeModule color = _ambiancePartiles.colorOverLifetime;
                
                GradientColorKey[] colorCopy = color.color.gradient.colorKeys;
                colorCopy[colorCopy.Length - 1].color =
                    ColorHelpers.GetPortalColor(associated.hostGun.variant, associated.side);
                
                GradientAlphaKey[] alphaCopy = color.color.gradient.alphaKeys;
                Gradient newGradient = new Gradient();
                newGradient.SetKeys(colorCopy, alphaCopy);

                color.color = newGradient;
            }
            else {
                LogError($"Couldn't find particles for {name}, please group your visuals under an object called \"{AmbianceParticlesPath}\"");
            }

            if (visualsRoot) {
                _renderers = visualsRoot.GetComponentsInChildren<Renderer>();
            }
            else {
                LogError(
                    $"Couldn't find visuals for {name}, please group your visuals under an object called \"{VisualsPath}\"");
            }
        }

        public void ColorPortal() {
            if (_renderers.Length < 1) {
                return;
            }

            UnityEngine.Color color = ColorHelpers.GetPortalColor(associated.hostGun.variant, associated.side);

            if (_ambiancePartiles) {
                _ambiancePartiles.gameObject.SetActive(ModConfig.ShowPortalAmbianceParticles.GetValue());
            }
            
            foreach (Renderer r in _renderers) {
                r.enabled = ModConfig.CanSeePortalBorders.GetValue();
                if (!r.enabled) {
                    continue;
                }
                
                r.material.SetColor("_Color", color);

                if (ModConfig.UseEmission.GetValue()) {
                    r.material.SetColor("_EmissionColor", color);
                }
                else {
                    r.material.SetColor("_EmissionColor", Color.black);
                }
            }
        }
    }
}