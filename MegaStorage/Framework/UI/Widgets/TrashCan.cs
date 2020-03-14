using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class TrashCan : BaseWidget
    {
        /*********
        ** Fields
        *********/
        public static readonly Vector2 LidOffset = new Vector2(60, 40);
        public static Rectangle TrashCanSourceRect =>
            new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26);
        public static Rectangle LidSourceRect =>
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

            Events.Draw = Draw;
            Events.LeftClick = LeftClick;
            Events.Hover = Hover;
            hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Draws the Trash Can and the lid
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="widget">The trash can being drawn</param>
        protected internal override void Draw(SpriteBatch b, IWidget widget)
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
        protected internal void LeftClick(IWidget widget)
        {
            Utility.trashItem(BaseMenu.heldItem);
            BaseMenu.heldItem = null;
        }

        /// <summary>
        /// Rotates the trash can lid while hovering over the trash can
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="widget">The trash can being hovered over</param>
        protected internal override void Hover(int x, int y, IWidget widget)
        {
            if (!containsPoint(x, y))
            {
                _lidRotation = Math.Max(_lidRotation - (float)Math.PI / 48f, 0.0f);
                return;
            }

            if (_lidRotation <= 0f)
                Game1.playSound("trashcanlid");

            _lidRotation = Math.Min(_lidRotation + (float)Math.PI / 48f, 1.570796f);

            if (BaseMenu.heldItem is null ||
                Utility.getTrashReclamationPrice(BaseMenu.heldItem, Game1.player) <= 0)
            {
                return;
            }

            if (containsPoint(x, y))
            {
                BaseMenu.hoverAmount = BaseMenu.hoverAmount == -1
                    ? Utility.getTrashReclamationPrice(BaseMenu.heldItem, Game1.player)
                    : BaseMenu.hoverAmount;
            }

            base.Hover(x, y, widget);
        }
    }
}
