using System;
using System.Collections;
using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UltraPortal.Projectiles;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public class PortalGun : GunBase {
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
       
        private Animator animator;

        private void FireProjectile(DynamicPortalExit exit, int animHash, bool altProjectile) {
	        string projectileAsset = altProjectile ? "Projectile A" : "Projectile B";
	        GameObject projectile = SpawnProjectileFromAsset(projectileAsset, ModConfig.PortalProjectileSpeed);
	        PortalProjectileHelper helper = projectile.AddComponent<PortalProjectileHelper>();
	        helper.exit = exit; 
	        helper.portal = portal;
	        
	        animator.Play(animHash);
        }
        
		protected override void Start() {
			base.Start();
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
			portalEntry.hostPortal = portal;
			
			GameObject portalExitObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			portalExitObject.name = "Exit";
			portalExit = portalExitObject.AddComponent<DynamicPortalExit>();
			portalExit.side = PortalSide.Exit;
			portalExit.hostPortal = portal;

			OnPrimaryFire += () => {
				FireProjectile(portalEntry, PrimaryFireAnimHash, false);
			};
			
			OnSecondaryFire += () => {
				FireProjectile(portalExit, SecondaryFireAnimHash, true);
			};
			
			InitPortals();
		}

		private void OnDestroy() {
			Destroy(portal.gameObject);
			Destroy(portalEntry.gameObject);
			Destroy(portalExit.gameObject);
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
			PlayerHeadRaycast(out bool success, out var hit);

			if (!success) {
				HudMessageReceiver.Instance.SendHudMessage("<color=red>Failed to spawn portal!</color>");
				return;
			}

			// HudMessageReceiver.Instance.SendHudMessage("Spawning portal!");
			exit.Initialize(portal, exit.side, hit);
		}
	}
}