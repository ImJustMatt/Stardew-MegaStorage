using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;

namespace MegaStorage.Framework.UI.Menus
{
    internal class ItemStackMenu : IClickableMenu, IMenu
    {
        /*********
        ** Fields
        *********/
        public IMenu ParentMenu { get; }
        public Vector2 Offset { get; set; }
        public Rectangle Bounds
        {
            get => new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
            set
            {
                xPositionOnScreen = value.X;
                yPositionOnScreen = value.Y;
                width = value.Width;
                height = value.Height;
            }
        }
        public Vector2 Position
        {
            get => new Vector2(xPositionOnScreen, yPositionOnScreen);
            set
            {
                xPositionOnScreen = (int)value.X;
                yPositionOnScreen = (int)value.Y;
            }
        }
        public Vector2 Dimensions
        {
            get => new Vector2(width, height);
            set
            {
                width = (int)value.X;
                height = (int)value.Y;
            }
        }
        public bool Visible { get; set; } = true;
        public bool FadedBackground => false;
        public IList<IMenu> SubMenus { get; } = new List<IMenu>();
        public IList<IMenu> Overlays { get; } = new List<IMenu>();

        /*********
        ** Public methods
        *********/
        protected ItemStackMenu(IMenu parentMenu, Vector2 offset)
        {
            ParentMenu = parentMenu;
            Offset = offset;
            allClickableComponents = new List<ClickableComponent>();

            Position = ParentMenu.Position + Offset;
        }
    }
}
