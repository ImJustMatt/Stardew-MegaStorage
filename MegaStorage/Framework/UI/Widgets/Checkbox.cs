using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class Checkbox : ClickableTexture
    {
        /*********
        ** Fields
        *********/
        public bool IsChecked;

        /*********
        ** Public methods
        *********/
        public Checkbox(
            string name,
            IMenu parentMenu,
            Vector2 offset)
            : base(name, parentMenu, offset, Game1.mouseCursors, OptionsCheckbox.sourceRectUnchecked)
        {
            LeftClickAction = LeftClick;
        }

        /*********
        ** Private methods
        *********/
        private void LeftClick(IWidget widget)
        {
            IsChecked = !IsChecked;
            sourceRect = IsChecked
                ? OptionsCheckbox.sourceRectChecked
                : OptionsCheckbox.sourceRectUnchecked;
        }
    }
}
