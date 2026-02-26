using BepInEx.Logging;
using ULTRAKILL.Portal;
using UnityEngine;
using Logger = UnityEngine.Logger;

namespace UltraPortal {
	public class PortalSpawner : MonoBehaviour {
		private static ManualLogSource Logger => Plugin.LogSource;
		
		private void Update() {
			if (Input.GetKeyDown(KeyCode.P)) {
				HudMessageReceiver.Instance.SendHudMessage($"Trying to spawn portal!");
				Transform mainCam = Camera.main.transform;
                
				if (Physics.Raycast(mainCam.position, mainCam.forward, out var hit,  100, LayerMask.NameToLayer("Default"))) {
					SpawnMirror(hit.point, hit.normal);
				}
			}
		}

		private void SpawnMirror(Vector3 position, Vector3 forward) {
			HudMessageReceiver.Instance.SendHudMessage($"Spawning portal at {position}, facing {forward}");
            
			GameObject mirrorObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
			mirrorObject.transform.position = position;
			mirrorObject.transform.forward = forward;
            
			Portal portal = mirrorObject.AddComponent<Portal>();

			portal.mirror = true;
			portal.supportInfiniteRecursion = false;
		}
	}
}