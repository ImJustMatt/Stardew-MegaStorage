using StardewValley;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal static class WidgetExtensions
    {
        public static void GameWindowSizeChanged(this IWidget widget) =>
            widget.Position = widget.ParentMenu.Position + widget.Offset;

        /// <summary>
        /// Zooms in on the hovered component
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="widget">The item being hovered over</param>
        public static void HoverZoom(int x, int y, IWidget widget)
        {
            if (!(widget is ClickableTexture cc))
                return;
            cc.scale = cc.containsPoint(x, y)
                ? Math.Min(1.1f, cc.scale + 0.05f)
                : Math.Max(1f, cc.scale - 0.05f);
            if (cc.containsPoint(x, y))
                cc.ItemGrabMenu.hoverText ??= cc.hoverText;
        }

        /// <summary>
        /// Zooms in on the hovered component (scaled up by Game1.pixelZoom)
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="widget">The item being hovered over</param>
        public static void HoverPixelZoom(int x, int y, IWidget widget)
        {
            if (!(widget is ClickableTexture cc))
                return;
            cc.scale = cc.containsPoint(x, y)
                ? Math.Min(Game1.pixelZoom * 1.1f, cc.scale + 0.05f)
                : Math.Max(Game1.pixelZoom * 1f, cc.scale - 0.05f);
            if (cc.containsPoint(x, y))
                cc.ItemGrabMenu.hoverText ??= cc.hoverText;
        }
    }
}
