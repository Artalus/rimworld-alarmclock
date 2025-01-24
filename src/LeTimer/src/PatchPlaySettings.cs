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

        bool openState = true;
        row.ToggleableIcon(
            ref openState,
            TexturesPlus.ToggleIcon,
            "LeTimer.ToggleWindow".Translate(),
            SoundDefOf.Mouseover_ButtonToggle
        );

        if (openState != lastOpenState)
        {
            lastOpenState = openState;
            // courtesy of simplechecklist
            var windowPresent = Find.WindowStack.IsOpen(typeof(TimersListWindow));
            switch (openState)
            {
                case true when !windowPresent:
                    Find.WindowStack.Add(new TimersListWindow());
                    break;
                case false when windowPresent:
                    Find.WindowStack.TryRemove(typeof(TimersListWindow));
                    break;
            }
        }
    }
}
