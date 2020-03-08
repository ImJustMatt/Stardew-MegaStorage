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

namespace MegaStorage.Framework.UI
{
    internal class ItemPickMenu : IClickableMenu, ISubMenu
    {
        /*********
        ** Fields
        *********/
        public IClickableMenu ParentMenu { get; }
        public MenuType MenuType => MenuType.Overlay;
        public Vector2 Offset { get; }
        public Rectangle Bounds => new Rectangle(xPositionOnScreen, yPositionOnScreen, height, width);
        public Vector2 Position => new Vector2(xPositionOnScreen, yPositionOnScreen);
        public Vector2 Dimensions => new Vector2(width, height);
        public bool Visible { get; set; }

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
        public static Vector2 Padding = new Vector2(56, 44);
        public const int ItemsPerRow = 11;
        public const int MaxRows = 7;

        private protected static readonly List<SObject> AllObjects = new List<SObject>();
        private protected static readonly List<string> AllCategories = new List<string>();
        private protected Label ChestTabName;
        private protected CustomClickableTextureComponent LeftArrow;
        private protected CustomClickableTextureComponent RightArrow;
        private protected Checkbox CategoryCheckbox;
        private protected Label CategoryName;
        private ChestTab _selectedTab;
        private int _currentCategory;
        private IList<SObject> _currentObjects;
        private readonly IList<IWidget> _itemSlots = new List<IWidget>();
        private string _hoverText;

        /*********
        ** Public methods
        *********/
        public ItemPickMenu(IClickableMenu parentMenu, Vector2 offset)
        {
            ParentMenu = parentMenu;
            var customItemGrabMenu = CommonHelper.OfType<IClickableMenu, CustomItemGrabMenu>(ParentMenu);
            var itemsToGrabMenu = customItemGrabMenu.ItemsToGrabMenu;
            var inventory = customItemGrabMenu.inventory;
            Offset = offset + Padding / 2;
            width = itemsToGrabMenu.width - (int)Padding.X;
            height = inventory.yPositionOnScreen -
                     itemsToGrabMenu.yPositionOnScreen +
                     inventory.height -
                     (int)Padding.Y;
            xPositionOnScreen = ParentMenu.xPositionOnScreen + (int)Offset.X;
            yPositionOnScreen = ParentMenu.yPositionOnScreen + (int)Offset.Y;

            allClickableComponents = new List<ClickableComponent>();
            SetupWidgets();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            // Draw Dialogue Box
            CommonHelper.DrawDialogueBox(b, xPositionOnScreen, yPositionOnScreen, width, height);

            // Draw Widgets
            foreach (var clickableComponent in allClickableComponents
                .Where(c =>
                    c is IWidget widget
                    && !(widget.DrawAction is null)))
            {
                ((IWidget)clickableComponent).DrawAction(b, clickableComponent);
            }

            // Draw Components
            foreach (var clickableComponent in allClickableComponents
                .OfType<ClickableTextureComponent>()
                .Where(c =>
                    !(c is IWidget widget)
                    || widget.DrawAction is null))
            {
                clickableComponent.draw(b);
            }

            // Hover Text
            if (!string.IsNullOrWhiteSpace(_hoverText))
                IClickableMenu.drawHoverText(b, _hoverText, Game1.smallFont);

            // Game Cursor
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!Visible)
                return;

            // Left Click Widgets
            foreach (var clickableComponent in allClickableComponents
                .Where(c =>
                    c.containsPoint(x, y)
                    && c is IWidget widget
                    && !(widget.LeftClickAction is null)))
            {
                ((IWidget)clickableComponent).LeftClickAction(clickableComponent);
            }

            if (!isWithinBounds(x, y))
                Visible = false;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            // Right Click Widgets
            foreach (var clickableComponent in allClickableComponents
                .Where(c =>
                    c.containsPoint(x, y)
                    && c is IWidget widget
                    && !(widget.RightClickAction is null)))
            {
                ((IWidget)clickableComponent).RightClickAction(clickableComponent);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            var mouseX = Game1.getOldMouseX();
            var mouseY = Game1.getOldMouseY();

            // Scroll Components
            foreach (var clickableComponent in allClickableComponents
                .Where(c =>
                    c.containsPoint(mouseX, mouseY)
                    && c is IWidget widget
                    && !(widget.ScrollAction is null)))
            {
                ((IWidget)clickableComponent).ScrollAction(direction, clickableComponent);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = null;

            // Hover Text
            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => !(c.hoverText is null) && c.containsPoint(x, y)))
            {
                _hoverText = clickableComponent.hoverText;
            }

            // Hover Widgets
            foreach (var clickableComponent in allClickableComponents
                .Where(c =>
                    c is IWidget widget
                    && !(widget.HoverAction is null)))
            {
                ((IWidget)clickableComponent).HoverAction(x, y, clickableComponent);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            xPositionOnScreen = ParentMenu.xPositionOnScreen + (int)Offset.X;
            yPositionOnScreen = ParentMenu.yPositionOnScreen + (int)Offset.Y;

            // Scroll Widgets
            foreach (var widget in allClickableComponents.OfType<IWidget>())
            {
                widget.GameWindowSizeChanged();
            }
        }

        private void PrevPage(ClickableComponent clickableComponent = null)
        {
            if (CurrentCategory <= 0)
                return;
            --CurrentCategory;
            CategoryName.label = Categories[CurrentCategory];
            LeftArrow.visible = CurrentCategory > 0;
            RightArrow.visible = CurrentCategory < Categories.Count - 1;
        }
        private void NextPage(ClickableComponent clickableComponent = null)
        {
            if (CurrentCategory >= Categories.Count - 1)
                return;
            ++CurrentCategory;
            CategoryName.label = Categories[CurrentCategory];
            LeftArrow.visible = CurrentCategory > 0;
            RightArrow.visible = CurrentCategory < Categories.Count - 1;
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
                CustomInventoryMenu.Padding,
                SelectedChestTab?.name,
                width - (int)Padding.X * 2,
                Game1.tileSize,
                Align.Center);
            allClickableComponents.Add(ChestTabName);

            // Left Arrow
            LeftArrow = new CustomClickableTextureComponent(
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
            RightArrow = new CustomClickableTextureComponent(
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
                Padding + new Vector2(0, Game1.tileSize));
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
                var itemSlotCC = new CustomClickableTextureComponent(
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

        private void DrawItem(SpriteBatch b, ClickableComponent clickableComponent)
        {
            if (!(clickableComponent is CustomClickableTextureComponent itemSlotCC))
                return;
            itemSlotCC.draw(
                b,
                Color.White * 0.5f,
                0.865f);
        }
    }
}
