using HarmonyLib;
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
	}
}