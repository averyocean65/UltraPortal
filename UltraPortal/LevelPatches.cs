using HarmonyLib;

namespace UltraPortal {
    [HarmonyPatch]
    public static class LevelPatches {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(FinalRank), nameof(FinalRank.SetInfo))]
        static void SetInfoPatch(ref FinalRank __instance, int restarts, bool damage, bool majorUsed, bool cheatsUsed) {
            Plugin.LogSource.LogInfo("Patching level ending!");
            if (PortalGunManager.EquippedPortalGun) {
                Plugin.LogSource.LogInfo("Portal gun was used!");
                __instance.extraInfo.text += "- <color=#FA0A56>PORTAL GUN USED</color>\n";
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.SaveRank))]
        static void SaveRankPatch(ref bool __runOriginal) {
            if (PortalGunManager.EquippedPortalGun) {
                // for reference: this blocks the actual GameProgressSaver.SaveRank function from running.
                __runOriginal = false;
            }
        }

        // Test if __runOriginal allows you to block the execution of a function.
        
        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(DynamicPortalExit), nameof(DynamicPortalExit.Initialize))]
        // static void InitializePatchTest(ref bool __runOriginal) {
        //     HudMessageReceiver.Instance.SendHudMessage("nuh uh");
        //     __runOriginal = false;
        // }
    }
}