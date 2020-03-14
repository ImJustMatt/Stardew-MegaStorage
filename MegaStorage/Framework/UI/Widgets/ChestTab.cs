using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ChestTab : ClickableTexture
    {
        /*********
        ** Fields
        *********/
        public const int SelectedOffset = 8;
        public Func<Item, bool> BelongsToCategory;

        /*********
        ** Public methods
        *********/
        public ChestTab(
            string name,
            IMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect)
            : base(name, parentMenu, offset, texture, sourceRect,
                MegaStorageMod.Helper.Translation.Get($"category.{name}"))
        {
            Events.LeftClick = LeftClick;

            if (!(ParentMenu is ChestInventoryMenu menu))
                return;
            menu.ChestTabChanged += OnChestTabChanged;
        }

        /*********
        ** Private methods
        *********/
        private void LeftClick(IWidget widget)
        {
            if (!(ParentMenu is ChestInventoryMenu menu))
                return;
            menu.CurrentTab = this;
        }

        private void OnChestTabChanged(object sender, EventArgs e)
        {
            if (!(ParentMenu is ChestInventoryMenu menu))
                return;
            bounds.X = (int)(ParentMenu.Position.X + Offset.X) +
                       (menu.CurrentTab.Equals(this) ? SelectedOffset : 0);
        }
    }
}