using System;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CrabPotFish
{
    class CrabPotReplacement
    {
        public static bool checkForActionPrefix(Farmer who, bool justCheckingForActivity, ref bool __result, CrabPot __instance)
        {
			if (__instance.tileIndexToShow == 714)
			{
				if (justCheckingForActivity)
				{
					__result = true;
					return false;
				}
				StardewValley.Object item = __instance.heldObject.Value;
				__instance.heldObject.Value = null;
				if (who.IsLocalPlayer && !who.addItemToInventoryBool(item))
				{
					__instance.heldObject.Value = item;
					Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
					__result = false;
					return false;
				}
				Dictionary<int, string> data = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
				if (data.ContainsKey(item.parentSheetIndex))
				{
					string[] rawData = data[item.parentSheetIndex].Split('/');
					bool isFish = rawData[1] != "trap";
					int minFishSize = ((rawData.Length <= 5) ? 1 : Convert.ToInt32(isFish ? rawData[3] : rawData[5]));
					int maxFishSize = ((rawData.Length > 5) ? Convert.ToInt32(isFish ? rawData[4] : rawData[6]) : 10);
					who.caughtFish(item.parentSheetIndex, Game1.random.Next(minFishSize, maxFishSize + 1));
				}
				__instance.readyForHarvest.Value = false;
				__instance.tileIndexToShow = 710;
				var lidFlapping = __instance.GetType().GetField("lidFlapping", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				lidFlapping.SetValue(__instance, true);
				var lidFlapTimer = __instance.GetType().GetField("lidFlapTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				lidFlapTimer.SetValue(__instance, 60f);
				__instance.bait.Value = null;
				who.animateOnce(279 + who.FacingDirection);
				who.currentLocation.playSound("fishingRodBend");
				DelayedAction.playSoundAfterDelay("coin", 500);
				who.gainExperience(1, 5);
				var shake = __instance.GetType().GetField("shake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				shake.SetValue(__instance, Vector2.Zero);
				var shakeTimer = __instance.GetType().GetField("shakeTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				shakeTimer.SetValue(__instance, 0f);
				__result = true;
				return false;
			}
			if (__instance.bait.Value == null)
			{
				if (justCheckingForActivity)
				{
					__result = true;
					return false;
				}
				if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
				{
					if (Game1.player.addItemToInventoryBool(__instance.getOne()))
					{
						if (who.isMoving())
						{
							Game1.haltAfterCheck = false;
						}
						Game1.playSound("coin");
						Game1.currentLocation.objects.Remove(__instance.tileLocation);
						__result = true;
						return false;
					}
					Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
				}
			}
			__result = false;
			return false;
		}
    }
}
