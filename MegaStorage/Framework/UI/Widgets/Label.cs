﻿using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MegaStorage.Framework.UI.Widgets
{
    public enum Align
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    internal class Label : ClickableComponent, IWidget
    {
        /*********
        ** Fields
        *********/
        public IMenu ParentMenu { get; }
        public Vector2 Offset { get; }
        public Rectangle Bounds
        {
            get => bounds;
            set => bounds = value;
        }
        public Vector2 Position
        {
            get => new Vector2(bounds.X, bounds.Y);
            set
            {
                bounds.X = (int)value.X;
                bounds.Y = (int)value.Y;
            }
        }
        public Vector2 Dimensions
        {
            get => Font.MeasureString(label);
            set
            {
                bounds.Width = (int)value.X;
                bounds.Height = (int)value.Y;
            }
        }

        public Action<SpriteBatch, IWidget> DrawAction { get; set; }
        public Action<IWidget> LeftClickAction { get; set; }
        public Action<IWidget> RightClickAction { get; set; }
        public Action<int, IWidget> ScrollAction { get; set; }
        public Action<int, int, IWidget> HoverAction { get; set; }

        public readonly SpriteFont Font;
        public readonly Align Align;

        /*********
        ** Public methods
        *********/
        public Label(
            string name,
            IMenu parentMenu,
            Vector2 offset,
            string label,
            int width = 0,
            int height = 0,
            Align align = Align.Left,
            SpriteFont font = null)
            : base(new Rectangle((int)(parentMenu.Position.X + offset.X),
                        (int)(parentMenu.Position.Y + offset.Y),
                        width,
                        height),
                    name,
                    label)
        {
            ParentMenu = parentMenu;
            Offset = offset;
            Font = font ?? Game1.dialogueFont;
            Align = align;

            DrawAction = Draw;
        }

        public void GameWindowSizeChanged() =>
            Position = ParentMenu.Position + Offset;

        /*********
        ** Private methods
        *********/
        private void Draw(SpriteBatch b, IWidget widget)
        {
            Utility.drawTextWithShadow(
                b,
                label,
                Game1.dialogueFont,
                Position + Align switch
                {
                    Align.Left => Vector2.Zero,
                    Align.Center => new Vector2((bounds.Width - Dimensions.X) / 2, 0),
                    Align.Right => new Vector2(bounds.Width - Dimensions.X, 0),
                    _ => Vector2.Zero
                },
                Game1.textColor);
        }
    }
}