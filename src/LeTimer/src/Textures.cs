using UnityEngine;
using Verse;

namespace LeTimer;

[StaticConstructorOnStartup]
public static class TexturesPlus
{
    public static Texture2D ToggleIcon { get; private set; } = ContentFinder<Texture2D>.Get("LeTimer/SettingsToggleIcon");
}
