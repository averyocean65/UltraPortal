using System;
using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal.Colorizers {
    public class PortalColorManager : MonoBehaviour {
        private const string VisualsPath = "Visuals";
        public DynamicPortalExit associated;

        private Renderer[] _renderers;
        
        private void Start() {
            _renderers = associated.info.portalEdgeRenderers;
        }

        public void ColorPortal() {
            if (_renderers.Length < 1) {
                return;
            }

            Color color = ColorHelpers.GetPortalColor(associated.hostGun.variant, associated.side);
            
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