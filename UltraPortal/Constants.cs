using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UltraPortal {
    public static class Constants {
        public static LayerMask EnvironmentLayer => LayerMaskDefaults.Get(LMD.Environment) | LayerMask.GetMask("PlayerOnly");
        public static LayerMask PortalLayer => LayerMask.NameToLayer("Portal");

        public const string PortalExplosionWeapon = "dynamicportal";
        public const string PortalProjectileWeapon = "portalprojectile";

        public const string StyleSafetyHazardId = "style.ultraportal.safetyhazard";
        public const string StyleSafetyHazardName = "SAFETY HAZARD";
        public const int StyleSafetyHazardPoints = 100;
        
        public const string StylePortalProjectileId = "style.ultraportal.projectiled";
        public const string StylePortalProjectileName = "DISPLACEMENT";
        public const int StylePortalProjectilePoints = 25;
        

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
            public const string  DebugBundle = "debug";

            public const string PortalExit = "Portal Exit";
            public const string Mirror = "Mirror";

            public const string PortalGun = "Portal Gun";
            public const string MirrorGun = "Mirror Gun";
            public const string TwistGun = "Twist Gun Variant";
            
            public const string Projectile = "Projectile";

            public const string PortalGunIcon = "UltraPortalGunIcon";
            public const string PortalGunIconGlow = "UltraPortalGunIconGlow";

            public const string Explosion = "Explosion";

            public const string DebugForwardArrow = "ForwardArrow";
        }
    }
}