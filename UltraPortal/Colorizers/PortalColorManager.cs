using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal.Colorizers {
    public class PortalColorManager : MonoBehaviour {
        private const string VisualsPath = "Visuals";
        public DynamicPortalExit associated;

        private Renderer[] _renderers;
        
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
            
            UnityEngine.Color color = associated.side == PortalSide.Enter
                ? ModConfig.PrimaryPortalColor.GetValue()
                : ModConfig.SecondaryPortalColor.GetValue();
            
            foreach (Renderer r in _renderers) {
                r.enabled = ModConfig.ShowPortalBorders.GetValue();
                if (!r.enabled) {
                    return;
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