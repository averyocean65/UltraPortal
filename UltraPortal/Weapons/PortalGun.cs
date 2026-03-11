using BepInEx.Logging;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;
using static UltraPortal.Constants;

namespace UltraPortal {
	public sealed class PortalGun : PortalGunBase {
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

        public void SpawnEntry(bool reinit = false) {
	        PortalEntry = SpawnPortalExit("Entry", PortalSide.Enter, _portal);
	        if (PortalEntry) {
		        PortalEntry.OnInitialized += UpdatePortalPassable;
	        }

	        if (reinit) {
		        InitPortals();
		        UpdatePortalPassable();
	        }
        }

        public void SpawnExit(bool reinit = false) {
	        PortalExit = SpawnPortalExit("Exit", PortalSide.Exit, _portal);
	        if (PortalExit) {
		        PortalExit.OnInitialized += UpdatePortalPassable;
	        }
	        
	        if (reinit) {
		        InitPortals();
		        UpdatePortalPassable();
	        }
        }
        
		protected override void Start() {
			base.Start();

			_animator = GetComponentInChildren<Animator>();
			if (!_animator) {
				HudMessageReceiver.Instance.SendHudMessage("<color=#ff000>Animator is invalid!</color>");
			}
			
			SpawnEntry();
			SpawnExit();

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

			return PortalEntry.ShouldBeDisabled() && PortalExit.ShouldBeDisabled();
		}

		private void UpdatePortalPassable() {
			if(PortalEntry)
				PortalEntry.SetPassable(BothPortalsInit);
			
			if(PortalExit)
				PortalExit.SetPassable(BothPortalsInit);
		}

		private void OnDestroy() {
			if (PortalEntry) {
				Destroy(PortalEntry.gameObject);
			}

			if (PortalExit) {
				Destroy(PortalExit.gameObject);
			}
			
			if (_portal) {
				Destroy(_portal.gameObject);
			}
		}

		private void InitPortals() {
			_portal = CreatePortal("Portal", PortalEntry.transform, PortalExit.transform, _portalSize);
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