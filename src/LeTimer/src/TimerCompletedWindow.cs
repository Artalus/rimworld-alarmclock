using UnityEngine;
using Verse;

namespace LeTimer;

// TODO: inherit from Dialog_Something maybe?
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
        this.doCloseX = false;
    }


    public override Vector2 InitialSize => new(200, 150);

    protected override void SetInitialSizeAndPosition()
    {
        // longer the description - wider the notification window
        const int characterWidth = 10;
        float adjustedWidth = Mathf.Clamp(
            // TODO: account for i18n diff between languages once translation kicks in
            50 + characterWidth * timerDesc.Length,
            InitialSize.x,
            // prevent ultra-long lines going across whole screen
            800
        );
        this.windowRect = new Rect(
            WindowPlus.MiddleScreenPos(InitialSize),
            new Vector2(adjustedWidth, InitialSize.y)
        );
    }

    public override void DoWindowContents(Rect rect)
    {
        int offsetX = 0;
        int offsetY = 0;
        DoLabels(offsetX, ref offsetY);
        // account for big CLOSE button at the end of window
        offsetY += (int)(Window.FooterRowHeight + GenUI.Gap);
        WindowPlus.ShrinkWindowHeightToContent(this, offsetY);
        windowRect.position = WindowPlus.MiddleScreenPos(windowRect);
    }

    private void DoLabels(int offsetX, ref int offsetY)
    {
        int x = offsetX + 10;
        Text.Font = GameFont.Medium;
        var r = new Rect(x, offsetY, windowRect.width, 30);
        Widgets.Label(r, "Time's up!");
        offsetY += 30;

        Text.Font = GameFont.Small;
        r = new Rect(offsetX, offsetY, windowRect.width, 20);
        Widgets.Label(r, $"Duration: {timerDuration}h");
        offsetY += 20;

        r = new Rect(offsetX, offsetY, windowRect.width, 40);
        Widgets.Label(r, $"Text: {timerDesc}");
    }
}
