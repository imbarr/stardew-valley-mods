using StardewModdingAPI;

namespace UnlockedBundles
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Patcher.DoPatching();
        }
    }
}
