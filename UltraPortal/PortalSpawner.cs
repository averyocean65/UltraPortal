using BepInEx.Logging;
using Gravity;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;

using static UltraPortal.ModConfig;
using static UltraPortal.Constants;

namespace UltraPortal {
	public class PortalSpawner : MonoBehaviour {
		private static ManualLogSource Logger => Plugin.LogSource;

		private Portal portal;
		private GameObject portalObject;
		private Vector2 portalSize = new Vector2(4, 6);

		private DynamicPortalExit portalEntry;
		private DynamicPortalExit portalExit;

		private void Start() {
			AssetBundle portals = AssetBundleHelpers.LoadAssetBundle(AssetPaths.PortalBundleName);
			GameObject portalPrefab = portals.LoadAsset<GameObject>("Portal Exit");

			if (!portalPrefab) {
				Logger.LogError("Failed to load portal prefab!");
				return;
			}
			
			GameObject portalEntryObject =
				Instantiate(portalPrefab, Vector3.zero, Quaternion.identity);
			portalEntryObject.name = "Entry";
			portalEntry = portalEntryObject.AddComponent<DynamicPortalExit>();
			portalEntry.side = PortalSide.Enter;
			
			GameObject portalExitObject =
				Instantiate(portalPrefab, Vector3.zero, Quaternion.identity);
			portalExitObject.name = "Exit";
			portalExit = portalExitObject.AddComponent<DynamicPortalExit>();
			portalExit.side = PortalSide.Exit;
			
			InitPortals();
		}
		
		private void Update() {
			if (OptionsMenuToManager.Instance.pauseMenu.activeSelf) {
				return;
			}
			
			if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasPerformedThisFrame) {
				SpawnPortal(portalEntry);
			}
			
			if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame) {
				SpawnPortal(portalExit);
			}
		}

		private void InitPortals() {
			portalObject = new GameObject("Portal") {
				layer = PortalLayer
			};

			portal = portalObject.AddComponent<Portal>();
			
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
			portal.entry = portalEntry.transform;
			portal.exit = portalExit.transform;
			portal.exitOffset = 1.5f;
			portal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			portal.fakeVPMatrix = Matrix4x4.zero;
			portal.mirror = false;
			portal.shape = new PlaneShape {
				width = portalSize.x,
				height = portalSize.y
			};
		}

		private void SpawnPortal(DynamicPortalExit exit) {
			bool success = Physics.Raycast(MainCamera.transform.position,
				MainCamera.transform.forward,
				out var hit,
				float.PositiveInfinity,
				EnvironmentLayer,
				QueryTriggerInteraction.Ignore);

			if (!success) {
				HudMessageReceiver.Instance.SendHudMessage("Failed to spawn portal end!");
				return;
			}

			HudMessageReceiver.Instance.SendHudMessage("Spawning portal end!");
			exit.Initialize(portal, exit.side, portalSize, hit);
		}
	}
}