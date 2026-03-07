using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UltraPortal {
    public static class Constants {
        public static LayerMask EnvironmentLayer => LayerMask.GetMask("Environment", "EnvironmentBaked", "PlayerOnly", "Outdoors", "OutdoorsBaked");
        public static LayerMask PortalLayer => LayerMask.NameToLayer("Portal");
        public static LayerMask IgnoreTravellersLayerMask => LayerMask.NameToLayer("GibLit");

        private static Camera _mainCamera;

        public static Camera MainCamera {
            get {
                if (!_mainCamera) {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }

        public static class AssetPaths {
            public static bool UseAltBundlePath = false;
            public static string AssemblyPath => Assembly.GetExecutingAssembly().Location;
            public static string AssemblyFolderPath => Path.GetDirectoryName(AssemblyPath);

            public static string BundlePath {
                get {
                    if (UseAltBundlePath) {
                        return AssemblyFolderPath;
                    }

                    return Path.Combine(AssemblyFolderPath, "Bundles");
                }
            }
            public const string  PortalBundle = "portals";
            public const string  WeaponBundle = "weapons";

            public const string  PortalExit = "Portal Exit";
            public const string  Mirror = "Mirror";

            public const string PortalGun = "Portal Gun";
            public const string MirrorGun = "Mirror Gun";
            
            public const string Projectile = "Projectile";

            public const string PortalGunIcon = "UltraPortalGunIcon";
            public const string PortalGunIconGlow = "UltraPortalGunIconGlow";

            public const string Explosion = "Explosion";
        }
    }
}