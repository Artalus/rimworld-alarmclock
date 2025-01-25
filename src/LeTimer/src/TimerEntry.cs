using RimWorld;
using Verse;

namespace LeTimer;

public class TimerEntry // : IExposable
{
    const int TICKS_IN_HOUR = 2500;
    public const int EXPIRATION_HOURS = 24;
    const int EXPIRATION_TIMEOUT = EXPIRATION_HOURS * TICKS_IN_HOUR;

    /// <summary>
    /// user-defined name for this timer
    /// </summary>
    public string Description;

    /// <summary>
    /// time till the timer completes
    /// </summary>
    public int DurationHours { get; private set; }

    /// <summary>
    /// user-friendly label with time till completion; 3q / 2d / 13h ...
    /// </summary>
    public string CompletesInLabel { get; private set; }

    /// <summary>
    /// after which game tick the timer completes
    /// </summary>
    public int CompletesOnTick { get; private set; }

    /// <summary>
    /// after which game tick the entry should be dropped from history
    /// </summary>
    private int expiresOnTick;

    public bool IsExpired()
    {
        var tick = Find.PlayLog!.LastTick;
        return tick > expiresOnTick;
    }

    public TimerEntry()
    {
        Description = "";
        CompletesInLabel = "";
        CompletesOnTick = 0;
        expiresOnTick = 0;
    }

    public TimerEntry(string label, int hours)
    {
        Description = label;
        // ="" is unnecessary, as it inits inside updates - but compiler warns otherwise
        CompletesInLabel = "";
        UpdateDuration(hours);
    }

    // TODO: convert ticks to absolute datetimes to save timers into savegame file
    // public void ExposeData()
    // {
    //     Scribe_Values.Look(ref Label, "Label");
    //     Scribe_Values.Look(ref Completed, "CompletesOnTick");
    // }

    public void UpdateDuration(int hours)
    {
        var tick = Find.PlayLog!.LastTick;
        this.CompletesOnTick = tick + (hours * TICKS_IN_HOUR);
        expiresOnTick = CompletesOnTick + EXPIRATION_TIMEOUT;
        UpdateExpirationLabel(tick);
    }

    /// <summary>
    /// should be called periodically from outside, when said outside decides to
    /// refresh displayed data
    /// </summary>
    public void UpdateExpirationLabel(int currentTick)
    {
        this.CompletesInLabel = CalcExpirationLabel(currentTick);
    }

    /// <summary>
    /// get readable expiration string from current tick and stored completion
    /// </summary>
    private string CalcExpirationLabel(int currentTick)
    {
        const int TICKS_IN_DAY = TICKS_IN_HOUR * 24;
        const int TICKS_IN_QUANDRUM = TICKS_IN_DAY * 15;
        var ticksLeft = CompletesOnTick - currentTick;
        return ticksLeft switch
        {
            < TICKS_IN_HOUR / 10 => "now!",
            < TICKS_IN_HOUR => "<1h",
            < TICKS_IN_DAY => $"{ticksLeft / TICKS_IN_HOUR}h",
            < TICKS_IN_QUANDRUM => $"{ticksLeft / TICKS_IN_DAY}d",
            _ => $"{ticksLeft / TICKS_IN_QUANDRUM}q"
        };
    }
}
