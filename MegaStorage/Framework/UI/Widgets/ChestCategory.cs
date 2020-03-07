using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ChestCategory : CustomClickableTextureComponent
    {
        protected internal Func<Item, bool> BelongsToCategory;

        private const int SelectedOffset = 8;
        public ChestCategory(
            string name,
            CustomInventoryMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect)
            : base(name, parentMenu, offset, texture, sourceRect, MegaStorageMod.ModHelper.Translation.Get($"category.{name}"))
        { }

        public void Draw(SpriteBatch b, bool selected = false)
        {
            bounds.X = ParentMenu.xPositionOnScreen + (int)Offset.X + (selected ? SelectedOffset : 0);
            bounds.Y = ParentMenu.yPositionOnScreen + (int)Offset.Y;
            draw(b);
        }
        public List<Item> Filter(IList<Item> items) => items.Where(BelongsToCategory).ToList();
    }
}