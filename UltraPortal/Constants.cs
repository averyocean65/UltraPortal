using UnityEngine;

namespace UltraPortal {
    public static class Constants {
        public static LayerMask EnvironmentLayer => LayerMask.GetMask("Environment", "EnvironmentBaked", "Outdoors", "OutdoorsBaked");
        public static LayerMask PortalLayer => LayerMask.NameToLayer("Portal");
    }
}