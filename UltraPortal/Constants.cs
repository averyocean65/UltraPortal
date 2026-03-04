using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UltraPortal {
    public static class Constants {
        public static LayerMask EnvironmentLayer => LayerMask.GetMask("Environment", "EnvironmentBaked", "PlayerOnly", "Outdoors", "OutdoorsBaked");
        public static LayerMask PortalLayer => LayerMask.NameToLayer("Portal");

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
            public static string AssemblyPath => Assembly.GetExecutingAssembly().Location;
            public static string AssemblyFolderPath => Path.GetDirectoryName(AssemblyPath);
            public static string BundlePath => Path.Combine(AssemblyFolderPath, "Bundles");
            public const string  PortalBundle = "portals";
            public const string  WeaponBundle = "weapons";

            public const string  PortalExit = "Portal Exit";
            public const string  Mirror = "Mirror";

            public const string PortalGun = "Portal Gun";
            public const string MirrorGun = "Mirror Gun";

            public const string MainPortalProjectile = "Projectile A";
            public const string AltPortalProjectile = "Projectile B";

            public const string PortalGunIcon = "UltraPortalGunIcon";
            public const string PortalGunIconGlow = "UltraPortalGunIconGlow";
        }
    }
}