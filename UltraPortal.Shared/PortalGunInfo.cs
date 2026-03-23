using UnityEngine;

namespace UltraPortal.Shared {
    public class PortalGunInfo : MonoBehaviour {
        public Animator animator;
        [SerializeField] private string primaryFireAnimationName;
        [SerializeField] private string altFireAnimationName;
        [SerializeField] private string closeAnimationName;

        public GameObject lastProjectile;
        
        public int PrimaryFireAnimation => Animator.StringToHash(primaryFireAnimationName);
        public int AltFireAnimation => Animator.StringToHash(altFireAnimationName);
        public int CloseAnimation => Animator.StringToHash(closeAnimationName);
    }
}