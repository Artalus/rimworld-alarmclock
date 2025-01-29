using HarmonyLib;
using RimWorld;
using Verse;

namespace AlarmClock;

// rimhud: [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
[HarmonyPatch]
public static class RimWorld_PlaySettings_DoPlaySettingsGlobalControls
{
    // the list of gray toggle buttons in bottom-right corner
    [HarmonyPatch(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls))]
    [HarmonyPostfix]
    private static void DoPlaySettingsGlobalControls_Postfix(WidgetRow? row, bool worldView)
    {
        if (worldView || row is null)
            return;

        var component = Current.Game.GetComponent<TimersListController>();
        bool openState = component.WindowVisible;
        row.ToggleableIcon(
            ref openState,
            TexturesPlus.ToggleIcon,
            "LeTimer.ToggleWindow".Translate(),
            SoundDefOf.Mouseover_ButtonToggle
        );
        component.ToggleWindow(openState);
    }
}
