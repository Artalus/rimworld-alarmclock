using System;
using UnityEngine;
using Verse;

namespace LeTimer;

public class TimerCompletedWindow : Window
{
    private string timerDesc = "";
    private int timerDuration = 0;
    public TimerCompletedWindow(TimerEntry timer)
    {
        timerDuration = timer.DurationHours;
        timerDesc = timer.Description;

        this.layer = WindowLayer.GameUI;
        this.draggable = false;
        this.doWindowBackground = true;
        this.drawShadow = true;
        this.closeOnClickedOutside = false;
        this.preventCameraMotion = false;
        this.onlyOneOfTypeAllowed = false;
        this.drawInScreenshotMode = false;
        this.focusWhenOpened = true;
        this.forcePause = true;
        // autogenerate "close" button and dont close with other means
        this.doCloseButton = true;
        this.closeOnAccept = false;
        this.closeOnCancel = false;
    }


    public override Vector2 InitialSize => new(200, 150);

    protected override void SetInitialSizeAndPosition()
    {
        // middle of the screen
        var position = new Vector2(
            (UI.screenWidth - InitialSize.x) / 2,
            (UI.screenHeight - InitialSize.y) / 2
        );
        // longer the description - wider the notification window
        const int characterWidth = 10;
        float adjustedWidth = Math.Max(
            InitialSize.x,
            // TODO: account for label's label name once translation kicks in
            timerDesc.Length * characterWidth + 50
        );
        this.windowRect = new Rect(position, new Vector2(adjustedWidth, InitialSize.y)).Rounded();
    }

    public override void DoWindowContents(Rect rect)
    {
        // TODO: is this *really* how gui in rw works?..
        Rect r = rect;
        r.x += 10;
        Text.Font = GameFont.Medium;
        Widgets.Label(r, "Time's up!");
        r = rect;
        r.y += 30;

        Text.Font = GameFont.Small;
        Widgets.Label(r, $"Duration: {timerDuration}h");
        r.y += 20;

        // growing width into +infinity ensures no text wraps are used...
        r.width = 999;
        Widgets.Label(r, $"Label: {timerDesc}");
    }
}
