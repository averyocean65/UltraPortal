using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Keybinds", "Portal Gun Slot")] 
		public static KeyCode PortalGunKeybind = KeyCode.Alpha7;
	}
}