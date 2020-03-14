using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ItemSlot : ClickableComponent, IWidget
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
        public int Slot { get; set; }
        protected internal Menus.InventoryMenu InventoryMenu => CommonHelper.OfType<Menus.InventoryMenu>(ParentMenu);
        protected internal InterfaceHost ItemGrabMenu => CommonHelper.OfType<InterfaceHost>(ParentMenu.ParentMenu);

        /*********
        ** Public methods
        *********/
        public ItemSlot(
            int slot,
            IMenu parentMenu,
            Vector2 offset,
            Item item = null,
            int width = Game1.tileSize,
            int height = Game1.tileSize)
            : base(new Rectangle((int)(parentMenu.Position.X + offset.X),
                    (int)(parentMenu.Position.Y + offset.Y),
                    width,
                    height),
                item)
        {
            Slot = slot;
            ParentMenu = parentMenu;
            Offset = offset;
            DrawAction = Draw;
            LeftClickAction = Click;
            RightClickAction = RightClick;
            HoverAction = Hover;
        }

        public void GameWindowSizeChanged() =>
            Position = ParentMenu.Position + Offset;

        public void DrawGrayedOut(SpriteBatch b) =>
            b.Draw(
                Game1.menuTexture,
                Position,
                Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57),
                Color.White * 0.5f);

        /*********
        ** Private methods
        *********/
        private void Draw(SpriteBatch b, IWidget widget)
        {
            item?.drawInMenu(
                b,
                Position,
                1f,
                1f,
                0.9f,
                StackDrawType.Draw,
                Color.White,
                false);
        }

        private void Click(IWidget widget)
        {
            InventoryMenu.ItemSlot ??= this;
        }

        private void RightClick(IWidget widget)
        {
            InventoryMenu.ItemSlot ??= this;
        }

        private void Hover(int x, int y, IWidget widget)
        {
            if (!Bounds.Contains(x, y))
                return;

            ItemGrabMenu.hoveredItem ??= item;
        }
    }
}
