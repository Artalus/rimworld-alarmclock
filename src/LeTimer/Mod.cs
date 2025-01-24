using Verse;

namespace LeTimer;

// inits when ready to show mainmenu
[StaticConstructorOnStartup]
public static class Startup
{
    static Startup()
    {
        var harmony = new HarmonyLib.Harmony("artalus.letimer");
        harmony.PatchAll();
        Log.Message("LeTimer: initialized");
    }
}
// inits during startups
// TODO: is this even needed if we dont access it anywhere?
public sealed class Mod : Verse.Mod
{
    public const string Id = "LeTimer";
    public const string Name = "LeTimer";
    public const string Version = "0.0.0";
    public const string WorkshopLink = "...";

    private static Mod? _instance;
    public static ModContentPack ContentPack => _instance!.Content;

    public Mod(ModContentPack content) : base(content)
    {
        _instance = this;
    }
}
