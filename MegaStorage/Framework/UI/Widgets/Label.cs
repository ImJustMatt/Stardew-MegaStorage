using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MegaStorage.Framework.UI.Widgets
{
    public enum Align
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    internal class Label : ClickableComponent, IWidget
    {
        /*********
        ** Fields
        *********/
        // Custom actions for widgets
        public Action<SpriteBatch, ClickableComponent> DrawAction { get; set; }
        public Action<ClickableComponent> LeftClickAction { get; set; }
        public Action<ClickableComponent> RightClickAction { get; set; }
        public Action<int, ClickableComponent> ScrollAction { get; set; }
        public Action<int, int, ClickableComponent> HoverAction { get; set; }

        // Relative Positioning
        public IClickableMenu ParentMenu { get; }
        public Vector2 Offset { get; }
        public Vector2 Position => new Vector2(bounds.X, bounds.Y);
        public Vector2 Dimensions => _font.MeasureString(label);

        private readonly SpriteFont _font;
        private readonly Align _align;

        /*********
        ** Public methods
        *********/
        public Label(
            string name,
            IClickableMenu parentMenu,
            Vector2 offset,
            string label = "",
            int width = 0,
            int height = 0,
            Align align = Align.Left,
            SpriteFont font = null)
            : base(new Rectangle(parentMenu.xPositionOnScreen + (int)offset.X, parentMenu.yPositionOnScreen + (int)offset.Y, width, height), name, label)
        {
            ParentMenu = parentMenu;
            Offset = offset;
            _font = font ?? Game1.dialogueFont;
            _align = align;

            DrawAction = Draw;
        }

        public void GameWindowSizeChanged()
        {
            bounds.X = ParentMenu.xPositionOnScreen + (int)Offset.X;
            bounds.Y = ParentMenu.yPositionOnScreen + (int)Offset.Y;
        }

        /*********
        ** Private methods
        *********/
        private void Draw(SpriteBatch b, ClickableComponent clickableComponent = null)
        {
            Utility.drawTextWithShadow(
                b,
                label,
                Game1.dialogueFont,
                Position + _align switch
                {
                    Align.Left => Vector2.Zero,
                    Align.Center => new Vector2((bounds.Width - Dimensions.X) / 2, 0),
                    Align.Right => new Vector2(bounds.Width - Dimensions.X, 0),
                    _ => Vector2.Zero
                },
                Game1.textColor);
        }
    }
}
