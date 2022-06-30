using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using HarmonyLib;
using System.Reflection;

namespace CrabPotFish
{
    public class ModEntry : Mod
    {
        private static IMonitor monitor;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.DayStarted += delegate { onDayStarted(); };
            monitor = this.Monitor;

            var harmony = new Harmony("com.crabPotFish.patch");
            var mOriginal = typeof(CrabPot).GetMethod("checkForAction", new Type[] { typeof(Farmer), typeof(bool) });
            var prefix = typeof(CrabPotReplacement).GetMethod("checkForActionPrefix", BindingFlags.Static | BindingFlags.Public);
            harmony.Patch(mOriginal, prefix: new HarmonyMethod(prefix));
        }

        private void onDayStarted()
        {
            foreach (var location in Game1.locations)
            {
                foreach (var item in location.Objects.Values)
                {
                    if (item is CrabPot pot)
                    {
                        var fish = pot.heldObject.Value;
                        if (pot != null && fish != null && pot.readyForHarvest.Value)
                        {
                            var random = new Random().Next(100);
                            if (random < 10 && !(fish.ParentSheetIndex >= 168 && fish.ParentSheetIndex < 173))
                            {
                                var owner = Game1.getFarmer(pot.owner.Value) ?? Game1.MasterPlayer;
                                var f = location.getFish(1f, pot.bait.Value.ParentSheetIndex, 100, owner, 0, pot.TileLocation);
                                pot.heldObject.Value = f;
                            }
                        }
                    }
                }
            }
        }
    }
}
