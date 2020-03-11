using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace MegaStorage.Framework.UI.Menus
{
    internal interface IMenu
    {
        IMenu ParentMenu { get; }
        Vector2 Offset { get; set; }
        Rectangle Bounds { get; set; }
        Vector2 Position { get; set; }
        Vector2 Dimensions { get; set; }
        bool Visible { get; set; }
        IList<IMenu> SubMenus { get; }
        IList<IMenu> Overlays { get; }
        Item HoverItem { get; set; }
        string HoverText { get; set; }
        int HoverAmount { get; set; }
    }
}