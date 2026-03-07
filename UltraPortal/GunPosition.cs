using UnityEngine;

namespace UltraPortal {
    public struct GunPosition {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public GunPosition(Vector3 position, Vector3 rotation, Vector3 scale) {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}