using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Reflection;

namespace QualityCrabPot
{
    public class ModEntry : Mod
    {
        private static IMonitor monitor;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.DayStarted += delegate { onDayStarted(); };
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
                        if (pot != null && fish != null && pot.readyForHarvest.Value && !(fish.ParentSheetIndex >= 168 && fish.ParentSheetIndex < 173))
                        {
                            float fishSize = 1f;
                            int minimumSizeContribution = 1 + Game1.getFarmer(pot.owner.Value).FishingLevel / 2;
                            fishSize *= (float)Game1.random.Next(minimumSizeContribution, Math.Max(6, minimumSizeContribution)) / 5f;
                            fishSize *= 1f + (float)Game1.random.Next(-10, 11) / 100f;
                            fishSize = Math.Max(0f, Math.Min(1f, fishSize));

                            if (fishSize < 0.33)
                            {
                                pot.heldObject.Value.Quality = 0;
                            } else if (fishSize < 0.66)
                            {
                                pot.heldObject.Value.Quality = 1;
                            } else
                            {
                                pot.heldObject.Value.Quality = 2;
                            }
                        }
                    }
                }
            }
        }
    }
}
