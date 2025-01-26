using UnityEngine;
using Verse;

namespace LeTimer;

/// <summary>
/// some frequently used helpers & constants
/// </summary>
public class WindowPlus
{
    public const int LABEL_HEIGHT_SM = 22;

    public static Vector2 MiddleScreenPos(Rect window)
    {
        return new Vector2(
            (UI.screenWidth - window.width) / 2,
            (UI.screenHeight - window.height) / 2
        );
    }
    public static Vector2 MiddleScreenPos(Vector2 window)
    {
        return new Vector2(
            (UI.screenWidth - window.x) / 2,
            (UI.screenHeight - window.y) / 2
        );
    }

    public static void ShrinkWindowHeightToContent(Window w, int sumHeight)
    {
        // windows have 17px margin from each side that erases contents, so:
        // - (0,0) is technically a (17,17)
        // - to draw 20pix height bar, window needs 17+20+17=44 height
        sumHeight += (int)GenUI.Gap * 2;
        int heightPrev = (int)w.windowRect.height;
        w.windowRect.height = sumHeight;
    }

}
