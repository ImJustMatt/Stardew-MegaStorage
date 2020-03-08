using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace MegaStorage.Framework.UI
{
    internal enum MenuType
    {
        BaseMenu = 0,
        Overlay = 1
    }

    internal interface ISubMenu
    {
        IClickableMenu ParentMenu { get; }
        MenuType MenuType { get; }
        Rectangle Bounds { get; }
        Vector2 Offset { get; }
        Vector2 Position { get; }
        Vector2 Dimensions { get; }
        bool Visible { get; set; }
    }
}