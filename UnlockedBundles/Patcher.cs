using HarmonyLib;
using System.Reflection;
using StardewValley.Locations;

namespace UnlockedBundles
{
    class Patcher
    {
        public static void DoPatching()
        {
            var harmony = new Harmony("com.unlocked.patch");
            var mOriginal = typeof(CommunityCenter).GetMethod("shouldNoteAppearInArea");
            var mPostfix = typeof(Patcher).GetMethod("Postfix", BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(mOriginal, postfix: new HarmonyMethod(mPostfix));
        }

        public static void Postfix(int area, ref bool __result)
        {
            if (area != 5 && area != 6)
            {
                __result = true;
            }
        }
    }
}
