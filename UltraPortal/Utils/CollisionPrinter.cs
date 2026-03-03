using System;
using UnityEngine;

namespace UltraPortal {
	public class CollisionPrinter : MonoBehaviour {
		private void OnCollisionEnter(Collision other) {
			Plugin.LogSource.LogInfo($"{name} COLLIDED WITH {other.gameObject.name}");
		}

		private void OnCollisionExit(Collision other) {
			Plugin.LogSource.LogInfo($"{name} STOPPED COLLIDING WITH {other.gameObject.name}");
		}

		private void OnTriggerEnter(Collider other) {
			Plugin.LogSource.LogInfo($"{name} IN TRIGGER {other.gameObject.name}");
		}
		
		private void OnTriggerExit(Collider other) {
			Plugin.LogSource.LogInfo($"{name} STOPPED TRIGGERING {other.gameObject.name}");
		}
	}
}