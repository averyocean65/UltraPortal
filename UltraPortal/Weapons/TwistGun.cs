using UnityEngine;

namespace UltraPortal {
    public class TwistGun : PortalGunBase {
        protected override void Start() {
            base.Start();

            OnPrimaryFire += () => {
                _animator.Play(PrimaryFireAnimHash);
            };
        }

        public override bool ShouldBeReset() {
            return true;
        }
    }
}