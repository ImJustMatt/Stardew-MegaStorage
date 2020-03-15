using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    internal class PlayerInventory : BaseInventory
    {
        /*********
        ** Fields
        *********/
        public static readonly Vector2 RightWidgetsOffset = new Vector2(24, -32);
        public static readonly Vector2 BackpackIconOffset = new Vector2(-48, 96);

        /*********
        ** Public methods
        *********/
        public PlayerInventory(IMenu parentMenu, Vector2 offset)
            : base(
                parentMenu,
                offset,
                Game1.player.Items,
                Math.Max(3, Game1.player.MaxItems / ItemsPerRow))
        {
            Game1.player.items.OnElementChanged += SyncItem;
            showGrayedOutSlots = true;
            SetupWidgets();
            SyncItems();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            // Backpack Icon
            Sprites.Inventory.DrawBackpack(b, Position + BackpackIconOffset);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (!(HeldItem is null) && isWithinBounds(x, y))
                BaseMenu.BehaviorFunction?.Invoke(HeldItem, Game1.player);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);

            if (!(HeldItem is null) && isWithinBounds(x, y))
                BaseMenu.BehaviorFunction?.Invoke(HeldItem, Game1.player);
        }

        public sealed override void SyncItems()
        {
            foreach (var itemSlot in allClickableComponents.OfType<ItemSlot>())
            {
                itemSlot.item = (itemSlot.Slot < actualInventory.Count)
                    ? actualInventory[itemSlot.Slot]
                    : null;
                itemSlot.visible = itemSlot.Slot < Game1.player.MaxItems;
            }
        }

        /*********
        ** Private methods
        *********/
        private void SetupWidgets()
        {
            // OK Button
            var okButton = new BaseWidget(
                "okButton",
                this,
                RightWidgetsOffset + new Vector2(width, 204),
                Sprites.Icons.Ok)
            {
                myID = 4857,
                upNeighborID = 5948,
                leftNeighborID = 11
            };
            okButton.Events.LeftClick = ClickOkButton;
            okButton.Events.Hover = WidgetEvents.HoverZoom;
            allClickableComponents.Add(okButton);
            BaseMenu.okButton = okButton;

            // Trash Can
            var trashCan = new TrashCan(this, RightWidgetsOffset + new Vector2(width, 68));
            allClickableComponents.Add(trashCan);
            BaseMenu.trashCan = trashCan;
        }

        private void SyncItem(NetList<Item, NetRef<Item>> list, int slot, Item oldValue, Item currentItem)
        {
            var itemSlot = allClickableComponents
                .OfType<ItemSlot>()
                .First(cc => cc.Slot == slot);

            if (itemSlot is null)
                return;

            itemSlot.item = currentItem;
        }

        private void ClickOkButton(IWidget widget)
        {
            BaseMenu.exitThisMenu();
            if (!(Game1.currentLocation.currentEvent is null))
                ++Game1.currentLocation.currentEvent.CurrentCommand;
            Game1.playSound("bigDeSelect");
        }
    }
}
