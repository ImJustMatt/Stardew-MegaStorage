using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MegaStorage.Framework.Models;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ChestCategory : CustomClickableTextureComponent
    {
        protected internal Func<Item, bool> BelongsToCategory;

        private const int SelectedOffset = 8;
        private readonly IList<int> _includes;
        private readonly IList<int> _excludes;

        public ChestCategory(
            string name,
            CustomInventoryMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect,
            StashConfig categoryConfig)
            : base(name, parentMenu, offset, texture, sourceRect, MegaStorageMod.ModHelper.Translation.Get($"category.{name}"))
        {
            _includes = categoryConfig.IncludesAsList;
            _excludes = categoryConfig.ExcludesAsList;
        }

        public void Draw(SpriteBatch b, bool selected = false)
        {
            bounds.X = ParentMenu.xPositionOnScreen + (int)Offset.X + (selected ? SelectedOffset : 0);
            bounds.Y = ParentMenu.yPositionOnScreen + (int)Offset.Y;
            draw(b);
        }
        public List<Item> Filter(IList<Item> items) => items.Where(BelongsToCategory).ToList();

        public bool BelongsToCategoryDefault(Item i) =>
            !(i is null)
            && (_includes is null || _includes.Contains(i.Category) || _includes.Contains(i.ParentSheetIndex))
            && (_excludes is null || !(_excludes.Contains(i.Category) || _excludes.Contains(i.ParentSheetIndex)));
    }
}