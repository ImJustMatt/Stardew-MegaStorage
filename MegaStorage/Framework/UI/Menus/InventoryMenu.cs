using MegaStorage.Framework.Models;
using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    internal abstract class InventoryMenu : StardewValley.Menus.InventoryMenu, IMenu
    {
        /*********
        ** Fields
        *********/
        public const int ItemsPerRow = 12;
        public static readonly Vector2 Padding = new Vector2(56, 44);

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
        public bool FadedBackground => false;
        public IList<IMenu> SubMenus { get; } = new List<IMenu>();
        public IList<IMenu> Overlays { get; } = new List<IMenu>();
        public ItemSlot ItemSlot { get; set; }
        protected internal InterfaceHost ItemGrabMenu => CommonHelper.OfType<InterfaceHost>(ParentMenu);
        protected internal CustomChest ActualChest => CommonHelper.OfType<CustomChest>(ItemGrabMenu.context);
        protected internal Item HeldItem
        {
            get => ItemGrabMenu.heldItem;
            set => ItemGrabMenu.heldItem = value;
        }

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

            if (showGrayedOutSlots)
            {
                for (var slot = Game1.player.MaxItems; slot < capacity; ++slot)
                {
                    var itemSlot = allClickableComponents
                        .OfType<ItemSlot>()
                        .Single(cc => cc.Slot == slot);
                    itemSlot.DrawGrayedOut(b);
                }
            }

            this.Draw(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) =>
            this.GameWindowSizeChanged(oldBounds, newBounds);

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            ItemSlot = null;

            this.ReceiveLeftClick(x, y, playSound);

            if (ItemSlot is null)
                return;

            if (HeldItem != null && ItemSlot.item != null && ItemSlot.item.canStackWith(HeldItem))
            {
                if (playSound)
                    Game1.playSound("stoneStep");
                HeldItem = Utility.addItemToInventory(HeldItem, ItemSlot.Slot, actualInventory, onAddItem);
                ItemSlot.item = actualInventory[ItemSlot.Slot];
            }
            else if (HeldItem == null && ItemSlot.item != null)
            {
                if (playSound)
                    Game1.playSound("dwop");
                HeldItem = Utility.removeItemFromInventory(ItemSlot.Slot, actualInventory);
                ItemSlot.item = null;
            }
            else if (HeldItem != null && ItemSlot.item == null)
            {
                if (playSound)
                    Game1.playSound("stoneStep");
                HeldItem = Utility.addItemToInventory(HeldItem, ItemSlot.Slot, actualInventory, onAddItem);
                ItemSlot.item = actualInventory[ItemSlot.Slot];
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            ItemSlot = null;

            this.ReceiveRightClick(x, y, playSound && ItemGrabMenu.playRightClickSound);

            if (ItemSlot is null)
                return;

            if (HeldItem != null && ItemSlot.item != null && ItemSlot.item.canStackWith(HeldItem))
            {
                if (playSound)
                    Game1.playSound("stoneStep");
                HeldItem = Utility.addItemToInventory(HeldItem, ItemSlot.Slot, actualInventory, onAddItem);
                ItemSlot.item = actualInventory[ItemSlot.Slot];
            }
            else if (HeldItem == null && ItemSlot.item != null)
            {
                if (playSound)
                    Game1.playSound("dwop");
                HeldItem = Utility.removeItemFromInventory(ItemSlot.Slot, actualInventory);
                ItemSlot.item = null;
            }
            else if (HeldItem != null && ItemSlot.item == null)
            {
                if (playSound)
                    Game1.playSound("stoneStep");
                HeldItem = Utility.addItemToInventory(HeldItem, ItemSlot.Slot, actualInventory, onAddItem);
                ItemSlot.item = actualInventory[ItemSlot.Slot];
            }
        }

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
                var itemSlot = new ItemSlot(
                    slot,
                    this,
                    Padding + new Vector2(
                        col * (Game1.tileSize + horizontalGap),
                        row * (Game1.tileSize + verticalGap)))
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

                allClickableComponents.Add(itemSlot);
            }
        }
    }
}
