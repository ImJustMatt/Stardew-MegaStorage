using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MegaStorage.Framework.UI
{
    internal class CustomInventoryMenu : InventoryMenu, ISubMenu
    {
        /*********
        ** Fields
        *********/
        public IClickableMenu ParentMenu { get; }
        public MenuType MenuType => MenuType.BaseMenu;
        public Vector2 Offset { get; }
        public Rectangle Bounds => new Rectangle(xPositionOnScreen, yPositionOnScreen, height, width);
        public Vector2 Position => new Vector2(xPositionOnScreen, yPositionOnScreen);
        public Vector2 Dimensions => new Vector2(width, height);
        public bool Visible { get; set; } = true;
        public ChestTab SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                RefreshItems();
            }
        }

        // Padding for Items Grid
        public static Vector2 Padding = new Vector2(56, 44);
        public int MaxItems =>
            _inventoryType == InventoryType.Player
                ? Game1.player.MaxItems
                : CommonHelper.OfType<IClickableMenu, CustomItemGrabMenu>(ParentMenu).ActiveChest?.Capacity
                ?? 0;
        public const int ItemsPerRow = 12;
        internal int MaxRows;
        public IList<Item> VisibleItems;
        private readonly InventoryType _inventoryType;

        private protected CustomClickableTextureComponent UpArrow;
        private protected CustomClickableTextureComponent DownArrow;
        private ChestTab _selectedCategory;
        private int _currentRow;

        /*********
        ** Public methods
        *********/
        public CustomInventoryMenu(IClickableMenu parentMenu, Vector2 offset, InventoryType inventoryType)
            : base(
                parentMenu.xPositionOnScreen + (int)offset.X,
                parentMenu.yPositionOnScreen + (int)offset.Y,
                false,
                inventoryType == InventoryType.Player ? Game1.player.Items : CommonHelper.OfType<IClickableMenu, CustomItemGrabMenu>(parentMenu).ActiveChest?.items,
                InventoryMenu.highlightAllItems,
                inventoryType == InventoryType.Player ? Math.Max(36, Game1.player.MaxItems) : 6 * ItemsPerRow,
                inventoryType == InventoryType.Player ? Math.Max(36, Game1.player.MaxItems) / ItemsPerRow : 6)
        {
            ParentMenu = parentMenu;
            Offset = offset;
            _inventoryType = inventoryType;

            width = (Game1.tileSize + horizontalGap) * ItemsPerRow + (int)Padding.X * 2;
            height = (Game1.tileSize + verticalGap) * rows + (int)Padding.Y * 2;
            showGrayedOutSlots = true;

            allClickableComponents = new List<ClickableComponent>();
            SetupWidgets();
            RefreshItems();
        }

        public override void draw(SpriteBatch b)
        {
            // Draw Dialogue Box
            CommonHelper.DrawDialogueBox(b, xPositionOnScreen, yPositionOnScreen, width, height);

            // Draw Grid
            for (var slot = 0; slot < capacity; ++slot)
            {
                var col = slot % ItemsPerRow;
                var row = slot / ItemsPerRow;
                var pos = Padding + Position + new Vector2(
                    col * (Game1.tileSize + horizontalGap),
                    row * (Game1.tileSize + verticalGap));

                b.Draw(
                    Game1.menuTexture,
                    pos,
                    Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.5f);

                if (slot >= MaxItems)
                {
                    b.Draw(
                        Game1.menuTexture,
                        pos,
                        Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57),
                        Color.White * 0.5f,
                        0.0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0.5f);
                }
            }

            // Draw Items
            for (var slot = 0; slot < Math.Min(capacity, VisibleItems.Count); ++slot)
            {
                var col = slot % ItemsPerRow;
                var row = slot / ItemsPerRow;
                var pos = Padding + new Vector2(
                    xPositionOnScreen + col * (Game1.tileSize + horizontalGap),
                    yPositionOnScreen + row * (Game1.tileSize + verticalGap));

                var currentItem = VisibleItems.ElementAt(slot);
                if (currentItem is null || currentItem.Stack == 0)
                    continue;
                currentItem.drawInMenu(
                    b,
                    pos,
                    1f,
                    1f,
                    0.865f,
                    StackDrawType.Draw,
                    Color.White,
                    false);
            }

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
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // Left Click Widgets
            foreach (var clickableComponent in allClickableComponents
                .Where(c =>
                    c.containsPoint(x, y)
                    && c is IWidget widget
                    && !(widget.LeftClickAction is null)))
            {
                ((IWidget)clickableComponent).LeftClickAction(clickableComponent);
            }
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

            // Scroll Items
            if (direction < 0)
            {
                ScrollDown();
            }
            else if (direction > 0)
            {
                ScrollUp();
            }

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

        public void ScrollDown(ClickableComponent clickableComponent = null)
        {
            if (_currentRow >= MaxRows - rows)
                return;
            ++_currentRow;
            RefreshItems();
        }

        public void ScrollUp(ClickableComponent clickableComponent = null)
        {
            if (_currentRow <= 0)
                return;
            --_currentRow;
            RefreshItems();
        }

        public void RefreshItems()
        {
            var parentMenu = CommonHelper.OfType<IClickableMenu, CustomItemGrabMenu>(ParentMenu);
            if (_inventoryType == InventoryType.Chest)
                MegaStorageApi.InvokeBeforeVisibleItemsRefreshed(parentMenu.CustomChestEventArgs);
            VisibleItems = (_selectedCategory?.Filter(actualInventory) ?? actualInventory)
                .Skip(ItemsPerRow * _currentRow)
                .ToList();
            MaxRows = (int)Math.Ceiling((double)VisibleItems.Count / ItemsPerRow);
            UpArrow.visible = _currentRow > 0;
            DownArrow.visible = _currentRow < MaxRows - rows;
            inventory.Clear();
            for (var slot = 0; slot < capacity; ++slot)
            {
                var col = slot % ItemsPerRow;
                var row = slot / ItemsPerRow;

                // Find actualInventory index
                var index = slot >= VisibleItems.Count
                    ? actualInventory.Count
                    : actualInventory.IndexOf(VisibleItems[slot]);

                inventory.Add(new ClickableComponent(
                    new Rectangle(
                        xPositionOnScreen + col * (Game1.tileSize + horizontalGap) + (int)Padding.X,
                        yPositionOnScreen + row * (Game1.tileSize + verticalGap) + (int)Padding.Y,
                        Game1.tileSize,
                        Game1.tileSize),
                    index.ToString(CultureInfo.InvariantCulture))
                {
                    myID = slot,
                    leftNeighborID = col != 0 ? slot - 1 : 107,
                    rightNeighborID = (slot + 1) % ItemsPerRow != 0 ? slot + 1 : 106,
                    downNeighborID = slot >= actualInventory.Count - capacity / rows ? 102 : slot + ItemsPerRow,
                    upNeighborID = slot < capacity / rows ? 12340 + slot : slot - capacity / rows,
                    region = 9000,
                    upNeighborImmutable = true,
                    downNeighborImmutable = true,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
            }
            if (_inventoryType == InventoryType.Chest)
                MegaStorageApi.InvokeAfterVisibleItemsRefreshed(parentMenu.CustomChestEventArgs);
        }

        /*********
        ** Private methods
        *********/
        private void SetupWidgets()
        {
            // Up Arrow
            UpArrow = new CustomClickableTextureComponent(
                "upArrow",
                this,
                new Vector2(width - Game1.tileSize + 8, 36),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12),
                scale: 1f)
            {
                myID = 88,
                downNeighborID = 89,
                visible = _currentRow > 0,
                LeftClickAction = ScrollUp
            };
            allClickableComponents.Add(UpArrow);

            // Down Arrow
            DownArrow = new CustomClickableTextureComponent(
                "downArrow",
                this,
                new Vector2(width - Game1.tileSize + 8, height - Game1.tileSize - 36),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11),
                scale: 1f)
            {
                myID = 89,
                upNeighborID = 88,
                visible = _currentRow <= MaxRows - rows,
                LeftClickAction = ScrollDown
            };
            allClickableComponents.Add(DownArrow);
        }
    }
}
