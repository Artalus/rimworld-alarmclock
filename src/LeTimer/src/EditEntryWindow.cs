using RimWorld;
using UnityEngine;
using Verse;

namespace LeTimer;

public class EditEntryWindow : Window
{

    private TimersListController controller;
    private TimerEntry entrySubstitute;
    private int? targetEntryIndex = null;
    // data storage for sliders (so have to be floats)
    private float _durationHours = 0;
    private float _durationDays = 0;

    /// <summary>
    /// if entryIndex>=0, assume we are editing existing item
    /// </summary>
    public EditEntryWindow(TimersListController c, int? entryIndex)
    {
        this.layer = WindowLayer.GameUI;
        this.draggable = false;
        this.forcePause = true;
        // allow exit on esc and x; accept requires validation
        this.doCloseX = true;
        this.closeOnAccept = false;
        this.closeOnCancel = true;
        this.closeOnClickedOutside = false;
        this.doCloseButton = false;

        this.drawShadow = false;
        this.preventCameraMotion = true;
        this.onlyOneOfTypeAllowed = true;
        this.doWindowBackground = true;
        this.absorbInputAroundWindow = true;
        this.drawInScreenshotMode = false;
        this.focusWhenOpened = true;

        controller = c;
        if (!entryIndex.HasValue)
        {
            entrySubstitute = new TimerEntry();
            return;
        }
        targetEntryIndex = entryIndex;
        entrySubstitute = new TimerEntry(controller.Timers[entryIndex.Value]);
        // float for easier type conversions
        float diff = entrySubstitute.CompletesOnTick - Ticker.Now;
        if (diff > 0)
        {
            _durationHours = Mathf.RoundToInt((diff % GenDate.TicksPerDay) / GenDate.TicksPerHour);
            _durationDays = Mathf.FloorToInt(diff / GenDate.TicksPerDay);
        }
    }

    TimerEntry? Target
    {
        get
        {
            if (targetEntryIndex.HasValue)
                return controller.Timers[targetEntryIndex.Value];
            return null;
        }
    }

    public override Vector2 InitialSize => new(450, 0);

    protected override void SetInitialSizeAndPosition()
    {
        // middle of the screen
        var position = new Vector2(
            (UI.screenWidth - InitialSize.x) / 2,
            (UI.screenHeight - InitialSize.y) / 2
        );

        this.windowRect = new Rect(position, InitialSize);
    }

    public override void DoWindowContents(Rect rect)
    {
        Text.Font = GameFont.Small;
        int offsetY = 0;
        HandleControls(ref offsetY);
        HandleHintLabels(ref offsetY);
        bool shouldClose = HandleButtons(ref offsetY);
        WindowPlus.ShrinkWindowHeightToContent(this, offsetY);
        if (shouldClose)
        {
            if (!Find.WindowStack.TryRemove(this))
                Log.Error("LeTimer: could not remove EditEntryWindow");
        }
    }

    private void HandleControls(ref int offsetY)
    {
        Rect r = new Rect(0, offsetY, windowRect.width, WindowPlus.LABEL_HEIGHT_SM);
        Widgets.Label(r, "Description:");
        offsetY += WindowPlus.LABEL_HEIGHT_SM;

        float availableWidth = WindowPlus.AvailableWidth(this);
        r = new Rect(0, offsetY, availableWidth, WindowPlus.TEXTFIELD_HEIGHT_SM);
        entrySubstitute.Description = Widgets.TextField(r, entrySubstitute.Description);
        offsetY += WindowPlus.TEXTFIELD_HEIGHT_SM;

        float hoursWidth = availableWidth * 0.4f;
        float slidersGap = availableWidth * 0.1f;
        float daysWidth = availableWidth * 0.5f;

        float x = 0;
        var date = (string x) => x.Colorize(ColoredText.DateTimeColor);
        Widgets.HorizontalSlider(
            new Rect(x, offsetY, hoursWidth, 50),
            ref _durationHours,
            new FloatRange(0, GenDate.HoursPerDay),
            "Hours: " + date(_durationHours.ToString()),
            roundTo: 1
        );
        x += hoursWidth + slidersGap / 2;
        Widgets.DrawLineVertical(x, offsetY + 10, 20);
        x += slidersGap / 2;
        Widgets.HorizontalSlider(
            new Rect(x, offsetY, daysWidth, 50),
            ref _durationDays,
            new FloatRange(0, GenDate.DaysPerYear),
            "Days: " + date(_durationDays.ToString()),
            roundTo: 1
        );
        x = 0;
        offsetY += 50;

        // avoid recalculating labels every frame!
        var newDuration = (int)(_durationHours + GenDate.HoursPerDay * _durationDays);
        if (newDuration != entrySubstitute.DurationHours)
            entrySubstitute.ResetTime(newDuration);
    }

