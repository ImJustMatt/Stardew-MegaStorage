using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class NumberSlider : ClickableComponent, IWidget
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

        /*********
        ** Public methods
        *********/
        public NumberSlider(string name,
            IMenu parentMenu,
            Vector2 offset,
            string label,
            int width = 0,
            int height = 0)
            : base(new Rectangle((int)(parentMenu.Position.X + offset.X),
                    (int)(parentMenu.Position.Y + offset.Y),
                    width,
                    height),
                name,
                label)
        {
            ParentMenu = parentMenu;
            Offset = offset;

            DrawAction = Draw;
        }

        public void GameWindowSizeChanged() =>
            Position = ParentMenu.Position + Offset;

        /*********
        ** Private methods
        *********/
        private void Draw(SpriteBatch b, IWidget widget)
        {

        }
    }
}
