using UnityEngine;
using Verse;

namespace LeTimer;

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
        this.doWindowBackground = true;
        this.drawInScreenshotMode = false;
        this.focusWhenOpened = false;
    }


    public override Vector2 InitialSize => new(300, 400);

    protected override void SetInitialSizeAndPosition()
    {
        this.windowRect = new Rect(
            controller.WindowX, controller.WindowY,
            InitialSize.x, InitialSize.y
        );
    }

    public override void DoWindowContents(Rect rect1)
    {
        // preserve position after drag
        controller.WindowX = windowRect.position.x;
        controller.WindowY = windowRect.position.y;
    }
}
