using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    internal class PlayerInventoryMenu : InventoryMenu
    {
        /*********
        ** Fields
        *********/
        public static readonly Vector2 RightWidgetsOffset = new Vector2(24, -32);
        public static readonly Vector2 BackpackIconOffset = new Vector2(-48, 96);

        /*********
        ** Public methods
        *********/
        public PlayerInventoryMenu(IMenu parentMenu, Vector2 offset)
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
            CommonHelper.DrawInventoryIcon(b, Position + BackpackIconOffset);
        }

        public sealed override void SyncItems()
        {
            for (var slot = 0; slot < capacity; ++slot)
            {
                var itemSlot = allClickableComponents
                    .OfType<ItemSlot>()
                    .Single(cc => cc.Slot == slot);

                itemSlot.item = (slot < actualInventory.Count)
                    ? actualInventory.ElementAt(slot)
                    : null;
            }
        }

        /*********
        ** Private methods
        *********/
        private void SetupWidgets()
        {
            // OK Button
            ItemGrabMenu.okButton = new ClickableTexture(
                "okButton",
                this,
                RightWidgetsOffset + new Vector2(width, 204),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
                scale: 1f)
            {
                myID = 4857,
                upNeighborID = 5948,
                leftNeighborID = 11,
                LeftClickAction = ClickOkButton,
                HoverAction = CommonHelper.HoverZoom
            };
            allClickableComponents.Add(ItemGrabMenu.okButton);

            // Trash Can
            ItemGrabMenu.trashCan = new TrashCan(this, RightWidgetsOffset + new Vector2(width, 68));
            allClickableComponents.Add(ItemGrabMenu.trashCan);
        }

        private void SyncItem(NetList<Item, NetRef<Item>> list, int slot, Item oldValue, Item currentItem)
        {
            var itemSlot = allClickableComponents
                .OfType<ItemSlot>()
                .Single(cc => cc.Slot == slot);
            itemSlot.item = currentItem;
        }

        private void ClickOkButton(IWidget widget)
        {
            ItemGrabMenu.exitThisMenu();
            if (!(Game1.currentLocation.currentEvent is null))
                ++Game1.currentLocation.currentEvent.CurrentCommand;
            Game1.playSound("bigDeSelect");
        }
    }
}
