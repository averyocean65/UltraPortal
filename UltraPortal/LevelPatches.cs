using HarmonyLib;

namespace UltraPortal {
    [HarmonyPatch]
    public static class LevelPatches {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.SetInfo))]
        static void SetInfoPatch(FinalRank __instance, int restarts, bool damage, bool majorUsed, bool cheatsUsed) {
            Plugin.LogSource.LogInfo("Patching level ending!");
            if (PortalGunManager.EquippedPortalGun) {
                Plugin.LogSource.LogInfo("Portal gun was used!");
                __instance.extraInfo.text += "- <color=#4C99E6>PORTAL GUN USED</color>\n";
            }
        }
    }
}