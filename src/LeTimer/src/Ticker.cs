using Verse;

namespace LeTimer;

/// <summary>
/// small wrapper to get game ticks, just to not mistake the 2 ingame counters
/// </summary>
public static class Ticker
{
    public static int Now
    {
        get
        {
            // TicksGame counts from 0 - literal amount of time since game started
            // TicksAbs takes into account that game starts on *some* day of the year, 5500-01-01
            return Find.TickManager.TicksAbs;
        }
    }
}
