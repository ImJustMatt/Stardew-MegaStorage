using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class CustomClickableTextureComponent : ClickableTextureComponent, IWidget
    {
        // Custom actions for widgets
        public Action<SpriteBatch, ClickableComponent> DrawAction { get; set; }
        public Action<ClickableComponent> LeftClickAction { get; set; }
        public Action<ClickableComponent> RightClickAction { get; set; }
        public Action<int, ClickableComponent> ScrollAction { get; set; }
        public Action<int, int, ClickableComponent> HoverAction { get; set; }

        // Relative Positioning
        public IClickableMenu ParentMenu { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 Position => new Vector2(bounds.X, bounds.Y);
        public Vector2 Dimensions => new Vector2(bounds.Width, bounds.Height);

        public CustomClickableTextureComponent(
            string name,
            IClickableMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect,
            string hoverText = "",
            int width = Game1.tileSize,
            int height = Game1.tileSize,
            float scale = Game1.pixelZoom)
            : base(name, new Rectangle(parentMenu.xPositionOnScreen + (int)offset.X, parentMenu.yPositionOnScreen + (int)offset.Y, width, height), "", hoverText, texture, sourceRect, scale)
        {
            ParentMenu = parentMenu;
            Offset = offset;
        }

        public void GameWindowSizeChanged()
        {
            bounds.X = ParentMenu.xPositionOnScreen + (int)Offset.X;
            bounds.Y = ParentMenu.yPositionOnScreen + (int)Offset.Y;
        }
    }
}
