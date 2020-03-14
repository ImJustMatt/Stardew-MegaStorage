using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework.UI.Menus
{
    internal class ItemPickMenu : BaseOverlay
    {
        /*********
        ** Fields
        *********/
        public const int ItemsPerRow = 11;
        public static Vector2 Padding = new Vector2(56, 44);

        public ChestTab SelectedChestTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                CurrentCategory = 0;
                ChestTabName.label = _selectedTab.hoverText;
            }
        }

        public static List<SObject> Objects
        {
            get
            {
                if (AllObjects.Any())
                    return AllObjects;
                foreach (var id in Game1.objectInformation.Keys)
                {
                    AllObjects.Add(new SObject(Vector2.Zero, id, 1));
                }
                return AllObjects;
            }
        }

        public static IList<string> Categories
        {
            get
            {
                if (AllCategories.Any())
                    return AllCategories;
                var categories = Objects
                    .Select(obj => obj.getCategoryName())
                    .Distinct()
                    .Where(cat => !string.IsNullOrWhiteSpace(cat))
                    .ToList();
                categories.Sort();
                AllCategories.AddRange(categories);
                return AllCategories;
            }
        }

        public int CurrentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                _currentObjects = Objects
                    .Where(o => o.getCategoryName().Equals(Categories[_currentCategory], StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
                for (var slot = 0; slot < ItemsPerRow * MaxRows; ++slot)
                {
                    var itemSlot = _itemSlots.ElementAt(slot);
                    if (!(itemSlot is ClickableTextureComponent itemSlotCC))
                        continue;
                    if (slot >= _currentObjects.Count)
                    {
                        itemSlotCC.visible = false;
                        continue;
                    }

                    var obj = _currentObjects[slot];
                    itemSlotCC.sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, obj.ParentSheetIndex, 16, 16);
                    itemSlotCC.name = obj.ParentSheetIndex.ToString(CultureInfo.InvariantCulture);
                    itemSlotCC.hoverText = obj.DisplayName;
                    itemSlotCC.visible = true;
                }
            }
        }

        // Padding for Items Grid
        public const int MaxRows = 7;

        private protected static readonly List<SObject> AllObjects = new List<SObject>();
        private protected static readonly List<string> AllCategories = new List<string>();
        private protected Label ChestTabName;
        private protected ClickableTexture LeftArrow;
        private protected ClickableTexture RightArrow;
        private protected Checkbox CategoryCheckbox;
        private protected Label CategoryName;
        private ChestTab _selectedTab;
        private int _currentCategory;
        private IList<SObject> _currentObjects;
        private readonly IList<IWidget> _itemSlots = new List<IWidget>();

        /*********
        ** Public methods
        *********/
        public ItemPickMenu(IMenu parentMenu, Vector2 offset)
            : base(parentMenu, offset)
        {
            width = ItemsToGrabMenu.width - (int)Padding.X;
            height = Inventory.yPositionOnScreen -
                     ItemsToGrabMenu.yPositionOnScreen +
                     Inventory.height -
                     (int)Padding.Y;
            SetupWidgets();
        }

        public override void draw(SpriteBatch b)
        {
            // Draw Dialogue Box
            Sprites.Menu.Draw(b, this.GetBounds());

            this.Draw(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (!isWithinBounds(x, y))
                Visible = false;
        }

        /*********
        ** Private methods
        *********/
        private void SetupWidgets()
        {
            // Chest Tab Name
            ChestTabName = new Label(
                "chestTabName",
                this,
                BaseInventoryMenu.Padding,
                SelectedChestTab?.name,
                width - (int)Padding.X * 2,
                Game1.tileSize,
                Align.Center);
            allClickableComponents.Add(ChestTabName);

            // Left Arrow
            LeftArrow = new ClickableTexture(
                "leftArrow",
                this,
                Padding * new Vector2(1, 1) +
                new Vector2(0, 0),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44),
                scale: 1f)
            {
                visible = false,
                LeftClickAction = PrevPage
            };
            allClickableComponents.Add(LeftArrow);

            // Right Arrow
            RightArrow = new ClickableTexture(
                "rightArrow",
                this,
                Padding * new Vector2(-1, 1) +
                new Vector2(width - Game1.tileSize, 0),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33),
                scale: 1f)
            {
                LeftClickAction = NextPage
            };
            allClickableComponents.Add(RightArrow);

            // Category Checkbox
            CategoryCheckbox = new Checkbox(
                "categoryCheckbox",
                this,
                Padding + new Vector2(0, Game1.tileSize),
                Game1.mouseCursors,
                OptionsCheckbox.sourceRectUnchecked,
                OptionsCheckbox.sourceRectChecked);
            allClickableComponents.Add(CategoryCheckbox);

            // Category Name (Label)
            CategoryName = new Label(
                "categoryName",
                this,
                Padding + new Vector2(Game1.tileSize, Game1.tileSize),
                Categories[CurrentCategory]);
            allClickableComponents.Add(CategoryName);

            // Items
            for (var slot = 0; slot < ItemsPerRow * MaxRows; ++slot)
            {
                var col = slot % ItemsPerRow;
                var row = slot / ItemsPerRow;
                var itemSlotCC = new ClickableTexture(
                    slot.ToString(CultureInfo.InvariantCulture),
                    this,
                    Padding + new Vector2(col, row + 2) * Game1.tileSize,
                    Game1.objectSpriteSheet,
                    Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 1))
                {
                    DrawAction = DrawItem
                };
                _itemSlots.Add(itemSlotCC);
                allClickableComponents.Add(itemSlotCC);
            }

            CurrentCategory = 0;
        }

        private static void DrawItem(SpriteBatch b, IWidget widget)
        {
            if (!(widget is ClickableTexture cc))
                return;
            cc.draw(
                b,
                Color.White * 0.5f,
                0.865f);
        }

        private void PrevPage(IWidget widget)
        {
            if (CurrentCategory <= 0)
                return;
            --CurrentCategory;
            CategoryName.label = Categories[CurrentCategory];
            LeftArrow.visible = CurrentCategory > 0;
            RightArrow.visible = CurrentCategory < Categories.Count - 1;
        }
        private void NextPage(IWidget widget)
        {
            if (CurrentCategory >= Categories.Count - 1)
                return;
            ++CurrentCategory;
            CategoryName.label = Categories[CurrentCategory];
            LeftArrow.visible = CurrentCategory > 0;
            RightArrow.visible = CurrentCategory < Categories.Count - 1;
        }
    }
}
