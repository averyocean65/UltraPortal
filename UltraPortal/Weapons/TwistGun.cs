using System;
using Gravity;
using ULTRAKILL.Portal;
using UltraPortal.Extensions;
using UnityEngine;

namespace UltraPortal {
    public class TwistGun : PortalGunBase {
        public Portal PrimaryPortal { get; private set; }
        public DynamicPortalExit TwistEntry { get; private set; }
        public DynamicPortalExit TwistExit { get; private set; }
        
        private readonly Vector2 _portalSize = new Vector2(5.95f, 7.95f);

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
                _animator.Play(PrimaryFireAnimHash);
            };
            
            OnSecondaryFire += () => {
                FireProjectile(TwistExit, PrimaryPortal);
                UpdateLastProjectile(PortalSide.Exit);
                _animator.Play(SecondaryFireAnimHash);
            };
            
            TwistEntry.OnPlayerTravelled += (entryExit) => OnPlayerTravelled(entryExit, TwistEntry);
            TwistExit.OnPlayerTravelled += (entryExit) => OnPlayerTravelled(entryExit, TwistExit);
            
            InitPortal();
        }

        private void OnPlayerTravelled(bool entryExit, DynamicPortalExit exit) {
            if (entryExit) {
                return;
            }
            
            NewMovement.Instance.rb.SetCustomGravityMode(true);
            NewMovement.Instance.rb.SetCustomGravity(-exit.transform.forward.normalized * Physics.gravity.magnitude);
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
        }
    }
}