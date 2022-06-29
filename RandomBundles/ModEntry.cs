using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace RandomBundles
{
    public class ModEntry : Mod
    {
        public static IModHelper ModHelper;

        private string[] colors = new string[] { "Red", "Blue", "Green", "Orange", "Purple", "Teal", "Yellow" };

        private static string modDataKey = "Imbar.RandomBundles";

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Content.AssetLoaders.Add(new BundleDataLoader());
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var data = ModHelper.Data.ReadSaveData<ModData>(modDataKey);
            if (data != null)
            {
                return; 
            }

            var bundleData = Helper.Data.ReadJsonFile<BundleData[]>("bundles.json");
            var bundleDataApi = RandomizeBundles(bundleData);

            Helper.Data.WriteJsonFile("apiBundles.json", bundleDataApi);

            var generated = new BundleGenerator().Generate(BundleDataLoader.AssetName, new Random((int)Game1.uniqueIDForThisGame * 9));
            generated = ModEntry.FixIridiumQualityItems(generated);
            Game1.netWorldState.Value.SetBundleData(generated);

            var path = Path.Combine(ModEntry.ModHelper.DirectoryPath, "apiBundles.json");
            File.Delete(path);

            ModHelper.Data.WriteSaveData(modDataKey, new ModData());
        }

        private BundleDataApi[] RandomizeBundles(BundleData[] bundles)
        {
            return bundles.Select(x => RandomizeBundle(x)).ToArray();
        }

        private BundleDataApi RandomizeBundle(BundleData bundles)
        {
            var random = new Random((int)Game1.uniqueIDForThisGame * 9);
            Shuffle(random, bundles.Bundles);
            var keys = bundles.Keys.Split(" ");
            var keysCount = keys.Length;
            var randomizedBundles = new ArraySegment<Bundle>(bundles.Bundles, 0, keysCount);

            var bundlesApi = new BundleApi[keysCount];
            for (int i = 0; i < keysCount; i++)
            {
                bundlesApi[i] = randomizedBundles[i].toApi(i, colors[i]);
            }

            var api = new BundleDataApi();
            api.AreaName = bundles.AreaName;
            api.Keys = bundles.Keys;
            api.Bundles = new BundleApi[0];
            api.BundleSets = new BundleSetApi[1];
            api.BundleSets[0] = new BundleSetApi();
            api.BundleSets[0].Bundles = bundlesApi;

            return api;
        }

        private static Dictionary<string, string> FixIridiumQualityItems(Dictionary<string, string> data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var item in data)
            {
                string[] bundleData = item.Value.Split('/');
                string[] itemsList = bundleData[2].Split(' ');

                for (int i = 2; i < itemsList.Length; i += 3)
                {
                    if (itemsList[i] == "3")
                    {
                        itemsList[i] = "4";
                    }
                }

                bundleData[2] = string.Join(" ", itemsList);
                result[item.Key] = string.Join("/", bundleData);
            }

            return result;
        }

        private static void Shuffle<T>(Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
