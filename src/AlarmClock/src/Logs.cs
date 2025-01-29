using Verse;

namespace AlarmClock;

public static class LogPlus
{
    public static void Warning(string message)
    {
        Log.Warning($"AlarmClock: {message}");
    }
    public static void Error(string message)
    {
        Log.Error($"AlarmClock: {message}");
    }
}
