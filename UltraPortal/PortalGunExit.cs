using System;
using UnityEngine;
using UnityEngine.Serialization;
using static UltraPortal.Constants;

namespace UltraPortal {
    public class PortalGunExit : MonoBehaviour {
        public float playerCheckRadius = 3.0f;
        public float checkSphereRadius = 0.1f;
        
        public bool playerAboutToPass;
        public PortalGunExit link;
        
        public Collider[] collidersToCheck = {};

        private void Start() {
            UpdateColliders();
        }

        private void Update() {
            float distance = Vector3.Distance(transform.position, NewMovement.Instance.Transform.position);
            bool inRadius = distance < playerCheckRadius;
            link.playerAboutToPass = inRadius;
            
            bool disableColliders = inRadius || playerAboutToPass;
            ToggleColliders(disableColliders);
        }

        public void UpdateColliders() {
            // re-enable previously active colliders
            ToggleColliders(true);
            
            // get new colliders
            collidersToCheck = Physics.OverlapSphere(transform.position, checkSphereRadius, EnvironmentLayer);
        }

        public void ToggleColliders(bool value) {
            if (collidersToCheck == null) {
                Plugin.LogSource.LogError("Collider array not initialized!");
                return;
            }
            
            if (collidersToCheck.Length < 1) {
                return;
            }

            foreach (Collider c in collidersToCheck) {
                if (!c) {
                    continue;
                }
                
                c.gameObject.GetComponent<Renderer>().material.color = Color.red; 
                c.enabled = value;
            }
        }
    }
}