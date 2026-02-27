using BepInEx.Logging;
using Gravity;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;

namespace UltraPortal {
	public class PortalSpawner : MonoBehaviour {
		private static LayerMask EnvironmentLayer => LayerMask.GetMask("Environment", "EnvironmentBaked", "Outdoors", "OutdoorsBaked");
		private static LayerMask PortalLayer => LayerMask.NameToLayer("Portal");
		private static ManualLogSource Logger => Plugin.LogSource;
		
		private void Update() {
			if (Input.GetKeyDown(KeyCode.T)) {
				Transform mainCam = Camera.main.transform;
                
				if (Physics.Raycast(mainCam.position, mainCam.forward, out var hit,  100, EnvironmentLayer)) {
					SpawnMirror(hit.point, -hit.normal);
				}
				else {
					HudMessageReceiver.Instance.SendHudMessage($"Failed to find suitable portal location!");
				}
			}
		}

		private void SpawnMirror(Vector3 position, Vector3 forward) {
			HudMessageReceiver.Instance.SendHudMessage($"Spawning portal at {position}, facing {forward}");

			GameObject mirrorObject = new GameObject("Custom Portal Spawn");

			mirrorObject.layer = PortalLayer;
			mirrorObject.transform.position = position - (forward * 0.001f);
			mirrorObject.transform.forward = forward;

			Portal portal = mirrorObject.AddComponent<Portal>();
			
			portal.additionalSampleThreshold = 0;
			portal.allowCameraTraversals = true;
			portal.appearsInRecursions = true;
			portal.canHearAudio = false;
			portal.canSeeItself = true;
			portal.canSeePortalLayer = true;
			portal.clippingMethod = PortalClippingMethod.Default;
			portal.consumeAudio = false;
			portal.disableRange = 0;
			portal.enableOverrideFog = false;
			portal.enterOffset = 1.5f;
			portal.entry = portal.transform;
			portal.exit = portal.transform;
			portal.exitOffset = 1.5f;
			portal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			portal.fakeVPMatrix = Matrix4x4.zero;
			portal.mirror = true;
			portal.shape = new PlaneShape {
				width = 6,
				height = 6
			};
		}
	}
}