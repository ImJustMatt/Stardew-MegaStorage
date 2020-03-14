using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class BaseWidget : ClickableComponent, IWidget
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
        public Action<SpriteBatch, IWidget> DrawAction { get; set; }
        public Action<IWidget> LeftClickAction { get; set; }
        public Action<IWidget> RightClickAction { get; set; }
        public Action<int, IWidget> ScrollAction { get; set; }
        public Action<int, int, IWidget> HoverAction { get; set; }

        /*********
        ** Public methods
        *********/
        public BaseWidget(string name,
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
        }

        /*********
        ** Private methods
        *********/
    }
}
