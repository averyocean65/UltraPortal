using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UltraPortal {
    public static class Constants {
        public static LayerMask EnvironmentLayer => LayerMask.GetMask("Environment", "EnvironmentBaked", "PlayerOnly", "Outdoors", "OutdoorsBaked");
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

        public static class AssetPaths {
            public static string BundlePath => Path.Combine(Environment.CurrentDirectory, "ULTRAPORTAL", "Bundles");
            public static readonly string PortalBundleName = "portals";
            public static string PortalBundlePath = Path.Combine(BundlePath, PortalBundleName);
        }
    }
}