using UnityEngine;

namespace UltraPortal.Extensions {
	public static class LayerMaskExtensions {
		// from: https://discussions.unity.com/t/how-do-i-check-what-an-objects-layer-mask-is-in-an-if-statement/888160/4
		public static bool Contains(this LayerMask val, int layer)
		{
			return ((val & (1<<layer))>0);
		}
	}
}