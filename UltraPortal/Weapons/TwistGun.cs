using System;
using ULTRAKILL.Portal;
using UnityEngine;

namespace UltraPortal {
    public class TwistGun : PortalGunBase {
        public Portal PrimaryPortal { get; private set; }
        public DynamicPortalExit Entry { get; private set; }
        public DynamicPortalExit Exit { get; private set; }
        
        private readonly Vector2 _portalSize = new Vector2(5.95f, 7.95f);

        public bool BothPortalsInit {
            get {
                if (!Entry || !Exit) {
                    return false;
                }

                return Entry.IsInitialized && Exit.IsInitialized;
            }
        }

        public void SpawnEntry(bool reinit = false) {
            Entry = SpawnPortalExit("Twist Entry", PortalSide.Enter, PrimaryPortal);
            if (Entry) {
                Entry.OnInitialized += UpdatePortalPassable;
            }

            if (reinit) {
                InitPortal();
                UpdatePortalPassable();
            }
        }
        
        public void SpawnExit(bool reinit = false) {
            Exit = SpawnPortalExit("Twist Exit", PortalSide.Exit, PrimaryPortal);
            if (Exit) {
                Exit.OnInitialized += UpdatePortalPassable;
            }

            if (reinit) {
                InitPortal();
                UpdatePortalPassable();
            }
        }

        private void UpdatePortalPassable() {
            if (Entry) {
                Entry.SetPassable(BothPortalsInit);
            }

            if (Exit) {
                Exit.SetPassable(BothPortalsInit);
            }
        }

        protected override void Start() {
            base.Start();
            
            SpawnEntry();
            SpawnExit();
            
            Exit.OnInitialized += () => {
                Vector3 euler = Exit.transform.localEulerAngles;
                Exit.transform.localEulerAngles = new Vector3(euler.x, euler.y, ModConfig.TwistGunAngle.GetValue());
            };
            
            OnPrimaryFire += () => {
                FireProjectile(Entry, PrimaryPortal);
                UpdateLastProjectile(PortalSide.Enter);
                _animator.Play(PrimaryFireAnimHash);
            };
            
            OnSecondaryFire += () => {
                FireProjectile(Exit, PrimaryPortal);
                UpdateLastProjectile(PortalSide.Exit);
                _animator.Play(SecondaryFireAnimHash);
            };
            
            InitPortal();
        }

        public override bool ShouldBeReset() {
            if (!Entry || !Exit) {
                return true;
            }

            return Entry.ShouldBeDisabled() && Exit.ShouldBeDisabled();
        }

        public void Reset() {
            if (Entry) {
                Entry.Reset();
                Entry.transform.position = DefaultPortalPosition;
            }

            if (Exit) {
                Exit.Reset();
                Exit.transform.position = DefaultPortalPosition;
            }

            UpdatePortalPassable();
        }

        private void InitPortal() {
            PrimaryPortal = CreatePortal("Twist Portal", Entry.transform, Exit.transform, _portalSize);
            PrimaryPortal.usePerceivedGravityOnEnter = true;
            PrimaryPortal.usePerceivedGravityOnExit = true;
        }
    }
}