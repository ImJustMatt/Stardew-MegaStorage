using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Globalization;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    internal class PlayerInventoryMenu : InventoryMenu
    {
        /*********
        ** Fields
        *********/
        private static readonly Vector2 RightWidgetsOffset = new Vector2(24, -32);
        private static readonly Vector2 BackpackIconOffset = new Vector2(-48, 96);

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
                var itemWidget = allClickableComponents
                    .OfType<ClickableTexture>()
                    .Single(cc =>
                        cc.name.Equals(slot.ToString(CultureInfo.InvariantCulture),
                            StringComparison.InvariantCultureIgnoreCase));

                var currentItem = (slot < actualInventory.Count)
                    ? actualInventory.ElementAt(slot)
                    : null;

                if (!(currentItem is null))
                {
                    itemWidget.hoverText = currentItem.DisplayName;
                    itemWidget.HoverNumber = currentItem.Stack;
                    itemWidget.texture = Game1.objectSpriteSheet;
                    itemWidget.sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, currentItem.ParentSheetIndex, 16, 16);
                    itemWidget.scale = Game1.pixelZoom;
                    itemWidget.visible = true;
                }
                else if (slot < Game1.player.MaxItems)
                {
                    itemWidget.hoverText = "";
                    itemWidget.HoverNumber = -1;
                    itemWidget.texture = Game1.menuTexture;
                    itemWidget.sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57);
                    itemWidget.scale = 1f;
                    itemWidget.visible = false;
                }
                else
                {
                    itemWidget.visible = true;
                }
            }
        }

        /*********
        ** Private methods
        *********/
        private void SetupWidgets()
        {
            if (!(ParentMenu is ItemGrabMenu itemGrabMenu))
                return;

            // OK Button
            itemGrabMenu.okButton = new ClickableTexture(
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
            allClickableComponents.Add(itemGrabMenu.okButton);

            // Trash Can
            itemGrabMenu.trashCan = new TrashCan(this, RightWidgetsOffset + new Vector2(width, 68));
            allClickableComponents.Add(itemGrabMenu.trashCan);
        }

        private void SyncItem(NetList<Item, NetRef<Item>> list, int slot, Item oldValue, Item currentItem)
        {
            var itemWidget = allClickableComponents
                .OfType<ClickableTexture>()
                .Single(cc =>
                    cc.name.Equals(slot.ToString(CultureInfo.InvariantCulture),
                        StringComparison.InvariantCultureIgnoreCase));

            if (!(currentItem is null))
            {
                itemWidget.hoverText = currentItem.DisplayName;
                itemWidget.HoverNumber = currentItem.Stack;
                itemWidget.texture = Game1.objectSpriteSheet;
                itemWidget.sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, currentItem.ParentSheetIndex, 16, 16);
                itemWidget.scale = Game1.pixelZoom;
            }
            else
            {
                itemWidget.hoverText = "";
                itemWidget.HoverNumber = -1;
                itemWidget.texture = Game1.menuTexture;
                itemWidget.sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57);
                itemWidget.scale = 1f;
            }
        }

        private void ClickOkButton(IWidget widget)
        {
            var itemGrabMenu = CommonHelper.OfType<IMenu, ItemGrabMenu>(ParentMenu);
            itemGrabMenu.exitThisMenu();
            if (!(Game1.currentLocation.currentEvent is null))
                ++Game1.currentLocation.currentEvent.CurrentCommand;
            Game1.playSound("bigDeSelect");
        }
    }
}
