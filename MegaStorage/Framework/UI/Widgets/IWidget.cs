using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using StardewValley;

namespace MegaStorage.Framework.UI.Widgets
{
    internal interface IWidget
    {
        IMenu ParentMenu { get; }
        Vector2 Offset { get; }
        Vector2 Position { get; set; }
        WidgetEvents Events { get; }
    }

    internal class WidgetEvents
    {
        public Action<SpriteBatch, IWidget> Draw { get; set; }
        public Action<IWidget> LeftClick { get; set; }
        public Action<IWidget> RightClick { get; set; }
        public Action<int, IWidget> Scroll { get; set; }
        public Action<int, int, IWidget> Hover { get; set; }

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
