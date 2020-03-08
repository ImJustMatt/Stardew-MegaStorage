using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;

namespace MegaStorage.Framework.UI.Widgets
{
    internal interface IWidget
    {
        Action<SpriteBatch, ClickableComponent> DrawAction { get; set; }
        Action<ClickableComponent> LeftClickAction { get; set; }
        Action<ClickableComponent> RightClickAction { get; set; }
        Action<int, ClickableComponent> ScrollAction { get; set; }
        Action<int, int, ClickableComponent> HoverAction { get; set; }
        IClickableMenu ParentMenu { get; }
        Vector2 Offset { get; }
        Vector2 Position { get; }
        Vector2 Dimensions { get; }
        void GameWindowSizeChanged();
    }
}
