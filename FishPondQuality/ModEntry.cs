using System;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using System.Linq;

namespace FishPondQuality
{
    public class ModEntry : Mod
    {
        public static string ModId = "Imbar.FishPondQuality";
        public static IMonitor monitor;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            Patcher.Patch();
            monitor = Monitor;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Farm farm = Game1.getFarm();
            foreach (Building building in farm.buildings)
            {
                if (building is FishPond || building.GetType().IsSubclassOf(typeof(FishPond)))
                {
                    FishPond pond = (FishPond)building;
                    if (!pond.modData.ContainsKey(ModId))
                    {
                        building.modData.Add(ModId, String.Concat(Enumerable.Repeat("0", pond.FishCount)));
                    }
                }
            }
        }
    }
}
