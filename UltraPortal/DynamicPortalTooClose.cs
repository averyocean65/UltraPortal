using System;
using System.Collections.Generic;
using UltraPortal.Extensions;
using UnityEngine;

using static UltraPortal.Constants;

namespace UltraPortal {
	public class DynamicPortalTooClose : MonoBehaviour {
		public List<Collider> travellers;

		private void Awake() {
			travellers = new List<Collider>();
		}

		private void OnTriggerEnter(Collider other) {
			if (!other.attachedRigidbody || other.isTrigger)
				return;
			
			travellers.Add(other);
		}

		private void OnTriggerExit(Collider other) {
			if (travellers.Contains(other)) {
				travellers.Remove(other);
			}
		}
	}
}