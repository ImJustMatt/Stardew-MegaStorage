using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class Checkbox : ClickableTexture
    {
        /*********
        ** Fields
        *********/
        public bool IsChecked;
        public Rectangle OffState;
        public Rectangle OnState;

        /*********
        ** Public methods
        *********/
        public Checkbox(
            string name,
            IMenu parentMenu,
            Vector2 offset,
            Texture2D texture,
            Rectangle offState,
            Rectangle onState)
            : base(name, parentMenu, offset, texture, offState)
        {
            OffState = offState;
            OnState = onState;
            Events.LeftClick = LeftClick;
        }

        /*********
        ** Private methods
        *********/
        private void LeftClick(IWidget widget)
        {
            IsChecked = !IsChecked;
            sourceRect = IsChecked
                ? OnState
                : OffState;
        }
    }
}
