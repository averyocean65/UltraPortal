using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable] public static KeyCode SpawnEntryKeybind = KeyCode.U;
		[Configgable] public static KeyCode SpawnExitKeybind = KeyCode.I;
	}
}