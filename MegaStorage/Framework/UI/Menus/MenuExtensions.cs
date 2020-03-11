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
            var clickableMenu = CommonHelper.OfType<IClickableMenu>(menu);

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

            // Draw Overlay
            foreach (var overlay in menu.Overlays.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                overlay.draw(b);
                break;
            }

            if (!(menu is InterfaceHost interfaceHost))
                return;

            interfaceHost.hoveredItem = menu.HoverItem;
            interfaceHost.hoverText = menu.HoverText;
            interfaceHost.hoverAmount = menu.HoverAmount;

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

            return;
        }

        public static void GameWindowSizeChanged(this IMenu menu, Rectangle oldBounds, Rectangle newBounds)
        {
            var clickableMenu = CommonHelper.OfType<IClickableMenu>(menu);

            // Reposition Self
            menu.Position = menu.Offset +
                            (menu.ParentMenu?.Position ??
                            new Vector2(Game1.viewport.Width, Game1.viewport.Height) / 2);

            // Reposition Overlays
            foreach (var overlay in menu.Overlays.OfType<IClickableMenu>())
            {
                overlay.gameWindowSizeChanged(oldBounds, newBounds);
            }

            // Reposition Menus
            foreach (var subMenu in menu.SubMenus.OfType<IClickableMenu>())
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
            var clickableMenu = CommonHelper.OfType<IClickableMenu>(menu);

            // Left Overlays
            foreach (var overlay in menu.Overlays.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                overlay.receiveLeftClick(x, y, playSound);
                return;
            }

            // Left Click Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.receiveLeftClick(x, y, playSound);
            }

            // Left Click Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => cc.Bounds.Contains(x, y) && !(cc.LeftClickAction is null)))
            {
                widget.LeftClickAction(widget);
            }
        }

        public static void ReceiveRightClick(this IMenu menu, int x, int y, bool playSound = true)
        {
            var clickableMenu = CommonHelper.OfType<IClickableMenu>(menu);

            // Right Click Overlays
            foreach (var overlay in menu.Overlays.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                overlay.receiveRightClick(x, y, playSound);
                return;
            }

            // Right Click Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.receiveRightClick(x, y, playSound);
            }

            // Right Click Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => cc.Bounds.Contains(x, y) && !(cc.RightClickAction is null)))
            {
                widget.RightClickAction(widget);
            }
        }

        public static void ReceiveScrollWheelAction(this IMenu menu, int direction)
        {
            var clickableMenu = CommonHelper.OfType<IClickableMenu>(menu);

            var mouseX = Game1.getOldMouseX();
            var mouseY = Game1.getOldMouseY();

            // Scroll Overlays
            foreach (var overlay in menu.Overlays.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                overlay.receiveScrollWheelAction(direction);
                return;
            }

            // Scroll Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                subMenu.receiveScrollWheelAction(direction);
            }

            // Scroll Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => cc.Bounds.Contains(mouseX, mouseY) && !(cc.ScrollAction is null)))
            {
                widget.ScrollAction(direction, widget);
            }
        }

        public static void PerformHoverAction(this IMenu menu, int x, int y)
        {
            var clickableMenu = CommonHelper.OfType<IClickableMenu>(menu);

            menu.HoverItem = null;
            menu.HoverText = null;
            menu.HoverAmount = -1;

            // Hover Overlays
            foreach (var overlay in menu.Overlays.Where(m => m.Visible))
            {
                var clickableOverlay = CommonHelper.OfType<IClickableMenu>(overlay);
                clickableOverlay.performHoverAction(x, y);
                menu.HoverItem = overlay.HoverItem;
                menu.HoverText = overlay.HoverText;
                menu.HoverAmount = overlay.HoverAmount;
                return;
            }

            // Hover Menus
            foreach (var subMenu in menu.SubMenus.Where(m => m.Visible))
            {
                var clickableSubMenu = CommonHelper.OfType<IClickableMenu>(subMenu);
                clickableSubMenu.performHoverAction(x, y);
                menu.HoverItem ??= subMenu.HoverItem;
                menu.HoverText ??= subMenu.HoverText;
                menu.HoverAmount = menu.HoverAmount == -1
                    ? subMenu.HoverAmount
                    : menu.HoverAmount;
            }

            // Hover Widgets
            foreach (var widget in clickableMenu.allClickableComponents
                .Where(cc => cc.visible)
                .OfType<IWidget>()
                .Where(cc => !(cc.HoverAction is null)))
            {
                widget.HoverAction(x, y, widget);
            }
        }
    }
}
