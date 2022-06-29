using StardewValley.Buildings;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using System.Linq;
using System;
using System.Reflection;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewValley.Objects;

namespace FishPondQuality
{
    class Patcher
    {
        public static void Patch()
        {
            var harmony = new Harmony("com.fishPondQuality.patch");

            var addFishToPond_Original = typeof(FishPond).GetMethod("addFishToPond", BindingFlags.Instance | BindingFlags.NonPublic);
            var addFishToPond_Postfix = typeof(Patcher).GetMethod("addFishToPond_Postfix", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(addFishToPond_Original, postfix: new HarmonyMethod(addFishToPond_Postfix));

            var clearPond_Original = typeof(FishPond).GetMethod("ClearPond");
            var clearPond_Postfix = typeof(Patcher).GetMethod("clearPond_Postfix", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(clearPond_Original, postfix: new HarmonyMethod(clearPond_Postfix));
            
            var spawnFish_Original = typeof(FishPond).GetMethod("SpawnFish");
            var spawnFish_Postfix = typeof(Patcher).GetMethod("spawnFish_Postfix", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(spawnFish_Original, postfix: new HarmonyMethod(spawnFish_Postfix));
            
            var getFishProduce_Original = typeof(FishPond).GetMethod("GetFishProduce");
            var getFishProduce_Postfix = typeof(Patcher).GetMethod("getFishProduce_Postfix", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(getFishProduce_Original, postfix: new HarmonyMethod(getFishProduce_Postfix));
            
            var pullFishFromWater_Original = typeof(FishingRod).GetMethod("pullFishFromWater");
            var pullFishFromWater_Prefix = typeof(Patcher).GetMethod("pullFishFromWater_Prefix", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(pullFishFromWater_Original, prefix: new HarmonyMethod(pullFishFromWater_Prefix));
            
            var draw_Original = typeof(PondQueryMenu).GetMethod("draw", new Type[] { typeof(SpriteBatch) });
            var draw_Postfix = typeof(Patcher).GetMethod("draw_Postfix", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(draw_Original, postfix: new HarmonyMethod(draw_Postfix));
        }

        private static void addFishToPond_Postfix(FishPond __instance, StardewValley.Object fish)
        {
            __instance.modData[ModEntry.ModId] += fish.Quality;
        }

        private static void clearPond_Postfix(FishPond __instance)
        {
            __instance.modData[ModEntry.ModId] = "";
        }

        private static void spawnFish_Postfix(FishPond __instance)
        {
            if (__instance.hasSpawnedFish.Value)
            {
                var quality = __instance.modData[ModEntry.ModId];
                var commonProb = quality.Count(q => q == '0') / __instance.FishCount * 100;
                var silverProb = commonProb + quality.Count(q => q == '1') / __instance.FishCount * 100;
                var goldProb = silverProb + quality.Count(q => q == '2') / __instance.FishCount * 100;

                var random = new Random().Next(100);

                if (random < commonProb)
                {
                    __instance.modData[ModEntry.ModId] += '0';
                }
                else if (random >= commonProb && random < silverProb)
                {
                    __instance.modData[ModEntry.ModId] += '1';
                }
                else if (random >= silverProb && random < goldProb)
                {
                    __instance.modData[ModEntry.ModId] += '2';
                }
                else if (random >= goldProb)
                {
                    __instance.modData[ModEntry.ModId] += '4';
                }
            }
        }

        private static void pullFishFromWater_Prefix(FishingRod __instance, ref int fishQuality, bool fromFishPond)
        {
            if (fromFishPond)
            {
                var calculateBobberTileMethod = AccessTools.Method(typeof(FishingRod), "calculateBobberTile");
                Vector2 bobberTile = (Vector2)calculateBobberTileMethod.Invoke(__instance, new object[] { });

                Building building = Game1.getFarm().getBuildingAt(bobberTile);
                if (building is FishPond || building.GetType().IsSubclassOf(typeof(FishPond)))
                {
                    FishPond pond = (FishPond)building;
                    string pondData = pond.modData[ModEntry.ModId];
                    int randomIndex = Game1.random.Next(pondData.Length);
                    fishQuality = int.Parse(pondData[randomIndex].ToString());
                    pond.modData[ModEntry.ModId] = pondData.Remove(randomIndex, 1);
                    return;
                }
            }
        }

        private static void draw_Postfix(PondQueryMenu __instance, SpriteBatch b)
        {
            if (!Game1.globalFade)
            {
                FishPond pond = (FishPond)__instance.GetType()
                    .GetField("_pond", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(__instance);

                float age = (float)__instance.GetType()
                    .GetField("_age", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(__instance);

                StardewValley.Object fishItem = (StardewValley.Object)__instance.GetType()
                    .GetField("_fishItem", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(__instance);

                string quality = pond.modData[ModEntry.ModId];

                int slots_to_draw = pond.maxOccupants.Value;
                float slot_spacing = 13f;
                int x = 0;
                int y = 0;
                for (int i = 0; i < slots_to_draw; i++)
                {
                    if (i < quality.Length)
                    {
                        fishItem.Quality = int.Parse(quality[i].ToString());
                    } else
                    {
                        fishItem.Quality = 0;
                    }

                    float y_offset = (float)Math.Sin(age * 1f + (float)x * 0.75f + (float)y * 0.25f) * 2f;
                    if (i < pond.FishCount)
                    {
                        fishItem.drawInMenu(b, new Vector2((float)(__instance.xPositionOnScreen + PondQueryMenu.width / 2) - slot_spacing * (float)Math.Min(slots_to_draw, 5) * 4f * 0.5f + slot_spacing * 4f * (float)x - 12f, (float)(__instance.yPositionOnScreen + (int)(y_offset * 4f)) + (float)(y * 4) * slot_spacing + 275.2f), 0.75f, 1f, 0f, StackDrawType.HideButShowQuality, Color.White, drawShadow: false);
                    }
                    else
                    {
                        fishItem.drawInMenu(b, new Vector2((float)(__instance.xPositionOnScreen + PondQueryMenu.width / 2) - slot_spacing * (float)Math.Min(slots_to_draw, 5) * 4f * 0.5f + slot_spacing * 4f * (float)x - 12f, (float)(__instance.yPositionOnScreen + (int)(y_offset * 4f)) + (float)(y * 4) * slot_spacing + 275.2f), 0.75f, 0.35f, 0f, StackDrawType.HideButShowQuality, Color.Black, drawShadow: false);
                    }
                    x++;
                    if (x == 5)
                    {
                        x = 0;
                        y++;
                    }
                }
            }
        }

        private static void getFishProduce_Postfix(ref StardewValley.Object __result, FishPond __instance)
        {
            if (__result == null)
            {
                return;
            }
            var possibility = 0d;
            foreach (char q in __instance.modData[ModEntry.ModId])
            {
                var quality = int.Parse(q.ToString());
                var fraction = 1d / __instance.FishCount;

                if (quality == 0)
                {
                    possibility += fraction * 0;
                }
                else if (quality == 1)
                {
                    possibility += fraction * 25;
                }
                else if (quality == 2)
                {
                    possibility += fraction * 50;
                }
                else if (quality == 4)
                {
                    possibility += fraction * 100;
                }
            }

            var random = new Random().Next(100);
            if (random < possibility)
            {
                __result.Stack *= 2;
            }
        }
    }
}