    private void HandleHintLabels(ref int offsetY)
    {
        string firesAt = entrySubstitute.LabelCompletesAt;
        string firesIn = entrySubstitute.LabelCompletesIn;
        int firesInTicks = entrySubstitute.CompletesOnTick - Ticker.Now;
        var warn = (string x) => x.Colorize(ColoredText.WarningColor);
        var gray = (string x) => x.Colorize(ColoredText.SubtleGrayColor);
        var date = (string x) => x.Colorize(ColoredText.DateTimeColor);

        Widgets.Label(
            new Rect(0, offsetY, windowRect.width, WindowPlus.LABEL_HEIGHT_SM),
            gray("Will fire: at ") + date(firesAt) + gray(" / in ")
            + date($"{firesIn} ({firesInTicks} ticks)")
        );
        offsetY += WindowPlus.LABEL_HEIGHT_SM;

        if (Target != null)
        {
            string oldAt = Target.LabelCompletesAt;
            string oldIn = Target.LabelCompletesIn;
            int oldTicks = Target.CompletesOnTick - Ticker.Now;
            Widgets.Label(
                new Rect(0, offsetY, windowRect.width, WindowPlus.LABEL_HEIGHT_SM),
                gray($"Old values: at {oldAt} / in {oldIn} ({oldTicks})")
            );
            offsetY += WindowPlus.LABEL_HEIGHT_SM;

            if (Target.Completed)
            {
                Widgets.Label(
                    new Rect(0, offsetY, windowRect.width, WindowPlus.LABEL_HEIGHT_SM),
                    warn("Completed timer; would have been removed in ")
                    + date(Target.LabelExpiresIn)
                );
                offsetY += WindowPlus.LABEL_HEIGHT_SM;
            }
        }
    }

    private bool HandleButtons(ref int offsetY)
    {
        float availableWidth = WindowPlus.AvailableWidth(this);

        string timerName = Target == null
            ? "new timer"
            : $"timer '{Target.Description.Truncate(100)}'"
            ;
        string validationTooltip = Target == null
            ? $"Add {timerName}"
            : $"Reset {timerName}"
            ;

        bool inputValid = entrySubstitute.Description.Length > 0
            && entrySubstitute.DurationHours > 0;
        if (entrySubstitute.Description.Length <= 0)
            validationTooltip = "Needs description";
        if (entrySubstitute.DurationHours <= 0)
            validationTooltip = "Duration must be greater than 0h";

        Color mainColor = inputValid ? Color.white : Widgets.InactiveColor;
        Color mouseoverColor = inputValid ? GenUI.MouseoverColor : Widgets.InactiveColor;

        // first draw rightmost "delete" button - then "save" to the left
        float x = availableWidth - GenUI.SmallIconSize;
        float y = offsetY;
        offsetY += (int)GenUI.SmallIconSize;

        if (targetEntryIndex.HasValue)
        {
            bool deletePressed = Widgets.ButtonImage(
                new Rect(x, y, GenUI.SmallIconSize, GenUI.SmallIconSize),
                TexButton.Delete,
                Color.white,
                tooltip: $"Delete {timerName}"
            );
            if (deletePressed)
            {
                controller.RemoveTimerAt(targetEntryIndex.Value);
                return true;
            }
            // small usability gap between buttons
            x -= (16 + GenUI.SmallIconSize);
        }

        bool savePressed = Widgets.ButtonImage(
            new Rect(x, y, GenUI.SmallIconSize, GenUI.SmallIconSize),
            TexButton.Save,
            mainColor,
            mouseoverColor,
            doMouseoverSound: inputValid,
            tooltip: validationTooltip
        );
        // make button ignore clicks if input is invalid
        // do it *after* button() call, otherwisee bool&& would shortcircuit on first `false`
        if (savePressed && inputValid)
        {
            SaveToController();
            return true;
        }
        return false;
    }

    private void SaveToController()
    {
        if (targetEntryIndex.HasValue)
            controller.EditTimerAt(targetEntryIndex.Value, entrySubstitute.Description, entrySubstitute.DurationHours);
        else
            controller.AddNewTimer(entrySubstitute.Description, entrySubstitute.DurationHours);
    }
}
