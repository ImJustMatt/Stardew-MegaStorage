using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    internal static class MenuExtensions
    {
        public static Rectangle GetBounds(this IClickableMenu menu) =>
            new Rectangle(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height);

        public static Vector2 GetDimensions(this IClickableMenu menu) =>
            new Vector2(menu.width, menu.height);
        public static IMenu BaseMenu(this IMenu menu)
        {
            while (true)
            {
                if (menu.Type == MenuType.Base || menu.ParentMenu is null)
                    return menu;
                menu = menu.ParentMenu;
            }
        }
        public static bool Draw(this IMenu menu, SpriteBatch b)
        {
            var done = false;

            if (menu.FadedBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            // Draw Widgets
            foreach (var widget in menu.Widgets())
            {
                widget.DrawAction?.Invoke(b, widget);
            }

            // Draw Menus
            foreach (var subMenu in menu.Menus(true, false))
            {
                CommonHelper.OfType<IClickableMenu>(subMenu).draw(b);
                if (subMenu.Type == MenuType.Overlay)
                    done = true;
            }

            if (menu.Type != MenuType.Base)
                return done;

            var interfaceHost = CommonHelper.OfType<InterfaceHost>(menu);

            // Hover Text
            if (!(interfaceHost.hoveredItem is null))
            {
                // Hover Item
                IClickableMenu.drawToolTip(
                    b,
                    interfaceHost.hoveredItem.getDescription(),
                    interfaceHost.hoveredItem.DisplayName,
                    interfaceHost.hoveredItem,
                    !(interfaceHost.heldItem is null));
            }
            else if (!(interfaceHost.hoverText is null) && interfaceHost.hoverAmount > 0)
            {
                // Hover Text w/Amount
                IClickableMenu.drawToolTip(
                    b,
                    interfaceHost.hoverText,
                    "",
                    null,
                    true,
                    moneyAmountToShowAtBottom: interfaceHost.hoverAmount);
            }
            else if (!(interfaceHost.hoverText is null))
            {
                // Hover Text
                IClickableMenu.drawHoverText(b, interfaceHost.hoverText, Game1.smallFont);
            }

            // Held Item
            interfaceHost.heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            // Game Cursor
            Game1.mouseCursorTransparency = 1f;
            interfaceHost.drawMouse(b);

            return done;
        }

        public static bool GameWindowSizeChanged(this IMenu menu, Rectangle oldBounds, Rectangle newBounds)
        {
            var done = false;

            // Reposition Self
            menu.Position = menu.Offset +
                            (menu.ParentMenu?.Position ??
                            new Vector2(Game1.viewport.Width, Game1.viewport.Height) / 2);

            // Reposition Menus
            foreach (var subMenu in menu.Menus(false))
            {
                CommonHelper.OfType<IClickableMenu>(subMenu).gameWindowSizeChanged(oldBounds, newBounds);
                if (subMenu.Type == MenuType.Overlay)
                    done = true;
            }

            // Reposition Widgets
            foreach (var widget in menu.Widgets(false))
            {
                widget.GameWindowSizeChanged();
            }

            return done;
        }

        public static bool ReceiveLeftClick(this IMenu menu, int x, int y, bool playSound = true)
        {
            // Left Click Menus
            foreach (var subMenu in menu.Menus())
            {
                CommonHelper.OfType<IClickableMenu>(subMenu).receiveLeftClick(x, y, playSound);
                if (subMenu.Type == MenuType.Overlay)
                    return true;
            }

            // Left Click Widgets
            foreach (var widget in menu.Widgets(true, x, y))
            {
                widget.LeftClickAction?.Invoke(widget);
            }

            return false;
        }

        public static bool ReceiveRightClick(this IMenu menu, int x, int y, bool playSound = true)
        {
            // Right Click Menus
            foreach (var subMenu in menu.Menus())
            {
                CommonHelper.OfType<IClickableMenu>(subMenu).receiveRightClick(x, y, playSound);
                if (subMenu.Type == MenuType.Overlay)
                    return true;
            }

            // Right Click Widgets
            foreach (var widget in menu.Widgets(true, x, y))
            {
                widget.RightClickAction?.Invoke(widget);
            }

            return false;
        }

        public static bool ReceiveScrollWheelAction(this IMenu menu, int direction)
        {
            var mouseX = Game1.getOldMouseX();
            var mouseY = Game1.getOldMouseY();

            // Scroll Menus
            foreach (var subMenu in menu.Menus())
            {
                CommonHelper.OfType<IClickableMenu>(subMenu).receiveScrollWheelAction(direction);
                if (subMenu.Type == MenuType.Overlay)
                    return true;
            }

            // Scroll Widgets
            foreach (var widget in menu.Widgets(true, mouseX, mouseY))
            {
                widget.ScrollAction?.Invoke(direction, widget);
            }

            return false;
        }

        public static bool PerformHoverAction(this IMenu menu, int x, int y)
        {
            if (menu is InterfaceHost interfaceHost)
            {
                interfaceHost.hoveredItem = null;
                interfaceHost.hoverText = null;
                interfaceHost.hoverAmount = -1;
            }

            // Hover Menus
            foreach (var subMenu in menu.Menus())
            {
                CommonHelper.OfType<IClickableMenu>(subMenu).performHoverAction(x, y);
                if (subMenu.Type == MenuType.Overlay)
                    return true;
            }

            // Hover Widgets
            foreach (var widget in menu.Widgets())
            {
                widget.HoverAction?.Invoke(x, y, widget);
            }

            return false;
        }

        private static IEnumerable<IWidget> Widgets(this IMenu menu, bool visible = true, int x = -1, int y = -1) =>
            CommonHelper.OfType<IClickableMenu>(menu).allClickableComponents
                .Where(cc => (!visible || cc.visible)
                             && (x == -1 || y == -1 || cc.containsPoint(x, y)))
                .OfType<IWidget>();

        private static IEnumerable<IMenu> Menus(this IMenu menu, bool visible = true, bool reverse = true) =>
            menu.SubMenus
                .Where(m => !visible || m.Visible)
                .OrderBy(m => reverse ? 3 - m.Type : m.Type);
    }
}
