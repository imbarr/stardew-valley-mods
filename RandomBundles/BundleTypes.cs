using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomBundles
{
    class Bundle
    {
        public string Name;
        public string Items;
        public int RequiredItems;
        public string Reward;
        
        public BundleApi toApi(int index, string color)
        {
            var api = new BundleApi();
            api.Name = Name;
            api.Color = color;
            api.Sprite = "LooseSprites\\BundleSprites:3";
            api.Items = Items;
            api.Pick = -1;
            api.RequiredItems = RequiredItems;
            api.Reward = Reward;
            api.Index = index;
            return api;
        }
    }

    class BundleData
    {
        public string AreaName;
        public string Keys;
        public Bundle[] Bundles;
    }

    class BundleApi
    {
        public string Name;
        public string Sprite;
        public string Color;
        public string Items;
        public int Pick;
        public int RequiredItems;
        public string Reward;
        public int Index;
    }

    class BundleSetApi
    {
        public BundleApi[] Bundles;
    }

    class BundleDataApi
    {
        public string AreaName;
        public string Keys;
        public BundleApi[] Bundles;
        public BundleSetApi[] BundleSets;
    }
}
