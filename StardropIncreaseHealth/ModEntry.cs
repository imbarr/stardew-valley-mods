using StardewModdingAPI;
using HarmonyLib;
using System.Reflection;
using StardewValley;

namespace StardropIcreaseHealth
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony("com.stardropIcreaseHealth.patch");
            var mOriginal = typeof(Farmer).GetMethod("doneEating");
            var mPostfix = typeof(ModEntry).GetMethod("Postfix", BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(mOriginal, postfix: new HarmonyMethod(mPostfix));
        }

        public static void Postfix(Farmer __instance)
        {
            Object consumed = __instance.itemToEat as Object;
            if (__instance.IsLocalPlayer && consumed.ParentSheetIndex == 434)
            {
                __instance.maxHealth += 5;
                __instance.health = __instance.maxHealth;
            }
        }
    }
}
