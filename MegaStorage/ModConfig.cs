using MegaStorage.Framework.UI.Menus;
using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage
{
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
        internal bool BelongsTo(Item item) =>
            (item is SObject obj)
            && (IncludesAsList is null || IncludesAsList.Contains(obj.Category) || IncludesAsList.Contains(obj.ParentSheetIndex))
            && (ExcludesAsList is null || !(ExcludesAsList.Contains(obj.Category) || ExcludesAsList.Contains(obj.ParentSheetIndex)));
    }

    public class ChestTabConfig : StashConfig
    {
        /*********
        ** Fields
        *********/
        internal static readonly Vector2 Offset = new Vector2(-48, 24);
        internal static readonly Dictionary<string, Rectangle> DefaultChestTabs = new Dictionary<string, Rectangle>()
        {
            {"All", Rectangle.Empty},
            {"Crops", new Rectangle(640, 80, 16, 16)},
            {"Seeds", new Rectangle(656, 64, 16, 16)},
            {"Materials", new Rectangle(672, 64, 16, 16)},
            {"Cooking", new Rectangle(688, 64, 16, 16)},
            {"Fishing", new Rectangle(640, 64, 16, 16)},
            {"Misc", new Rectangle(672, 80, 16, 16)}
        };
        public string Name { get; set; }
        public string Image { get; set; } = "";
        internal Texture2D Texture => !string.IsNullOrWhiteSpace(Image)
            ? MegaStorageMod.Helper.Content.Load<Texture2D>(Path.Combine("assets", Image))
            : Game1.mouseCursors;
        internal Rectangle SourceRect =>
            string.IsNullOrWhiteSpace(Image)
            && DefaultChestTabs.TryGetValue(Name, out var sourceRect)
                ? sourceRect
                : Rectangle.Empty;

        /*********
        ** Public methods
        *********/
        internal ChestTab ChestTab(IMenu parentMenu, int index) => new ChestTab(
            Name,
            parentMenu,
            Offset + new Vector2(0, index * 60),
            Texture,
            SourceRect);

    }

    public class ModConfig
    {
        public SButton StashKey { get; set; } = SButton.Q;
        public SButton StashAnywhereKey { get; set; } = SButton.Z;
        public SButton StashButton { get; set; } = SButton.RightStick;

        public IList<ChestTabConfig> ChestTabs { get; set; } =
            new List<ChestTabConfig>(7)
            {
                {
                    new ChestTabConfig()
                    {
                        Name = "All",
                        Image = "AllTab.png"
                    }
                },
                {
                    new ChestTabConfig()
                    {
                        Name = "Crops",
                        Includes =
                            SObject.GreensCategory + " " +
                            SObject.flowersCategory + " " +
                            SObject.FruitsCategory + " " +
                            SObject.VegetableCategory
                    }
                },
                {
                    new ChestTabConfig()
                    {
                        Name = "Seeds",
                        Includes =
                            SObject.SeedsCategory + " " +
                            SObject.fertilizerCategory
                    }
                },
                {
                    new ChestTabConfig()
                    {
                        Name = "Materials",
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
                    new ChestTabConfig()
                    {
                        Name = "Cooking",
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
                    new ChestTabConfig()
                    {
                        Name = "Fishing",
                        Includes =
                            SObject.FishCategory + " " +
                            SObject.baitCategory + " " +
                            SObject.tackleCategory
                    }
                },
                {
                    new ChestTabConfig()
                    {
                        Name = "Misc",
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