using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltraPortal {
	public class KeepActive : MonoBehaviour {
		public GameObject target;
		// private List<GameObject> _cachedHierarchy;
		// private GameObject _cacheHolder;
		
		private List<GameObject> GetTargetHierarchy() {
			// if (_cacheHolder == target) {
			// 	return _cachedHierarchy;
			// }
			
			List<GameObject> output = new List<GameObject>();
			output.Add(target);

			GameObject observed = target;
			while (observed.transform.parent) {
				observed = observed.transform.parent.gameObject;
				output.Add(observed);
			}

			// _cachedHierarchy = output;
			// _cacheHolder = target;
			
			return output;
		}
		
		private void Update() {
			if (!target || NewMovement.Instance.dead) {
				return;
			}
			
			if (!target.activeInHierarchy) {
				List<GameObject> hierarchy = GetTargetHierarchy();
				foreach (var obj in hierarchy) {
					obj.SetActive(true);
					Plugin.LogSource.LogInfo($"Setting {obj.name} to active!");
				}
			}
		}
	}
}