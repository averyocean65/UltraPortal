using ULTRAKILL.Portal;
using UltraPortal.Shared;

namespace UltraPortal.Extensions {
	public static class PortalExtensions {
		public static bool IsUltraPortal(this PortalHandle handle) {
			return PortalUtils.GetPortalObject(handle).IsUltraPortal();
		}

		public static bool IsUltraPortal(this Portal portal) {
			return portal.entry.TryGetComponent<PortalInfo>(out var info);
		}
	}
}