using UnityEngine;
using Verse;

namespace AlarmClock;

// FIXME: use UIScaling.AdjustRectToUIScaling everywhere!
// or it won't work for peeps with zoomed ui

public class TimersListWindow : Window
{
    TimersListController controller;
    public TimersListWindow(TimersListController controller)
    {
        this.controller = controller;
        this.draggable = true;
        this.forcePause = false;
        this.closeOnAccept = false;
        this.closeOnCancel = false;
        this.closeOnClickedOutside = false;
        this.doCloseButton = false;
        this.drawShadow = false;
        this.preventCameraMotion = false;
        this.onlyOneOfTypeAllowed = true;
        this.layer = WindowLayer.GameUI;
        this.doWindowBackground = false;
        this.drawInScreenshotMode = false;
        this.focusWhenOpened = false;
    }


    public override Vector2 InitialSize => new(300, 0);

    protected override void SetInitialSizeAndPosition()
    {
        this.windowRect = new Rect(controller.WindowPos, InitialSize);
    }

    public override void DoWindowContents(Rect rect)
    {
        int offsetY = 0;
        HandleButton(ref offsetY);
        offsetY += 8;
        DoEntriesList(ref offsetY);
        WindowPlus.ShrinkWindowHeightToContent(this, offsetY);
        HandleWindowPosition();
    }

    /// <summary>
    /// ensure window remains on screen at least partially in bottom-right corner
    /// </summary>
    private void HandleWindowPosition()
    {
        // TODO: implement height limits? or just forbid adding new timers if too many?
        windowRect.position = ClampWindowPosToScreen(windowRect.position.x, windowRect.position.y);

        // preserve position in the controller, so dragging the window does not
        // reset after it's closed in playsettings
        // ...if only there was a PostDrag or smth((
        controller.WindowPos = windowRect.position;
    }

    public static Vector2 ClampWindowPosToScreen(float x, float y)
    {
        const int marginTopLeft = 5;
        const int marginBottomRight = (int)GenUI.Gap * 3;
        return new(
            Mathf.Clamp(x, marginTopLeft, UI.screenWidth - marginBottomRight),
            Mathf.Clamp(y, marginTopLeft, UI.screenHeight - marginBottomRight)
        );
    }

    static readonly Color[] _entiresBg = [
        new(0.0f, 0.0f, 0.0f, 0.2f),
        new(0.5f, 0.5f, 0.5f, 0.13f),
    ];
    static readonly Color _entriesHigh = new(.1f, .3f, .5f, .3f);
    private void DoEntriesList(ref int offsetY)
    {
        const int entryHeight = WindowPlus.LABEL_HEIGHT_SM;

        Text.Font = GameFont.Small;
        int index = -1;
        foreach (var item in controller.Timers)
        {
            ++index;
            var r = new Rect(0, offsetY + entryHeight * index, windowRect.width, entryHeight);
            var bgColor = Mouse.IsOver(r) ? _entriesHigh : _entiresBg[index % 2];
            Widgets.DrawBoxSolid(r, bgColor);
            Widgets.Label(r, $"({item.LabelCompletesIn}) {item.Description}".Truncate(windowRect.width - GenUI.Gap * 2));
            if (Widgets.ButtonInvisible(r))
            {
                Find.WindowStack.Add(new EditEntryWindow(controller, index));
            }
        }
        offsetY += entryHeight * controller.Timers.Count;
    }

    private void HandleButton(ref int offsetY)
    {
        float x = 0;
        // draw anchor for easier dragging
        var dragRect = new Rect(x, offsetY, GenUI.SmallIconSize, GenUI.SmallIconSize);
        TooltipHandler.TipRegion(dragRect, "AlarmClock.WindowList.TipDrag".Translate());
        GUI.DrawTexture(dragRect, TexturesPlus.DragHandle);

        x += GenUI.SmallIconSize;
        var btnRect = new Rect(x, offsetY, GenUI.SmallIconSize, GenUI.SmallIconSize);
        if (Widgets.ButtonImage(btnRect, TexButton.Add, Color.white, tooltip: "AlarmClock.WindowList.TipAdd".Translate()))
        {
            Find.WindowStack.Add(new EditEntryWindow(controller, null));
        }

        x += 8 + GenUI.SmallIconSize;
        int w = WindowPlus.AvailableWidth(this, (int)x);
        var lblRect = new Rect(x, offsetY, w, WindowPlus.LABEL_HEIGHT_SM);
        Widgets.Label(lblRect, "AlarmClock.WindowList.Title".Translate());

        offsetY += (int)GenUI.SmallIconSize;
    }
}
