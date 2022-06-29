using StardewModdingAPI;
using HarmonyLib;
using System.Reflection;
using StardewValley;

namespace OutdoorPlacementFix
{
    public class ModEntry: Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony("com.outdoorPlacementFix.patch");
            var mOriginal = typeof(Object).GetMethod("canBePlacedHere");
            var mPostfix = typeof(ModEntry).GetMethod("Postfix", BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(mOriginal, postfix: new HarmonyMethod(mPostfix));
        }

        public static void Postfix(Object __instance, ref bool __result, GameLocation l)
        {
            if(__instance.bigCraftable.Value && Game1.bigCraftablesInformation[__instance.parentSheetIndex].Split('/')[5] == "false" && l.IsOutdoors)
            {
                __result = false; 
            }
        }
    }
}
