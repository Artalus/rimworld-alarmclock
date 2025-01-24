using Verse;

namespace LeTimer
{
    [StaticConstructorOnStartup]
    public static class MyMod
    {
        static MyMod()
        {
            Log.Message("Hello World!");
        }
    }
}
