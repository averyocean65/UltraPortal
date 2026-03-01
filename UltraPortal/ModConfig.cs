using Configgy;
using UnityEngine;

namespace UltraPortal {
	public static class ModConfig {
		[Configgable("Keybinds", "Spawn First Portal Key")] public static KeyCode SpawnEntryKeybind = KeyCode.U;
		[Configgable("Keybinds", "Spawn Second Portal Key")] public static KeyCode SpawnExitKeybind = KeyCode.I;
	}
}