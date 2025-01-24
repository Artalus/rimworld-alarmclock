using UnityEngine;
using Verse;

namespace LeTimer;

public class TimersListWindow : Window
{
    public TimersListWindow()
    {
        this.draggable = true;
        this.closeOnAccept = false;
        this.closeOnCancel = false;
        this.closeOnClickedOutside = false;
        this.doCloseButton = false;
        this.drawShadow = false;
        this.preventCameraMotion = false;
        this.onlyOneOfTypeAllowed = true;
        this.layer = WindowLayer.GameUI;
        this.doWindowBackground = true;
        this.drawInScreenshotMode = false;
        this.focusWhenOpened = false;
    }

    protected override void SetInitialSizeAndPosition()
    {
        this.windowRect = new Rect(100, 200, 300, 400);
    }

    public override void DoWindowContents(Rect rect1)
    {
    }
}

