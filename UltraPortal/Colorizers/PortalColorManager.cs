using System;
using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal.Colorizers {
    public class PortalColorManager : MonoBehaviour {
        private const string VisualsPath = "Visuals";
        public DynamicPortalExit associated;

        private Renderer[] _renderers;

        private Color GetColor() {
            switch (associated.hostGun.variant) {
                case WeaponVariant.BlueVariant:
                    return associated.side == PortalSide.Enter
                        ? ModConfig.PrimaryPortalColor.GetValue()
                        : ModConfig.SecondaryPortalColor.GetValue();
                case WeaponVariant.GreenVariant:
                    return associated.side == PortalSide.Enter
                        ? ModConfig.PrimaryMirrorColor.GetValue()
                        : ModConfig.FlippedMirrorColor.GetValue();
                case WeaponVariant.RedVariant:
                    return associated.side == PortalSide.Enter
                        ? ModConfig.PrimaryTwistColor.GetValue()
                        : ModConfig.SecondaryTwistColor.GetValue();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Start() {
            Transform visualsRoot = transform.Find(VisualsPath);
            if (!visualsRoot) {
                Plugin.LogSource.LogInfo(
                    $"Couldn't find visuals for {name}, please group your visuals under an object called \"{VisualsPath}\"");
                enabled = false;
            }

            _renderers = visualsRoot.GetComponentsInChildren<Renderer>();
        }

        public void ColorPortal() {
            if (_renderers.Length < 1) {
                return;
            }

            UnityEngine.Color color = GetColor();
            
            foreach (Renderer r in _renderers) {
                r.gameObject.SetActive(ModConfig.UsePortalBorders.GetValue());
                if (!r.gameObject.activeSelf) {
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