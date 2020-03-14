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
        public Vector2 Position
        {
            get => new Vector2(bounds.X, bounds.Y);
            set
            {
                bounds.X = (int)value.X;
                bounds.Y = (int)value.Y;
            }
        }
        public WidgetEvents Events { get; } = new WidgetEvents();
        protected internal IMenu BaseMenu => _baseMenu ??= ParentMenu.BaseMenu();
        protected internal InterfaceHost ItemGrabMenu => CommonHelper.OfType<InterfaceHost>(BaseMenu);

        private IMenu _baseMenu;

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
