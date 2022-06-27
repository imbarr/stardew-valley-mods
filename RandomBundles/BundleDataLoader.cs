using Newtonsoft.Json;
using StardewModdingAPI;
using System.IO;
using System;

namespace RandomBundles
{
    class BundleDataLoader : IAssetLoader
    {
        public static string AssetName = "Data\\ExpertBundles";

        public bool CanLoad<T>(IAssetInfo asset) => asset.Name.IsEquivalentTo(AssetName);
        public T Load<T>(IAssetInfo asset)
        {
            var path = Path.Combine(ModEntry.ModHelper.DirectoryPath, "apiBundles.json");
            var obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            if (obj == null)
            {
                throw new InvalidOperationException("Failed to load");
            }
            return obj;
        }
    }
}