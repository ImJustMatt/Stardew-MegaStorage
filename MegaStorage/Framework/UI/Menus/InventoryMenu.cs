using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    internal abstract class InventoryMenu : StardewValley.Menus.InventoryMenu, IMenu
    {
        /*********
        ** Fields
        *********/
        public const int ItemsPerRow = 12;
        public static Vector2 Padding = new Vector2(56, 44);

        public IMenu ParentMenu { get; }
        public Vector2 Offset { get; set; }
        public Rectangle Bounds
        {
            get => new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
            set
            {
                xPositionOnScreen = value.X;
                yPositionOnScreen = value.Y;
                width = value.Width;
                height = value.Height;
            }
        }
        public Vector2 Position
        {
            get => new Vector2(xPositionOnScreen, yPositionOnScreen);
            set
            {
                xPositionOnScreen = (int)value.X;
                yPositionOnScreen = (int)value.Y;
            }
        }
        public Vector2 Dimensions
        {
            get => new Vector2(width, height);
            set
            {
                width = (int)value.X;
                height = (int)value.Y;
            }
        }
        public bool Visible { get; set; } = true;
        public IList<IMenu> SubMenus { get; } = new List<IMenu>();

        /*********
        ** Public methods
        *********/
        protected InventoryMenu(
            IMenu parentMenu,
            Vector2 offset,
            IList<Item> items,
            int rows)
            : base(
                    (int)(parentMenu.Position.X + offset.X),
                    (int)(parentMenu.Position.Y + offset.Y),
                    false,
                    items,
                    InventoryMenu.highlightAllItems,
                    rows * ItemsPerRow,
                    rows)
        {
            ParentMenu = parentMenu;
            Offset = offset;
            width = (Game1.tileSize + horizontalGap) * ItemsPerRow + (int)Padding.X * 2;
            height = (Game1.tileSize + verticalGap) * rows + (int)Padding.Y * 2;
            allClickableComponents = new List<ClickableComponent>();
            SetupItemSlots();
        }

        public override void draw(SpriteBatch b)
        {
            // Draw Dialogue Box
            CommonHelper.DrawDialogueBox(b, Bounds);

            // Draw Grid
            CommonHelper.DrawInventoryGrid(b,
                Position + Padding,
                Dimensions - Padding * 2);

            this.Draw(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) =>
            this.GameWindowSizeChanged(oldBounds, newBounds);
        public override void receiveLeftClick(int x, int y, bool playSound = true) =>
            this.ReceiveLeftClick(x, y, playSound);
        public override void receiveRightClick(int x, int y, bool playSound = true) =>
            this.ReceiveRightClick(x, y, playSound);
        public override void receiveScrollWheelAction(int direction) =>
            this.ReceiveScrollWheelAction(direction);
        public override void performHoverAction(int x, int y) =>
            this.PerformHoverAction(x, y);
        public abstract void SyncItems();

        /*********
        ** Private methods
        *********/
        private void SetupItemSlots()
        {
            for (var slot = 0; slot < capacity; ++slot)
            {
                var col = slot % ItemsPerRow;
                var row = slot / ItemsPerRow;
                var itemWidget = new ClickableTexture(
                    slot.ToString(CultureInfo.InvariantCulture),
                    this,
                    Padding + new Vector2(
                        col * (Game1.tileSize + horizontalGap),
                        row * (Game1.tileSize + verticalGap)),
                    Game1.menuTexture,
                    Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57),
                    "",
                    Game1.tileSize,
                    Game1.tileSize,
                    1f)
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
                };

                allClickableComponents.Add(itemWidget);
            }
        }
    }
}
