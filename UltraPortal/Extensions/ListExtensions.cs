using System.Collections.Generic;
using static UltraPortal.DebugUtils;

namespace UltraPortal.Extensions {
	public static class ListExtensiosn {
		public static void SafeAdd<T>(this List<T> list, T element) {
			if (element == null) {
				return;
			}

			if (list == null) {
				LogError("List is null!");
				return;
			}

			if (list.Contains(element)) {
				return;
			}
			
			list.Add(element);
		}
		
		public static void SafeRemove<T>(this List<T> list, T element) {
			if (element == null) {
				return;
			}
			
			if (list == null) {
				LogError("List is null!");
				return;
			}

			if (!list.Contains(element)) {
				return;
			}
			
			list.Remove(element);
		}
	}
}