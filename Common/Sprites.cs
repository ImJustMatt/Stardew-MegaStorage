using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace MegaStorage
{
    internal static class Sprites
    {
        // Dialogue Box Tiles
        public static class Menu
        {
            /*********
            ** Fields
            *********/
            public static readonly Rectangle Background = GetTile(1, 2);
            public static readonly Rectangle Top = GetTile(2, 0);
            public static readonly Rectangle Right = GetTile(3, 2);
            public static readonly Rectangle Bottom = GetTile(2, 3);
            public static readonly Rectangle Left = GetTile(0, 2);
            public static readonly Rectangle TopRight = GetTile(3, 0);
            public static readonly Rectangle BottomRight = GetTile(3, 3);
            public static readonly Rectangle BottomLeft = GetTile(0, 3);
            public static readonly Rectangle TopLeft = GetTile(0, 0);

            /*********
            ** Public methods
            *********/
            public static void Draw(SpriteBatch b, Rectangle bounds) =>
                Draw(b, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            public static void Draw(SpriteBatch b, int x, int y, int width, int height)
            {
                // Background
                b.Draw(
                    Game1.menuTexture,
                    new Rectangle(
                        x + Game1.tileSize / 2,
                        y + Game1.tileSize / 2,
                        width - Game1.tileSize,
                        height - Game1.tileSize),
                    Background,
                    Color.White);

                // Top Border
                b.Draw(
                    Game1.menuTexture,
                    new Rectangle(
                        x + Game1.tileSize,
                        y,
                        width - Game1.tileSize * 2,
                        Game1.tileSize),
                    Top,
                    Color.White);

                // Bottom Border
                b.Draw(
                    Game1.menuTexture,
                    new Rectangle(
                        x + Game1.tileSize,
                        y + height - Game1.tileSize,
                        width - Game1.tileSize * 2,
                        Game1.tileSize),
                    Bottom,
                    Color.White);

                // Left Border
                b.Draw(
                    Game1.menuTexture,
                    new Rectangle(
                        x,
                        y + Game1.tileSize,
                        Game1.tileSize,
                        height - Game1.tileSize * 2),
                    Left,
                    Color.White);

                // Right Border
                b.Draw(
                    Game1.menuTexture,
                    new Rectangle(
                        x + width - Game1.tileSize,
                        y + Game1.tileSize,
                        Game1.tileSize,
                        height - Game1.tileSize * 2),
                    Right,
                    Color.White);

                // Top-Right Corner
                b.Draw(
                    Game1.menuTexture,
                    new Vector2(x + width - Game1.tileSize, y),
                    TopRight,
                    Color.White);

                // Top-Left Corner
                b.Draw(
                    Game1.menuTexture,
                    new Vector2(x, y),
                    TopLeft,
                    Color.White);

                // Bottom-Right Corner
                b.Draw(
                    Game1.menuTexture,
                    new Vector2(x + width - Game1.tileSize, y + height - Game1.tileSize),
                    BottomRight,
                    Color.White);

                // Bottom-Left Corner
                b.Draw(
                    Game1.menuTexture,
                    new Vector2(x, y + height - Game1.tileSize),
                    BottomLeft,
                    Color.White);
            }
        }

        public static class Inventory
        {
            /*********
            ** Fields
            *********/
            public static readonly Rectangle Grid = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10);
            public static readonly Rectangle GrayedOut = Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57);
            public static readonly Rectangle LeftTab = new Rectangle(16, 368, 12, 16);
            public static readonly Rectangle RightTab = new Rectangle(21, 368, 11, 16);
            public static readonly Rectangle Backpack = new Rectangle(4, 372, 8, 11);

            /*********
            ** Public methods
            *********/
            public static void DrawGrid(SpriteBatch b, Vector2 position, Vector2 dimensions, int maxItems = -1) =>
                DrawGrid(b, (int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y, maxItems);
            public static void DrawGrid(SpriteBatch b, int x, int y, int width, int height, int maxItems = -1)
            {
                var rows = (int)Math.Floor((double)width / Game1.tileSize);
                var cols = (int)Math.Floor((double)height / Game1.tileSize);
                var capacity = rows * cols;
                var horizontalGap = (width - rows * Game1.tileSize) / (rows - 1);
                var verticalGap = (height - cols * Game1.tileSize) / (cols - 1);

                for (var slot = 0; slot < capacity; ++slot)
                {
                    var col = slot % rows;
                    var row = slot / rows;
                    var pos = new Vector2(
                        x + col * (Game1.tileSize + horizontalGap),
                        y + row * (Game1.tileSize + verticalGap));

                    b.Draw(
                        Game1.menuTexture,
                        pos,
                        Grid,
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0.5f);

                    if (maxItems > -1 && slot >= maxItems)
                    {
                        b.Draw(
                            Game1.menuTexture,
                            pos,
                            GrayedOut,
                            Color.White * 0.5f,
                            0.0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            0.5f);
                    }
                }
            }

            public static void DrawBackpack(SpriteBatch b, Vector2 position) =>
                DrawBackpack(b, (int)position.X, (int)position.Y);
            public static void DrawBackpack(SpriteBatch b, int x, int y)
            {
                b.Draw(Game1.mouseCursors,
                    new Vector2(x, y + 60),
                    LeftTab,
                    Color.White,
                    4.712389f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    1f);
                b.Draw(Game1.mouseCursors,
                    new Vector2(x, y + 28),
                    RightTab,
                    Color.White,
                    4.712389f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    1f);
                b.Draw(Game1.mouseCursors,
                    new Vector2(x + 24, y),
                    Backpack,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    SpriteEffects.None,
                    1f);
            }
        }

        public static class Icons
        {
            // Star Button
            public static readonly Rectangle ActiveStarIcon = new Rectangle(310, 392, 16, 16);
            public static readonly Rectangle InactiveStarIcon = new Rectangle(294, 392, 16, 16);
        }

        public static Rectangle GetTile(int x, int y) =>
            new Rectangle(
                Game1.tileSize * x,
                Game1.tileSize * y,
                Game1.tileSize,
                Game1.tileSize);

        public static TemporaryAnimatedSprite CreatePoof(int x, int y) => new TemporaryAnimatedSprite(
            "TileSheets/animations",
            new Rectangle(0, 320, Game1.tileSize, Game1.tileSize),
            50f,
            8,
            0,
            new Vector2(x - x % Game1.tileSize + 16, y - y % Game1.tileSize + 16),
            false,
            false);
    }
}
