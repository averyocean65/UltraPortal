using System.Collections;
using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public class PortalSpawner : MonoBehaviour {
		private static ManualLogSource Logger => Plugin.LogSource;

		private static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
		private static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire"); 
		
		private bool spawnedEntry;
		private bool spawnedExit;

		private Portal portal;
		private GameObject portalObject;
		private Vector2 portalSize = new Vector2(3.9f, 5.9f);

		private DynamicPortalExit portalEntry;
		private DynamicPortalExit portalExit;
        
        private bool canPrimaryFire = true;
        private bool canSecondaryFire = true;
       
        private Animator animator;

        private float fireCooldown = 0.5f;

		private void Start() {
			AssetBundle portals = AssetBundleHelpers.LoadAssetBundle(AssetPaths.PortalBundleName);
			GameObject portalPrefab = portals.LoadAsset<GameObject>("Portal Exit");

			if (!portalPrefab) {
				Logger.LogError("Failed to load portal prefab!");
				return;
			}

			animator = GetComponentInChildren<Animator>();
			if (!animator) {
				HudMessageReceiver.Instance.SendHudMessage("<color=#ff000>Animator is invalid!</color>");
			}

			Vector3 spawnPos = Vector3.down * 100000;
			
			GameObject portalEntryObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			portalEntryObject.name = "Entry";
			portalEntry = portalEntryObject.AddComponent<DynamicPortalExit>();
			portalEntry.side = PortalSide.Enter;
			
			GameObject portalExitObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			portalExitObject.name = "Exit";
			portalExit = portalExitObject.AddComponent<DynamicPortalExit>();
			portalExit.side = PortalSide.Exit;
			
			InitPortals();
		}

		private void OnDestroy() {
			Destroy(portal.gameObject);
			Destroy(portalEntry.gameObject);
			Destroy(portalExit.gameObject);
		}

		private IEnumerator IFireCooldown(bool isPrimary) {
			int hash = isPrimary ? PrimaryFireAnimHash : SecondaryFireAnimHash;
			if (isPrimary) {
				canPrimaryFire = false;
			}
			else {
				canSecondaryFire = false;
			}

			if (animator) {
				animator.Play(hash);
			}
			
			yield return new WaitForSeconds(fireCooldown);
			
			if (isPrimary) {
				canPrimaryFire = true;
			}
			else {
				canSecondaryFire = true;
			}
		}

		private void Update() {
			if (OptionsMenuToManager.Instance.pauseMenu.activeSelf) {
				return;
			}
			
			if (MonoSingleton<InputManager>.Instance.InputSource.Fire1.WasPerformedThisFrame && canPrimaryFire) {
				SpawnPortal(portalEntry);
				spawnedEntry = true;
				StartCoroutine(IFireCooldown(true));
			}
			
			if (MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame && canSecondaryFire) {
				SpawnPortal(portalExit);
                spawnedExit = true;
				StartCoroutine(IFireCooldown(false));
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
            portal.minimumEntrySideSpeed = 10f;
            
			portal.exit = portalExit.transform;
			portal.exitOffset = 1.5f;
			portal.minimumExitSideSpeed = 10f;
			
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
				HudMessageReceiver.Instance.SendHudMessage("Failed to spawn portal!");
				return;
			}

			HudMessageReceiver.Instance.SendHudMessage("Spawning portal!");
			exit.Initialize(portal, exit.side, portalSize, hit);
		}
	}
}