using Microsoft.Xna.Framework;
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
    }
}