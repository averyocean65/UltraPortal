using Gravity;
using UnityEngine;
using UnityEngine.Serialization;

namespace UltraPortal.Extensions {
    public class DirectionalGravityVolume : GravityVolume {
        public Vector3 Forward = new Vector3(0, -40, 0);

        protected override Vector3 CalculateGravityVector() {
            return Vector3.up * -Physics.gravity.magnitude;
        }
    }
}