using UnityEngine;
using Verse;

namespace LeTimer;

// textures have to reside in their own static class to avoid runtime threading errors
[StaticConstructorOnStartup]
public static class TexturesPlus
{
    // native icon of diagonal lines - similar to "resize" triangle in window corner
    // courtesy of improved workbenches
    public static readonly Texture2D DragHandle = ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true);

    public static readonly Texture2D ToggleIcon = ContentFinder<Texture2D>.Get("LeTimer/SettingsToggleIcon");
}
