using HarmonyLib;
using ULTRAKILL.Portal;
using UltraPortal.Extensions;
using UnityEngine;

namespace UltraPortal {
	[HarmonyPatch]
	public static class PlayerPatches {
		[HarmonyPrefix]
		[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Respawn))]
		static void RespawnPatch() {
			PortalGun portalGun = GameObject.FindObjectOfType<PortalGun>(true);
			if(portalGun)
				portalGun.Reset();

			MirrorGun mirrorGun = GameObject.FindObjectOfType<MirrorGun>(true);
			if(mirrorGun)
				mirrorGun.Reset();
			
			TwistGun twistGun = GameObject.FindObjectOfType<TwistGun>(true);
			if(twistGun)
				twistGun.Reset();
		}

		public static bool RespawnFlag { get; private set; } = false;
		
		// I could make this update patch a class, but I'm too lazy ngl
		
		[HarmonyPrefix]
		[HarmonyPatch(typeof(NewMovement), "Update")]
		static void UpdatePatch() {
			if (NewMovement.Instance.transform.position.y < -1e5) {
				if (RespawnFlag) {
					return;
				}
				
				RespawnFlag = true;
				
				HudMessageReceiver.Instance.SendHudMessage("Sorry about that!\n- ULTRAPORTAL Developers");

				if (StatsManager.Instance.currentCheckPoint) {
					StatsManager.Instance.currentCheckPoint.OnRespawn();
				}
				else {
					SceneHelper.RestartScene();
				}
			}
			else {
				RespawnFlag = false;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.OnTeleportBlocked))]
		static void TeleportBlockedPatch(PortalTravelDetails details, ref bool __runOriginal) {
			if (details.enterHandle.IsUltraPortal()) {
				__runOriginal = false;
			}
		}
	}
}