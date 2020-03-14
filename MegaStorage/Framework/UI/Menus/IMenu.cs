using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MegaStorage.Framework.UI.Menus
{
    public enum MenuType
    {
        Base = 0,
        Sub = 1,
        Overlay = 2
    }

    internal interface IMenu
    {
        IMenu ParentMenu { get; }
        MenuType Type { get; }
        Vector2 Offset { get; set; }
        Vector2 Position { get; set; }
        bool Visible { get; set; }
        bool FadedBackground { get; }
        IList<IMenu> SubMenus { get; }
    }
}