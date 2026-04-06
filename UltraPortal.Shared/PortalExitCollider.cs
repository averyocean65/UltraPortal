using System;
using UnityEngine;
using UnityEngine.Events;

namespace UltraPortal.Shared {
	[RequireComponent(typeof(BoxCollider))]
	public class PortalExitCollider : MonoBehaviour {
		public UnityEvent<Collider> OnEnter;
		public UnityEvent<Collider> OnExit;
		public bool detectsPlayer;
		
		private Collider _collider;
		
		private void Start() {
			_collider = GetComponent<Collider>();
			_collider.isTrigger = true;
		}

		private void OnTriggerEnter(Collider other) {
			OnEnter.Invoke(other);
		}

		private void OnTriggerExit(Collider other) {
			OnExit.Invoke(other);
		}
	}
}