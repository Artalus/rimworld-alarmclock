using HarmonyLib;
using RimWorld;
using Verse;

namespace LeTimer;

// rimhud: [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
[HarmonyPatch]
public static class RimWorld_PlaySettings_DoPlaySettingsGlobalControls
{
    private static bool lastOpenState = true;

    // the list of gray toggle buttons in bottom-right corner
    [HarmonyPatch(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls))]
    [HarmonyPostfix]
    private static void DoPlaySettingsGlobalControls_Postfix(WidgetRow? row, bool worldView)
    {
        if (worldView || row is null)
            return;
        Log.Message("harmony! ooohh!");
    }
}
