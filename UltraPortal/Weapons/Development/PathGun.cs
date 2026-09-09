using System;
using ULTRAKILL.Portal;
using UnityEngine;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal.Development {
    public class PathGun : PortalGunBase {
        private Collider _lastCollider;
        private Renderer _lastRenderer;
        private Material _lastPrimaryMaterial;
        
        protected override void Start() {
            base.Start();
            
            OnPrimaryFire += () => {
                GetScenePath();
            };
            
            OnSecondaryFire += () => {
                if (_lastCollider) {
                    Destroy(_lastCollider.gameObject);
                }
            };
            
            HudMessageReceiver.Instance.SendHudMessage("Primary Fire: Select object\nSecondary Fire: Delete object");
        }

        public void Reset() {
            Unselect();
            
            _lastCollider = null;
            _lastRenderer = null;
            _lastPrimaryMaterial = null;
        }

        public override bool ShouldBeReset() {
            return true;
        }
        public override bool ShouldPlayReset() {
            return true;
        }

        protected void GetScenePath() {
            bool success = PortalPhysicsV2.Raycast(MainCamera.transform.position, MainCamera.transform.forward,
                out PhysicsCastResult result, Mathf.Infinity,
                EnvironmentLayer, QueryTriggerInteraction.Ignore);

            if (!success) {
                HudMessageReceiver.Instance.SendHudMessage("<color=red>Failed to find object!</color>");
                return;
            }
            
            Unselect();
            Select(result);
        }

        private void Select(PhysicsCastResult hit) {
            _lastCollider = hit.collider;
            _lastRenderer = hit.collider.GetComponent<Renderer>();
            
            if (_lastRenderer) {
                _lastPrimaryMaterial = _lastRenderer.material;
                _lastRenderer.material = new Material(Shader.Find("Standard")); 
                _lastRenderer.material.SetColor("_EmissionColor", Color.red * 5);
            }
        }

        private void Unselect() {
            if (_lastRenderer) {
                _lastRenderer.material = _lastPrimaryMaterial;
            }
        }
    }
}