using RimWorld;
using Verse;

namespace AlarmClock;

public class TimerEntry : IExposable
{
    public const int EXPIRATION_HOURS = 24;
    const int EXPIRATION_TIMEOUT = EXPIRATION_HOURS * GenDate.TicksPerHour;

    /// <summary>
    /// user-defined name for this timer
    /// </summary>
    public string Description = "";

    /// <summary>
    /// time till the timer completes
    /// </summary>
    public int DurationHours { get; private set; } = 0;

    /// <summary>
    /// user-friendly label with initial timer duration; 1y / 2d / 13h ...
    /// </summary>
    public string LabelDuration { get; private set; } = "";

    /// <summary>
    /// user-friendly label with time untill completion
    /// </summary>
    public string LabelCompletesIn { get; private set; } = "";

    /// <summary>
    /// user-friendly label with date upon which timer completes
    /// </summary>
    public string LabelCompletesAt { get; private set; } = "";

    /// <summary>
    /// user-friendly label with time untill timer removal
    /// </summary>
    public string LabelExpiresIn { get; private set; } = "";

    /// <summary>
    /// after which game tick the timer completes
    /// </summary>
    public int CompletesOnTick { get; private set; } = 0;

    public bool Completed = false;

    /// <summary>
    /// after which game tick the entry should be dropped from history
    /// </summary>
    private int expiresOnTick = 0;

    public TimerEntry()
    {
        ResetTime(0);
    }

    public TimerEntry(string description, int hours)
    {
        Description = description;
        ResetTime(hours);
    }

    public bool ShouldExpire()
    {
        return Completed && Ticker.Now > expiresOnTick;
    }

    public TimerEntry(TimerEntry other)
    {
        Description = other.Description;
        DurationHours = other.DurationHours;
        CompletesOnTick = other.CompletesOnTick;
        Completed = other.Completed;
        expiresOnTick = other.expiresOnTick;
        UpdateLabels(Ticker.Now, true);
    }

    public void ResetTime(int hours)
    {
        DurationHours = hours;
        Completed = false;
        var tick = Ticker.Now;
        this.CompletesOnTick = tick + (hours * GenDate.TicksPerHour);
        expiresOnTick = tick + EXPIRATION_TIMEOUT;
        UpdateLabels(tick, true);
    }

    /// <summary>
    /// should be called periodically from outside, when said outside decides to
    /// refresh displayed data
    /// </summary>
    public void UpdateLabels(int currentTick, bool durationUpdated = false)
    {
        this.LabelCompletesIn = FormatDurationString(CompletesOnTick - currentTick);
        this.LabelExpiresIn = FormatDurationString(expiresOnTick - currentTick);
        if (!durationUpdated)
            return;

        this.LabelDuration = FormatDurationString(DurationHours * GenDate.TicksPerHour);
        // CurrentMap may not exist at the very first update - and also while on world map
        // also this is purely for display, as switching between times involves timezones
        if (Current.Game.CurrentMap == null)
        {
            this.LabelCompletesAt = "??";
        }
        else
        {
            this.LabelCompletesAt = GenDate.DateFullStringWithHourAt(
                CompletesOnTick,
                Find.WorldGrid.LongLatOf(Current.Game.CurrentMap.Tile)
            );
        }
    }

    private static string FormatDurationString(int durationTicks)
    {
        return durationTicks switch
        {
            // TODO: maybe use one of GenDate.ToString* to get free translation
            < 0 => "✓",
            < GenDate.TicksPerHour / 10 => "‼",
            < GenDate.TicksPerHour => "<1h",
            < GenDate.TicksPerDay => $"{durationTicks / GenDate.TicksPerHour}h",
            < GenDate.TicksPerYear => $"{durationTicks / GenDate.TicksPerDay}d",
            _ => $"{durationTicks / GenDate.TicksPerYear}y"
        };
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref expiresOnTick, "ExpiresOnTick");
        Scribe_Values.Look(ref Completed, "Completed");
        // without default value string gets treated as nullable
        Scribe_Values.Look(ref Description, "Description", "");
        // ref & scribe does not support properties(
        int tmp = DurationHours;
        Scribe_Values.Look(ref tmp, "Duration");
        DurationHours = tmp;
        tmp = CompletesOnTick;
        Scribe_Values.Look(ref tmp, "CompletesOnTick");
        CompletesOnTick = tmp;
    }
}
