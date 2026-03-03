using HarmonyLib;
using UnityEngine;

namespace UltraPortal {
	[HarmonyPatch]
	public static class PlayerPatches {
		[HarmonyPrefix]
		[HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Respawn))]
		static void RespawnPatch() {
			// void ToggleObject(MonoBehaviour behaviour) {
			// 	if (!behaviour) {
			// 		return;
			// 	}
			// 	
			// 	bool prevState = behaviour.gameObject.activeSelf;
			// 	behaviour.gameObject.SetActive(true);
			// 	behaviour.Invoke("Reset", 0);
			// 	behaviour.gameObject.SetActive(prevState);
			// }
			
			PortalGun portalGun = GameObject.FindObjectOfType<PortalGun>(true);
			if(portalGun)
				portalGun.Reset();

			MirrorGun mirrorGun = GameObject.FindObjectOfType<MirrorGun>(true);
			if(mirrorGun)
				mirrorGun.Reset();
		}
	}
}