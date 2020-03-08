using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class Checkbox : CustomClickableTextureComponent
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
            IClickableMenu parentMenu,
            Vector2 offset)
            : base(name, parentMenu, offset, Game1.mouseCursors, OptionsCheckbox.sourceRectUnchecked)
        {
            LeftClickAction = LeftClick;
        }

        /*********
        ** Private methods
        *********/
        private void LeftClick(ClickableComponent clickableComponent = null)
        {
            IsChecked = !IsChecked;
            sourceRect = IsChecked
                ? OptionsCheckbox.sourceRectChecked
                : OptionsCheckbox.sourceRectUnchecked;
        }
    }
}
