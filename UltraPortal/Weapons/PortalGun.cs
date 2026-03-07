using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public sealed class PortalGun : PortalGunBase {
		public static readonly Vector3 DefaultPortalPosition = new Vector3(0, -1e6f, 0);
		
		private static ManualLogSource Logger => Plugin.LogSource;

		private static int PrimaryFireAnimHash => Animator.StringToHash("Base Layer.Primary Fire"); 
		private static int SecondaryFireAnimHash => Animator.StringToHash("Base Layer.Secondary Fire"); 
 
		private Portal _portal;
		private GameObject _portalObject;
		private readonly Vector2 _portalSize = new Vector2(5.95f, 7.95f);

		public DynamicPortalExit PortalEntry { get; private set; }
		public DynamicPortalExit PortalExit { get; private set; }
       
        private Animator _animator;

        public bool BothPortalsInit {
	        get {
		        if (!PortalEntry || !PortalExit) {
			        return false;
		        }

		        return PortalEntry.transform.position.y > DefaultPortalPosition.y &&
		               PortalExit.transform.position.y > DefaultPortalPosition.y;
	        }
        }
        
		protected override void Start() {
			base.Start();
			
			AssetBundle portals = AssetBundleHelpers.LoadAssetBundle(AssetPaths.PortalBundle);
			GameObject portalPrefab = portals.LoadAsset<GameObject>(AssetPaths.PortalExit);

			if (!portalPrefab) {
				Logger.LogError("Failed to load portal prefab!");
				return;
			}

			_animator = GetComponentInChildren<Animator>();
			if (!_animator) {
				HudMessageReceiver.Instance.SendHudMessage("<color=#ff000>Animator is invalid!</color>");
			}

			Vector3 spawnPos = DefaultPortalPosition;
			
			GameObject portalEntryObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			portalEntryObject.name = "Entry";
			PortalEntry = portalEntryObject.AddComponent<DynamicPortalExit>();
			PortalEntry.side = PortalSide.Enter;
			PortalEntry.OnInitialized += UpdatePortalPassable;
			PortalEntry.hostPortal = _portal;
			
			GameObject portalExitObject =
				Instantiate(portalPrefab, spawnPos, Quaternion.identity);
			portalExitObject.name = "Exit";
			PortalExit = portalExitObject.AddComponent<DynamicPortalExit>();
			PortalExit.OnInitialized += UpdatePortalPassable;
			PortalExit.side = PortalSide.Exit;
			PortalExit.hostPortal = _portal;

			OnPrimaryFire += () => {
				FireProjectile(PortalEntry, _portal);
				UpdateLastProjectile(PortalSide.Enter);
				_animator.Play(PrimaryFireAnimHash);
			};
			
			OnSecondaryFire += () => {
				FireProjectile(PortalExit, _portal);
				UpdateLastProjectile(PortalSide.Exit);
				_animator.Play(SecondaryFireAnimHash);
				
			};
			
			InitPortals();
		}

		public override bool ShouldBeReset() {
			if (!PortalEntry || !PortalExit) {
				return true;
			}

			if (!BothPortalsInit) {
				return true;
			}

			return PortalEntry.ShouldBeDisabled() && PortalExit.ShouldBeDisabled();
		}

		private void UpdatePortalPassable() {
			if(PortalEntry)
				PortalEntry.SetPassable(BothPortalsInit);
			
			if(PortalExit)
				PortalExit.SetPassable(BothPortalsInit);
		}

		private void OnDestroy() {
			if(_portal)
				Destroy(_portal.gameObject);
			
			if(PortalEntry)
				Destroy(PortalEntry.gameObject);
			
			if(PortalExit)
				Destroy(PortalExit.gameObject);
		}

		private void InitPortals() {
			_portalObject = new GameObject("Portal") {
				layer = PortalLayer
			};

			_portal = _portalObject.AddComponent<Portal>(); 
			
			_portal.additionalSampleThreshold = 0;
			_portal.allowCameraTraversals = true;
			_portal.appearsInRecursions = true;
			_portal.canHearAudio = false;
			_portal.canSeeItself = true;
			_portal.canSeePortalLayer = true;
			_portal.clippingMethod = PortalClippingMethod.Default;
			_portal.consumeAudio = false;
			_portal.disableRange = 0;
			_portal.enableOverrideFog = false;
			_portal.enterOffset = 1.5f;
			_portal.entry = PortalEntry.transform;
            _portal.minimumEntrySideSpeed = ModConfig.MinimumEntryExitSpeed;
            
			_portal.exit = PortalExit.transform;
			_portal.exitOffset = 1.5f;
			_portal.minimumExitSideSpeed = ModConfig.MinimumEntryExitSpeed;
			
			_portal.renderSettings = PortalSideFlags.Enter | PortalSideFlags.Exit;
			_portal.fakeVPMatrix = Matrix4x4.zero;
			_portal.mirror = false;
			_portal.shape = new PlaneShape {
				width = _portalSize.x,
				height = _portalSize.y
			};
		}

		public void Reset() {
			if (PortalEntry) {
				PortalEntry.Reset();
				PortalEntry.transform.position = DefaultPortalPosition;
			}

			if (PortalExit) {
				PortalExit.Reset();
				PortalExit.transform.position = DefaultPortalPosition;
			}

			UpdatePortalPassable();
		}
	}
}