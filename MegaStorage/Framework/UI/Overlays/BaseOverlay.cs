using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;

namespace MegaStorage.Framework.UI.Overlays
{
    internal class BaseOverlay : IClickableMenu, IMenu
    {
        /*********
        ** Fields
        *********/
        public IMenu ParentMenu { get; }
        public MenuType Type { get; } = MenuType.Overlay;
        public Vector2 Offset { get; set; }
        public Vector2 Position
        {
            get => new Vector2(xPositionOnScreen, yPositionOnScreen);
            set
            {
                xPositionOnScreen = (int)value.X;
                yPositionOnScreen = (int)value.Y;
            }
        }
        public bool Visible { get; set; }
        public bool FadedBackground => true;
        public IList<IMenu> SubMenus { get; } = new List<IMenu>();
        protected internal InterfaceHost BaseMenu => CommonHelper.OfType<InterfaceHost>(ParentMenu);
        protected internal ChestInventory ChestInventoryMenu => BaseMenu.ChestInventoryMenu;
        protected internal PlayerInventory PlayerInventoryMenu => BaseMenu.PlayerInventoryMenu;

        /*********
        ** Public methods
        *********/
        public BaseOverlay(IMenu parentMenu, Vector2 offset)
        {
            ParentMenu = parentMenu;
            Offset = offset;
            allClickableComponents = new List<ClickableComponent>();
            Position = ParentMenu.Position + Offset;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.GameWindowSizeChanged(oldBounds, newBounds);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.ReceiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            this.ReceiveRightClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            this.ReceiveScrollWheelAction(direction);
        }

        public override void performHoverAction(int x, int y)
        {
            this.PerformHoverAction(x, y);
        }

        /*********
        ** Private methods
        *********/
    }
}
