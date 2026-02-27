using BepInEx.Logging;
using JetBrains.Annotations;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public class PortalSpawner : MonoBehaviour {
		private static ManualLogSource Logger => Plugin.LogSource;

		private PortalGunExit entry;
		private PortalGunExit exit;
		private GameObject portalObject;
		
		private bool PerformPlayerRaycast(out RaycastHit hit) {
			Transform mainCam = Camera.main.transform;
			return Physics.Raycast(mainCam.position, mainCam.forward, out hit, 500f, EnvironmentLayer);
		}

		private void Start() {
			GameObject entryObject = new GameObject("Entry") {
				transform = {
					parent = transform
				}
			};
			entry = entryObject.AddComponent<PortalGunExit>();

			GameObject exitObject = new GameObject("Exit") {
				transform = {
					parent = transform
				}
			};
			exit = exitObject.AddComponent<PortalGunExit>();
			
			entry.link = exit;
			exit.link = entry;
			
			InitPortals();
		}

		private void SetTransformToLook(Transform affected, [CanBeNull] PortalGunExit pgExit) {
			if (PerformPlayerRaycast(out var hit)) {
				affected.position = hit.point + (hit.normal.normalized * 0.05f);
				affected.forward = -hit.normal;

				if (pgExit) {
					pgExit.UpdateColliders();
				}
			}
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.T)) {
				portalObject.SetActive(!portalObject.activeSelf);
				HudMessageReceiver.Instance.SendHudMessage($"Toggled on?: {portalObject.activeSelf}");
			}

			if (Input.GetKeyDown(KeyCode.I)) {
				SetTransformToLook(entry.transform, entry);
			}
			
			if (Input.GetKeyDown(KeyCode.U)) {
				SetTransformToLook(exit.transform, exit);
			}
		}

		private void InitPortals() {
			portalObject = new GameObject("Portal") {
				transform = {
					parent = transform
				}
			};
			
			portalObject.layer = PortalLayer;

			Portal portal = portalObject.AddComponent<Portal>();
			
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
			portal.entry = entry.transform;
			portal.exit = exit.transform;
			portal.exitOffset = 1.5f;
			portal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			portal.fakeVPMatrix = Matrix4x4.zero;
			portal.mirror = false;
			portal.shape = new PlaneShape {
				width = 4,
				height = 6
			};
		}
	}
}