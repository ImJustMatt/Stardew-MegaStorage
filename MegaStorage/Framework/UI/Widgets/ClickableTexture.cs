using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ClickableTexture : ClickableTextureComponent, IWidget
    {
        /*********
        ** Fields
        *********/
        public IMenu ParentMenu { get; }
        public Vector2 Offset { get; }
        public Rectangle Bounds
        {
            get => bounds;
            set => bounds = value;
        }
        public Vector2 Position
        {
            get => new Vector2(bounds.X, bounds.Y);
            set
            {
                bounds.X = (int)value.X;
                bounds.Y = (int)value.Y;
            }
        }
        public Vector2 Dimensions
        {
            get => new Vector2(bounds.Width, bounds.Height);
            set
            {
                bounds.Width = (int)value.X;
                bounds.Height = (int)value.Y;
            }
        }

        public Action<SpriteBatch, IWidget> DrawAction { get; set; }
        public Action<IWidget> LeftClickAction { get; set; }
        public Action<IWidget> RightClickAction { get; set; }
        public Action<int, IWidget> ScrollAction { get; set; }
        public Action<int, int, IWidget> HoverAction { get; set; }

        public int HoverNumber;

        /*********
        ** Public methods
        *********/
        public ClickableTexture(
            string name,
            IMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect,
            string hoverText = "",
            int width = Game1.tileSize,
            int height = Game1.tileSize,
            float scale = Game1.pixelZoom)
            : base(name,
                new Rectangle((int)(parentMenu.Position.X + offset.X),
                    (int)(parentMenu.Position.Y + offset.Y),
                    width,
                    height),
                "", hoverText, texture, sourceRect, scale)
        {
            ParentMenu = parentMenu;
            Offset = offset;
        }

        public void GameWindowSizeChanged() =>
            Position = ParentMenu.Position + Offset;

        /*********
        ** Private methods
        *********/
    }
}
