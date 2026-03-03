using UnityEngine;

namespace UltraPortal.Extensions {
	public static class LayerMaskExtensions {
		// from: https://gist.github.com/unitycoder/cb887da3be89458968101a92cf61720b
		public static bool Contains(this LayerMask mask, int layer)
		{
			return (0 != (mask & (1 << layer)));
		}
	}
}