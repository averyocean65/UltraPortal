using UnityEngine;

namespace UltraPortal {
    public static class Constants {
        public static LayerMask EnvironmentLayer => LayerMask.GetMask("Environment", "EnvironmentBaked", "Outdoors", "OutdoorsBaked");
        public static LayerMask PortalPassableLayer => LayerMask.GetMask("Projectile", "Default", "IgnoreRaycast");
        public static LayerMask PortalLayer => LayerMask.NameToLayer("Portal");

        private static Camera _mainCamera = null;

        public static Camera MainCamera {
            get {
                if (!_mainCamera) {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }
        
        public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
        public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
    }
}