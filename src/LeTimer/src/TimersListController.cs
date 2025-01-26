using System.Collections.Generic;
using Verse;

namespace LeTimer;

public class TimersListController : GameComponent
{
    // simulate LongTick unavailable to GameComponent; 1000/60 ~= 17sec on normal speed
    private const int TIMERS_UPDATE_INTERVAL_TICKS = 1000;
    private int _nextUpdateTick = 0;

    /// <summary>
    /// timers created by user; will be regularly updated
    /// </summary>
    public List<TimerEntry> Timers = [];
    private TimersListWindow? _window;

    /// <summary>
    /// write-only helper for serialization; should never be read outside of
    /// ToggleWindow and ExposeData
    /// </summary>
    private bool _windowVisible = false;
    /// <summary>
    /// write-only helper for serialization; should never be read outside of ExposeData
    /// </summary>
    public float WindowX = 100;
    /// <summary>
    /// write-only helper for serialization; should never be read outside of ExposeData
    /// </summary>
    public float WindowY = 200;

    public TimersListController(Game game)
    {
        // necessary even when does nothing
    }

    public override void GameComponentTick()
    {
        var tick = Ticker.Now;
        if (tick < _nextUpdateTick)
            return;

        _nextUpdateTick = tick + TIMERS_UPDATE_INTERVAL_TICKS;
        UpdateTimers(tick);
    }

    /// <summary>
    /// update all timers, creating notifications and removing expired entries as needed
    /// </summary>
    private void UpdateTimers(int currentTick)
    {
        foreach (var timer in Timers)
        {
            timer.UpdateLabels(currentTick);
            if (timer.Completed)
                continue;
            if (currentTick >= timer.CompletesOnTick)
            {
                timer.Completed = true;
                Find.WindowStack.Add(new TimerCompletedWindow(timer));
            }
        }
        int removed = Timers.RemoveAll(x => x.ShouldExpire());
    }

    public bool WindowVisible { get { return _windowVisible; } }

    /// <summary>
    /// whether the window for this component is displayed; controlled by
    /// DoPlaySettingsGlobalControls
    /// </summary>
    public void ToggleWindow(bool value)
    {
        _windowVisible = value;
        bool visible = this._window is not null;
        if (value == visible)
            return;

        if (value)
        {
            _window = new TimersListWindow(this);
            Find.WindowStack.Add(_window);
        }
        else
        {
            _window = null;
            bool removed = Find.WindowStack.TryRemove(typeof(TimersListWindow));
            if (!removed)
                Log.Error("LeTimer: could not remove List window");
        }
    }

    public void AddNewTimer(string description, int hours)
    {
        Timers.Add(new(description, hours));
        Timers.SortBy(x => x.CompletesOnTick);
    }

    public void EditTimerAt(int index, string description, int hours)
    {
        var x = Timers[index];
        x.Description = description;
        x.ResetTime(hours);
        Timers.SortBy(x => x.CompletesOnTick);
    }

    public void RemoveTimerAt(int index)
    {
        Timers.RemoveAt(index);
    }

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref Timers, "Items", LookMode.Deep, []);
        Scribe_Values.Look(ref WindowX, "WindowX");
        Scribe_Values.Look(ref WindowY, "WindowY");
        Scribe_Values.Look(ref _windowVisible, "WindowVisible");
        base.ExposeData();
    }

    public override void FinalizeInit()
    {
        // entries do not store their labels, so need to reset those
        // cannot do this right here - map component might not be available - so use queue
        LongEventHandler.QueueLongEvent(() =>
        {
            foreach (var item in Timers)
            {
                item.UpdateLabels(Ticker.Now, true);
            }
        },
            "LeTimerLabelsUpdate",
            doAsynchronously: false,
            exceptionHandler: null
        );
        ToggleWindow(_windowVisible);
        base.FinalizeInit();
    }
}
