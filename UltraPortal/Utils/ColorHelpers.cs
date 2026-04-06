using System;
using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal {
    public static class ColorHelpers {
        public static Color GetPortalColor(WeaponVariant variant, PortalSide side) {
            switch (variant) {
                case WeaponVariant.BlueVariant:
                    return side == PortalSide.Enter
                        ? ModConfig.PrimaryPortalColor.GetValue()
                        : ModConfig.SecondaryPortalColor.GetValue();
                case WeaponVariant.GreenVariant:
                    return side == PortalSide.Enter
                        ? ModConfig.PrimaryMirrorColor.GetValue()
                        : ModConfig.FlippedMirrorColor.GetValue();
                case WeaponVariant.RedVariant:
                    return side == PortalSide.Enter
                        ? ModConfig.PrimaryTwistColor.GetValue()
                        : ModConfig.SecondaryTwistColor.GetValue();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public static Color GetPortalParticleColor(WeaponVariant variant, PortalSide side) {
            switch (variant) {
                case WeaponVariant.BlueVariant:
                    return side == PortalSide.Enter
                        ? ModConfig.PrimaryPortalParticleColor.GetValue()
                        : ModConfig.SecondaryPortalParticleColor.GetValue();
                case WeaponVariant.GreenVariant:
                    return side == PortalSide.Enter
                        ? ModConfig.PrimaryMirrorParticleColor.GetValue()
                        : ModConfig.FlippedMirrorParticleColor.GetValue();
                case WeaponVariant.RedVariant:
                    return side == PortalSide.Enter
                        ? ModConfig.PrimaryTwistParticleColor.GetValue()
                        : ModConfig.SecondaryTwistParticleColor.GetValue();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}