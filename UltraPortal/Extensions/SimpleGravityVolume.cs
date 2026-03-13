using Gravity;
using UnityEngine;

namespace UltraPortal.Extensions {
    public class SimpleGravityVolume : GravityVolume {
        public Vector3 DesiredGravity = new Vector3(0, -40, 0);

        protected override Vector3 CalculateGravityVector() {
            return DesiredGravity;
        }
    }
}