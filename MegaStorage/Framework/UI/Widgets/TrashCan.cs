using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class TrashCan : ClickableTexture
    {
        /*********
        ** Fields
        *********/
        private static readonly Vector2 LidOffset = new Vector2(60, 40);

        private static Rectangle TrashCanSourceRect =>
            new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26);
        private static Rectangle LidSourceRect =>
            new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10);
        private float _lidRotation;

        /*********
        ** Public methods
        *********/
        public TrashCan(IMenu parentMenu, Vector2 offset)
            : base("trashCan", parentMenu, offset, Game1.mouseCursors, TrashCanSourceRect, "", Game1.tileSize, 104)
        {
            myID = 5948;
            downNeighborID = 4857;
            leftNeighborID = 23;
            upNeighborID = 106;

            DrawAction = Draw;
            LeftClickAction = LeftClick;
            HoverAction = Hover;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Draws the Trash Can and the lid
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="widget">The trash can being drawn</param>
        private void Draw(SpriteBatch b, IWidget widget)
        {
            draw(b);
            b.Draw(
                Game1.mouseCursors,
                Position + LidOffset,
                LidSourceRect,
                Color.White,
                _lidRotation,
                new Vector2(16f, 10f),
                Game1.pixelZoom,
                SpriteEffects.None,
                0.86f);
        }

        /// <summary>
        /// Trashes the currently held item
        /// </summary>
        /// <param name="widget">The trash can that was clicked</param>
        internal void LeftClick(IWidget widget)
        {
            if (!(ParentMenu.ParentMenu is ItemGrabMenu itemGrabMenu) || itemGrabMenu.heldItem is null)
                return;
            Utility.trashItem(itemGrabMenu.heldItem);
            itemGrabMenu.heldItem = null;
        }

        /// <summary>
        /// Rotates the trash can lid while hovering over the trash can
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="widget">The trash can being hovered over</param>
        internal void Hover(int x, int y, IWidget widget)
        {
            if (!(ParentMenu.ParentMenu is ItemGrabMenu itemGrabMenu))
                return;

            if (!Bounds.Contains(x, y))
            {
                _lidRotation = Math.Max(_lidRotation - (float)Math.PI / 48f, 0.0f);
                return;
            }

            if (_lidRotation <= 0f)
                Game1.playSound("trashcanlid");

            _lidRotation = Math.Min(_lidRotation + (float)Math.PI / 48f, 1.570796f);

            if (itemGrabMenu.heldItem is null ||
                Utility.getTrashReclamationPrice(itemGrabMenu.heldItem, Game1.player) <= 0)
            {
                return;
            }

            hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
            HoverNumber = Utility.getTrashReclamationPrice(itemGrabMenu.heldItem, Game1.player);
        }
    }
}
