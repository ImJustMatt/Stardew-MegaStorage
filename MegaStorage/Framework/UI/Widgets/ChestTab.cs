using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ChestTab : CustomClickableTextureComponent
    {
        /*********
        ** Fields
        *********/
        protected internal Func<Item, bool> BelongsToCategory;

        private const int SelectedOffset = 8;

        /*********
        ** Public methods
        *********/
        public ChestTab(
            string name,
            IClickableMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect)
            : base(name, parentMenu, offset, texture, sourceRect,
                MegaStorageMod.ModHelper.Translation.Get($"category.{name}"))
        {
            DrawAction = Draw;
            LeftClickAction = LeftClick;
        }
        public List<Item> Filter(IList<Item> items) => items.Where(BelongsToCategory).ToList();

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Switches the chest menu's currently selected tab
        /// </summary>
        /// <param name="clickableComponent">The tab button that was clicked</param>
        private void LeftClick(ClickableComponent clickableComponent = null)
        {
            if (!(ParentMenu is CustomInventoryMenu customInventoryMenu))
                return;
            customInventoryMenu.SelectedCategory = this;
        }

        /// <summary>
        /// Draws the tabs to the left of the ItemsToGrabMenu
        /// </summary>
        /// <param name="clickableComponent">The tab button that was clicked</param>
        private void Draw(SpriteBatch b, ClickableComponent clickableComponent = null)
        {
            if (!(ParentMenu is CustomInventoryMenu customInventoryMenu))
                return;
            bounds.X = ParentMenu.xPositionOnScreen + (int)Offset.X +
                       (customInventoryMenu.SelectedCategory.Equals(this) ? SelectedOffset : 0);
            bounds.Y = ParentMenu.yPositionOnScreen + (int)Offset.Y;
            draw(b);
        }
    }
}