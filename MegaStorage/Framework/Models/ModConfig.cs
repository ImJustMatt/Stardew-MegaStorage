using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework.Models
{
    public class CustomChestConfig
    {
        public bool EnableChest { get; set; }
        public bool EnableCategories { get; set; }
    }

    public class StashConfig
    {
        public string Includes { get; set; } = "";
        public string Excludes { get; set; } = "";

        internal IList<int> IncludesAsList => !string.IsNullOrWhiteSpace(Includes)
            ? Includes
                .Split(' ')
                .Select(c => Convert.ToInt32(c, CultureInfo.InvariantCulture))
                .ToList()
            : null;
        internal IList<int> ExcludesAsList => !string.IsNullOrWhiteSpace(Excludes)
            ? Excludes
                .Split(' ')
                .Select(c => Convert.ToInt32(c, CultureInfo.InvariantCulture))
                .ToList()
            : null;
    }

    public class CustomCategoryConfig: StashConfig
    {
        public string CategoryName { get; set; }
        public string Image { get; set; } = "";
    }

    public class ModConfig
    {
        public CustomChestConfig LargeChest { get; set; } = new CustomChestConfig
        {
            EnableChest = true,
            EnableCategories = false
        };
        public CustomChestConfig MagicChest { get; set; } = new CustomChestConfig
        {
            EnableChest = true,
            EnableCategories = true
        };
        public CustomChestConfig SuperMagicChest { get; set; } = new CustomChestConfig
        {
            EnableChest = true,
            EnableCategories = true
        };

        public SButton StashKey { get; set; } = SButton.Q;
        public SButton StashAnywhereKey { get; set; } = SButton.Z;
        public SButton StashButton { get; set; } = SButton.RightStick;

        public IList<CustomCategoryConfig> Categories { get; set; } =
            new List<CustomCategoryConfig>(7)
            {
                {
                    new CustomCategoryConfig()
                    {
                        CategoryName = "All",
                        Image = "AllTab.png"
                    }
                },
                {
                    new CustomCategoryConfig()
                    {
                        CategoryName = "Crops",
                        Includes =
                            SObject.GreensCategory + " " +
                            SObject.flowersCategory + " " +
                            SObject.FruitsCategory + " " +
                            SObject.VegetableCategory
                    }
                },
                {
                    new CustomCategoryConfig()
                    {
                        CategoryName = "Seeds",
                        Includes =
                            SObject.SeedsCategory + " " +
                            SObject.fertilizerCategory
                    }
                },
                {
                    new CustomCategoryConfig()
                    {
                        CategoryName = "Materials",
                        Includes =
                            SObject.metalResources + " " +
                            SObject.buildingResources + " " +
                            SObject.GemCategory + " " +
                            SObject.mineralsCategory + " " +
                            SObject.CraftingCategory + " " +
                            SObject.monsterLootCategory
                    }
                },
                {
                    new CustomCategoryConfig()
                    {
                        CategoryName = "Cooking",
                        Includes =
                            SObject.ingredientsCategory + " " +
                            SObject.CookingCategory + " " +
                            SObject.sellAtPierresAndMarnies + " " +
                            SObject.meatCategory + " " +
                            SObject.MilkCategory + " " +
                            SObject.EggCategory + " " +
                            SObject.syrupCategory + " " +
                            SObject.artisanGoodsCategory
                    }
                },
                {
                    new CustomCategoryConfig()
                    {
                        CategoryName = "Fishing",
                        Includes =
                            SObject.FishCategory + " " +
                            SObject.baitCategory + " " +
                            SObject.tackleCategory
                    }
                },
                {
                    new CustomCategoryConfig()
                    {
                        CategoryName = "Misc",
                        Includes =
                            SObject.furnitureCategory + " " +
                            SObject.junkCategory
                    }
                }
            };

        public StashConfig StashItems { get; set; } = new StashConfig()
        {
            Includes =
                SObject.GreensCategory + " " +
                SObject.flowersCategory + " " +
                SObject.FruitsCategory + " " +
                SObject.VegetableCategory + " " +
                SObject.SeedsCategory + " " +
                SObject.fertilizerCategory + " " +
                SObject.metalResources + " " +
                SObject.buildingResources + " " +
                SObject.GemCategory + " " +
                SObject.mineralsCategory + " " +
                SObject.CraftingCategory + " " +
                SObject.monsterLootCategory + " " +
                SObject.ingredientsCategory + " " +
                SObject.CookingCategory + " " +
                SObject.sellAtPierresAndMarnies + " " +
                SObject.meatCategory + " " +
                SObject.MilkCategory + " " +
                SObject.EggCategory + " " +
                SObject.syrupCategory + " " +
                SObject.artisanGoodsCategory + " " +
                SObject.FishCategory + " " +
                SObject.baitCategory + " " +
                SObject.tackleCategory + " " +
                SObject.furnitureCategory + " " +
                SObject.junkCategory
        };

        public ModConfig()
        {
            Instance = this;
        }
        public static ModConfig Instance { get; private set; }
    }
}