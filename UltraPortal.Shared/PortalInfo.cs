using UnityEngine;

namespace UltraPortal.Shared {
	public class PortalInfo : MonoBehaviour {
		public GameObject passable;
		
		public Renderer[] portalEdgeRenderers;
		
		public ParticleSystem spawnParticles;
		public ParticleSystem ambianceParticles;
		public Collider[] portalColliders;
	}
}