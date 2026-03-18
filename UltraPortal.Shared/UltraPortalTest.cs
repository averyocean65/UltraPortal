using System;
using UnityEngine;

namespace UltraPortal.Shared {
    public class UltraPortalTest : MonoBehaviour {
        [SerializeField] private Renderer renderer;
        [SerializeField] private Color startUpColor;

        private void Start() {
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = startUpColor;

            renderer.material = newMaterial;
        }
    }
}