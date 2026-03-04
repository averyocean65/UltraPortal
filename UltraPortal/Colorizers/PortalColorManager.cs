using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal.Colorizers {
    public class PortalColorManager : MonoBehaviour {
        private const string VisualsPath = "Visuals";
        public DynamicPortalExit associated;
        
        private void Start() {
            Transform visualsRoot = transform.Find(VisualsPath);
            if (!visualsRoot) {
                Plugin.LogSource.LogInfo(
                    $"Couldn't find visuals for {name}, please group your visuals under an object called \"{VisualsPath}\"");
                enabled = false;
            }

            Renderer[] renderers = visualsRoot.GetComponentsInChildren<Renderer>();
            UnityEngine.Color color = associated.side == PortalSide.Enter
                ? ModConfig.PrimaryPortalColor
                : ModConfig.SecondaryPortalColor;

            // you WILL change your color, i'm no longer asking.
            foreach (Renderer r in renderers) {
                r.material.color = color;
            }
        }
    }
}