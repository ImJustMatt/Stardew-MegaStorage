using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal interface IWidget
    {
        IMenu ParentMenu { get; }
        Vector2 Offset { get; }
        Rectangle Bounds { get; }
        Vector2 Position { get; set; }
        Action<SpriteBatch, IWidget> DrawAction { get; set; }
        Action<IWidget> LeftClickAction { get; set; }
        Action<IWidget> RightClickAction { get; set; }
        Action<int, IWidget> ScrollAction { get; set; }
        Action<int, int, IWidget> HoverAction { get; set; }
    }
}
