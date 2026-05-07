using System;
using UnityEngine;
using static UltraPortal.Constants;
using static UltraPortal.DebugUtils;

namespace UltraPortal.Development {
    public class PathGun : GunBase {
        protected override void Start() {
            base.Start();
            
            OnPrimaryFire = FirePrimary;
            HudMessageReceiver.Instance.SendHudMessage("Primary Fire: Get path in scene\nSecondary Fire: undefined");
        }

        private void FirePrimary() {
            LogInfo("firing!");
            bool success = PortalPhysicsV2.Raycast(MainCamera.transform.position, MainCamera.transform.forward,
                out PhysicsCastResult result, Mathf.Infinity,
                EnvironmentLayer, QueryTriggerInteraction.Ignore);

            if (!success) {
                HudMessageReceiver.Instance.SendHudMessage("<color=red>Failed to find object!</color>");
                return;
            }

            HudMessageReceiver.Instance.SendHudMessage($"{result.collider.gameObject.name}");
        }
    }
}