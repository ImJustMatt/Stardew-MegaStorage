using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class BaseWidget : ClickableTextureComponent, IWidget
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
        public Color Color { get; set; } = Color.White;
        protected internal InterfaceHost BaseMenu =>
            _baseMenu ??= CommonHelper.OfType<InterfaceHost>(ParentMenu.BaseMenu());
        private InterfaceHost _baseMenu;

        /*********
        ** Public methods
        *********/
        public BaseWidget(string name, IMenu parentMenu, Vector2 offset, string label, int width = 0, int height = 0)
            : base(name,
                new Rectangle((int)(parentMenu.Position.X + offset.X),
                    (int)(parentMenu.Position.Y + offset.Y),
                    width,
                    height),
                label, null, null, Rectangle.Empty, 1f)
        {
            ParentMenu = parentMenu;
            Offset = offset;
            Events.Draw = Draw;
            Events.Hover = Hover;
        }

        public BaseWidget(
            string name,
            IMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle sourceRect,
            string hoverText = null,
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
            Events.Draw = Draw;
            Events.Hover = Hover;
        }

        /*********
        ** Private methods
        *********/
        protected internal virtual void Draw(SpriteBatch b, IWidget widget)
        {
            if (!(item is null))
                drawItem(b);
            else
                draw(b, Color, 0.860000014305115f + bounds.Y / 20000f);
        }

        protected internal virtual void Hover(int x, int y, IWidget widget)
        {
            if (!containsPoint(x, y))
                return;

            BaseMenu.hoverText ??= hoverText;
            BaseMenu.hoveredItem ??= item;
        }
    }
}
