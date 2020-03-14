using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class Label : BaseWidget
    {
        /*********
        ** Fields
        *********/
        public enum Align
        {
            Left = 0,
            Center = 1,
            Right = 2
        }

        public readonly SpriteFont Font;
        public readonly Align TextAlign;
        public float StringWidth => Font.MeasureString(label).X;

        /*********
        ** Public methods
        *********/
        public Label(
            string name,
            IMenu parentMenu,
            Vector2 offset,
            string label,
            int width = 0,
            int height = 0,
            Align align = Align.Left,
            SpriteFont font = null,
            IWidget forWidget = null)
            : base(name, parentMenu, offset, label, width, height)
        {
            TextAlign = align;
            Font = font ?? Game1.dialogueFont;
            DrawAction = Draw;
            if (!(forWidget is null))
                LeftClickAction = forWidget.LeftClickAction;
        }

        /*********
        ** Private methods
        *********/
        private void Draw(SpriteBatch b, IWidget widget)
        {
            Utility.drawTextWithShadow(
                b,
                label,
                Game1.dialogueFont,
                Position + TextAlign switch
                {
                    Align.Left => Vector2.Zero,
                    Align.Center => new Vector2((bounds.Width - StringWidth) / 2, 0),
                    Align.Right => new Vector2(bounds.Width - StringWidth, 0),
                    _ => Vector2.Zero
                },
                Game1.textColor);
        }
    }
}
