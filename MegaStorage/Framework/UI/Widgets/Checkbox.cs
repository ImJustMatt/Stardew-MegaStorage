using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class Checkbox : BaseWidget
    {
        /*********
        ** Fields
        *********/
        public bool IsChecked;
        public Sprite OffSprite;
        public Sprite OnSprite;

        /*********
        ** Public methods
        *********/
        public Checkbox(
            string name,
            IMenu parentMenu,
            Vector2 offset,
            Sprite offSprite,
            Sprite onSprite)
            : base(name, parentMenu, offset, offSprite)
        {
            OffSprite = offSprite;
            OnSprite = onSprite;
            Events.LeftClick = LeftClick;
        }

        /*********
        ** Private methods
        *********/
        private void LeftClick(IWidget widget)
        {
            IsChecked = !IsChecked;
            sourceRect = IsChecked
                ? OnSprite.SourceRect
                : OffSprite.SourceRect;
        }
    }
}
