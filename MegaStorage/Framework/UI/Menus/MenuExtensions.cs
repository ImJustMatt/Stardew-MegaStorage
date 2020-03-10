using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    internal static class MenuExtensions
    {
        public static void Draw(this IMenu menu, SpriteBatch b)
        {
            var clickableMenu = CommonHelper.OfType<IMenu, IClickableMenu>(menu);

            // Draw Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.draw(b);
            }

            // Draw Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>())
            {
                if (!(widget.DrawAction is null))
                {
                    widget.DrawAction(b, widget);
                }
                else if (widget is ClickableTextureComponent cc)
                {
                    cc.draw(b);
                }
            }
        }

        public static void GameWindowSizeChanged(this IMenu menu, Rectangle oldBounds, Rectangle newBounds)
        {
            var clickableMenu = CommonHelper.OfType<IMenu, IClickableMenu>(menu);

            // Reposition Self
            menu.Position = menu.Offset +
                            (menu.ParentMenu?.Position ??
                            new Vector2(Game1.viewport.Width, Game1.viewport.Height) / 2f);

            // Reposition Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.gameWindowSizeChanged(oldBounds, newBounds);
            }

            // Reposition Widgets
            foreach (var widget in clickableMenu.allClickableComponents.OfType<IWidget>())
            {
                widget.GameWindowSizeChanged();
            }
        }

        public static void ReceiveLeftClick(this IMenu menu, int x, int y, bool playSound = true)
        {
            var clickableMenu = CommonHelper.OfType<IMenu, IClickableMenu>(menu);

            // Left Click Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => cc.Bounds.Contains(x, y) && !(cc.LeftClickAction is null)))
            {
                widget.LeftClickAction(widget);
            }

            // Left Click Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.receiveLeftClick(x, y, playSound);
            }
        }

        public static void ReceiveRightClick(this IMenu menu, int x, int y, bool playSound = true)
        {
            var clickableMenu = CommonHelper.OfType<IMenu, IClickableMenu>(menu);

            // Right Click Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => cc.Bounds.Contains(x, y) && !(cc.RightClickAction is null)))
            {
                widget.RightClickAction(widget);
            }

            // Right Click Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.receiveRightClick(x, y, playSound);
            }
        }

        public static void ReceiveScrollWheelAction(this IMenu menu, int direction)
        {
            var clickableMenu = CommonHelper.OfType<IMenu, IClickableMenu>(menu);

            var mouseX = Game1.getOldMouseX();
            var mouseY = Game1.getOldMouseY();

            // Scroll Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => cc.Bounds.Contains(mouseX, mouseY) && !(cc.ScrollAction is null)))
            {
                widget.ScrollAction(direction, widget);
            }

            // Scroll Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.receiveScrollWheelAction(direction);
            }
        }

        public static void PerformHoverAction(this IMenu menu, int x, int y)
        {
            var clickableMenu = CommonHelper.OfType<IMenu, IClickableMenu>(menu);

            // Hover Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => !(cc.HoverAction is null)))
            {
                widget.HoverAction(x, y, widget);
            }

            // Hover Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.performHoverAction(x, y);
            }
        }
    }
}
