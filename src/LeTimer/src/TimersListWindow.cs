using UnityEngine;
using Verse;

namespace LeTimer;

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
        this.windowRect = new Rect(
            controller.WindowX, controller.WindowY,
            InitialSize.x, InitialSize.y
        );
    }

    public override void DoWindowContents(Rect rect)
    {
        int offsetY = 0;
        DoEntriesList(ref offsetY);
        // skip some space after last entry
        offsetY += 8;
        HandleButton(ref offsetY);
        HandleWindowDimensions(offsetY);
        WindowPlus.ShrinkWindowHeightToContent(this, offsetY);
    }

    /// <summary>
    /// adding a new entry heightens window and moves it up a bit, while removing
    /// shrinks it and moves down - maintaining the "add" button at the very bottom
    /// </summary>
    private void HandleWindowDimensions(int newHeight)
    {
        int heightDiff = newHeight - (int)windowRect.height;
        if (Mathf.Abs(heightDiff) <= 0)
            return;

        var newY = windowRect.position.y - heightDiff;
        // TODO: implement height limits? or just forbid adding new timers if too many?
        windowRect.position = new Vector2(windowRect.position.x, newY);

        // preserve position in the controller, so dragging the window does not
        // reset after it's closed in playsettings
        // ...if only there was a PostDrag or smth((
        controller.WindowX = windowRect.position.x;
        controller.WindowY = windowRect.position.y;
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
        float x1 = 0;
        // draw anchor for easier dragging
        var dragRect = new Rect(x1, offsetY, GenUI.SmallIconSize, GenUI.SmallIconSize);
        TooltipHandler.TipRegion(dragRect, "Drag the window");
        GUI.DrawTexture(dragRect, TexturesPlus.DragHandle);

        float x2 = GenUI.SmallIconSize;
        var btnRect = new Rect(x2, offsetY, GenUI.SmallIconSize, GenUI.SmallIconSize);
        if (Widgets.ButtonImage(btnRect, TexButton.Add, Color.white, tooltip: "Add new timer"))
        {
            Find.WindowStack.Add(new EditEntryWindow(controller, null));
        }
        offsetY += (int)GenUI.SmallIconSize;
    }
}
