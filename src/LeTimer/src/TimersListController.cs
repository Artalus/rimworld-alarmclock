using System.Collections.Generic;
using Verse;

namespace LeTimer;

public class TimersListController : GameComponent
{
    // simulate LongTick unavailable to GameComponent; 2000/60 ~= 33sec
    private const int TIMERS_UPDATE_INTERVAL_TICKS = 2000;
    private int _nextUpdateTick = 0;

    /// <summary>
    /// timers created by user; will be regularly updated
    /// </summary>
    public List<TimerEntry> Items = [];
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
        var tick = Find.PlayLog!.LastTick;
        if (tick < _nextUpdateTick)
            return;

        _nextUpdateTick = tick + TIMERS_UPDATE_INTERVAL_TICKS;
        UpdateTimers(tick);
    }

    /// <summary>
    /// update all timers, creating notifications and removing expired entries as needed
    private void UpdateTimers(int currentTick)
    {
        Log.Message($"LeTimer: update @ {currentTick}");
        foreach (var item in Items)
        {
            item.UpdateExpirationLabel(currentTick);
            if (currentTick >= item.CompletesOnTick)
            {
                Find.WindowStack.Add(new TimerCompletedWindow(item));
                continue;
            }
        }
        int removed = Items.RemoveAll(x => x.IsExpired());
        Log.Message($"LeTimer: removed {removed} expired entries");
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

    public override void FinalizeInit()
    {
        // ChecklistWindow.UpdateItemCount(Items.Count);
        ToggleWindow(_windowVisible);
        // FIXME: REMOVE for tests only
        Items.Add(new TimerEntry("test label to check length calculations", 1));

        base.FinalizeInit();
    }
}
