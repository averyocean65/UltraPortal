using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltraPortal {
	public class LayerPrinter : MonoBehaviour {
		private List<int> _collectedLayers = new List<int>();
		private Dictionary<int, List<GameObject>> _objectMap = new Dictionary<int, List<GameObject>>();
		
		private void GatherChildren(Transform root) {
			for (int i = 0; i < root.childCount; i++) {
				GameObject child = transform.GetChild(i).gameObject;
				AddLayer(child.layer, child);
			}
		}
		
		private void Start() {
			_collectedLayers = new List<int>();
			_objectMap = new Dictionary<int, List<GameObject>>();
			
			AddLayer(gameObject.layer, gameObject);
			GatherChildren(transform);

			Plugin.LogSource.LogInfo($"-- LAYERS IN {gameObject.name} --");
			foreach (int layer in _collectedLayers) {
				Plugin.LogSource.LogInfo($"- {LayerMask.LayerToName(layer)} -");
				
				foreach(GameObject obj in _objectMap[layer])
				{
					Plugin.LogSource.LogInfo($"{obj.name}");
				}
			}
		}

		private void AddLayer(int layer, GameObject obj) {
			if (_collectedLayers.Contains(layer)) {
				List<GameObject> array = _objectMap[layer];
				array.Add(obj);
				return;
			}
			
			_collectedLayers.Add(layer);
			_objectMap.Add(layer, new List<GameObject>());
		}
	}
}