using System;
using ULTRAKILL.Portal;
using UnityEngine;

using static UltraPortal.DebugUtils;

namespace UltraPortal.Colorizers {
    public class PortalColorManager : MonoBehaviour {
        public DynamicPortalExit associated;

        private Renderer[] _renderers;
        private ParticleSystem _ambiancePartiles;
        
        private void Start() {
            _renderers = associated.info.portalEdgeRenderers;
            _ambiancePartiles = associated.info.ambianceParticles;
            
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