using System.Collections;
using ULTRAKILL.Portal;
using ULTRAKILL.Portal.Geometry;
using UnityEngine;

using static UltraPortal.DebugUtils;

namespace UltraPortal {
    public class TwistGun : PortalGunBase {
        public Portal PrimaryPortal { get; private set; }
        public DynamicPortalExit TwistEntry { get; private set; }
        public DynamicPortalExit TwistExit { get; private set; }
        
        private readonly Vector2 _portalSize = new Vector2(5.95f, 7.95f) * ModConfig.PortalScaleMod.GetValue();

        public bool BothPortalsInit {
            get {
                if (!TwistEntry || !TwistExit) {
                    return false;
                }

                return TwistEntry.IsInitialized && TwistExit.IsInitialized;
            }
        }

        public void SpawnEntry(bool reinit = false) {
            TwistEntry = SpawnPortalExit("Twist Entry", PortalSide.Enter, PrimaryPortal);
            if (TwistEntry) {
                TwistEntry.OnInitialized += UpdatePortalPassable;
            }

            if (reinit) {
                InitPortal();
                UpdatePortalPassable();
            }
        }
        
        public void SpawnExit(bool reinit = false) {
            TwistExit = SpawnPortalExit("Twist Exit", PortalSide.Exit, PrimaryPortal);
            if (TwistExit) {
                TwistExit.OnInitialized += UpdatePortalPassable;
            }
            
            if (reinit) {
                InitPortal();
                UpdatePortalPassable();
            }
        }

        private void UpdatePortalPassable() {
            if (TwistEntry) {
                TwistEntry.SetPassable(BothPortalsInit);
            }

            if (TwistExit) {
                TwistExit.SetPassable(BothPortalsInit);
            }
        }

        protected override void Start() {
            base.Start();
            
            SpawnEntry();
            SpawnExit();
            
            OnPrimaryFire += () => {
                FireProjectile(TwistEntry, PrimaryPortal);
                UpdateLastProjectile(PortalSide.Enter);
                _animator.Play(_info.PrimaryFireAnimation);
            };
            
            OnSecondaryFire += () => {
                FireProjectile(TwistExit, PrimaryPortal);
                UpdateLastProjectile(PortalSide.Exit);
                _animator.Play(_info.AltFireAnimation);
            };
            
            InitPortal();
            
            TwistEntry.otherExit = TwistExit;
            TwistExit.otherExit = TwistEntry;
        }

        private IEnumerator ISwitchRigidbodyGravity(Rigidbody rb, DynamicPortalExit exit) {
            LogInfo("Changing player gravity");

            yield return new WaitForSecondsRealtime(0.05f);
            
            rb.SetCustomGravityMode(true);
            rb.SetCustomGravity(-exit.transform.forward.normalized *
                                                     Physics.gravity.magnitude);
        }
        
        private bool _travelEventLocked = false;
        private void OnObjectTravel(DynamicPortalExit exit, IPortalTraveller traveller, PortalTravelDetails details) {
            LogVerboseInfo("Checking portal traveller");
            if (traveller is MonoBehaviour mono) {
                LogVerboseInfo($"Found {nameof(MonoBehaviour)} on {mono.name}");
                Rigidbody rb = mono.GetComponent<Rigidbody>();
                if (!rb) {
                    LogVerboseWarning($"{mono.name} doesn't have a rigidbody! Aborting gravity change!");
                }
                
                StartCoroutine(ISwitchRigidbodyGravity(rb, exit));
            }
        }

        public override bool ShouldBeReset() {
            if (!TwistEntry || !TwistExit) {
                return true;
            }

            return TwistEntry.ShouldBeDisabled() && TwistExit.ShouldBeDisabled();
        }

        public void Reset() {
            if (TwistEntry) {
                TwistEntry.Reset();
                TwistEntry.transform.position = DefaultPortalPosition;
            }

            if (TwistExit) {
                TwistExit.Reset();
                TwistExit.transform.position = DefaultPortalPosition;
            }

            UpdatePortalPassable();
        }

        private void InitPortal() {
            PrimaryPortal = CreatePortal("Twist Portal", TwistEntry.transform, TwistExit.transform, _portalSize);
            // PrimaryPortal.onEntryTravel = new UnityEventPortalTravel();
            // PrimaryPortal.onEntryTravel.AddListener((traveller, details) =>
            //     OnObjectTravel(TwistExit, traveller, details));
            
            PrimaryPortal.onExitTravel = new UnityEventPortalTravel();
            PrimaryPortal.onExitTravel.AddListener((traveller, details) =>
                OnObjectTravel(TwistEntry, traveller, details));
        }
    }
}