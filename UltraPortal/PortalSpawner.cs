using System;
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

		private GameObject entry;
		private GameObject exit;
		private GameObject portalObject;
		
		private bool PerformPlayerRaycast(out RaycastHit hit) {
			Transform mainCam = Camera.main.transform;
			return Physics.Raycast(mainCam.position, mainCam.forward, out hit, 500f, EnvironmentLayer);
		}

		private void Start() {
			entry = new GameObject("Entry") {
				transform = {
					parent = transform
				}
			};

			exit = new GameObject("Exit") {
				transform = {
					parent = transform
				}
			};

			InitPortals();
		}

		private void SetTransformToLook(Transform affected) {
			if (PerformPlayerRaycast(out var hit)) {
				affected.position = hit.point + (hit.normal.normalized * 0.7f);
				affected.forward = -hit.normal;
			}
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.T)) {
				portalObject.SetActive(!portalObject.activeSelf);
				HudMessageReceiver.Instance.SendHudMessage($"Toggled on?: {portalObject.activeSelf}");
			}

			if (Input.GetKeyDown(KeyCode.I)) {
				SetTransformToLook(entry.transform);
			}
			
			if (Input.GetKeyDown(KeyCode.U)) {
				SetTransformToLook(exit.transform);
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
			portal.enterOffset = 0.5f;
			portal.entry = entry.transform;
			portal.exit = exit.transform;
			portal.exitOffset = 0.5f;
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