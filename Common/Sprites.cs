using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage
{
    internal static class Sprites
    {
        // Dialogue Box
        public static class Menu
        {
            /*********
            ** Fields
            *********/
            public static readonly Sprite Background = new Sprite(Game1.menuTexture, GetTile(1, 2));
            public static readonly Sprite BorderN = new Sprite(Game1.menuTexture, GetTile(2, 0));
            public static readonly Sprite BorderE = new Sprite(Game1.menuTexture, GetTile(3, 2));
            public static readonly Sprite BorderS = new Sprite(Game1.menuTexture, GetTile(2, 3));
            public static readonly Sprite BorderW = new Sprite(Game1.menuTexture, GetTile(0, 2));
            public static readonly Sprite BorderNE = new Sprite(Game1.menuTexture, GetTile(3, 0));
            public static readonly Sprite BorderSE = new Sprite(Game1.menuTexture, GetTile(3, 3));
            public static readonly Sprite BorderSW = new Sprite(Game1.menuTexture, GetTile(0, 3));
            public static readonly Sprite BorderNW = new Sprite(Game1.menuTexture, GetTile(0, 0));

            /*********
            ** Public methods
            *********/
            public static void Draw(SpriteBatch b, Rectangle bounds)
            {
                Draw(b, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }

            public static void Draw(SpriteBatch b, int x, int y, int width, int height)
            {
                // Background
                Background.Draw(b,
                    x + Game1.tileSize / 2,
                    y + Game1.tileSize / 2,
                    width - Game1.tileSize,
                    height - Game1.tileSize);

                BorderN.Draw(b,
                    x + Game1.tileSize,
                    y,
                    width - Game1.tileSize * 2,
                    Game1.tileSize);

                BorderS.Draw(b,
                    x + Game1.tileSize,
                    y + height - Game1.tileSize,
                    width - Game1.tileSize * 2,
                    Game1.tileSize);

                BorderE.Draw(b,
                    x + width - Game1.tileSize,
                    y + Game1.tileSize,
                    Game1.tileSize,
                    height - Game1.tileSize * 2);

                BorderW.Draw(b,
                    x,
                    y + Game1.tileSize,
                    Game1.tileSize,
                    height - Game1.tileSize * 2);

                BorderNW.Draw(b, x, y);
                BorderNE.Draw(b, x + width - Game1.tileSize, y);
                BorderSE.Draw(b, x + width - Game1.tileSize, y + height - Game1.tileSize);
                BorderSW.Draw(b, x, y + height - Game1.tileSize);
            }
        }

        public static class Inventory
        {
            /*********
            ** Fields
            *********/
            public static readonly Sprite Grid = new Sprite(Game1.menuTexture, 10);
            public static readonly Sprite GrayedOut = new Sprite(Game1.menuTexture, 57, Color.White * 0.5f);

            public static readonly Sprite BorderW =
                new Sprite(
                    Game1.mouseCursors,
                    new Rectangle(16, 368, 12, 16),
                    270,
                    Game1.pixelZoom);

            public static readonly Sprite BorderE =
                new Sprite(
                    Game1.mouseCursors,
                    new Rectangle(21, 368, 11, 16),
                    270,
                    Game1.pixelZoom);

            public static readonly Sprite Backpack =
                new Sprite(
                    Game1.mouseCursors,
                    new Rectangle(4, 372, 8, 11),
                    0,
                    Game1.pixelZoom);

            /*********
            ** Public methods
            *********/
            public static void DrawGrid(SpriteBatch b, Vector2 position, Vector2 dimensions, int maxItems = -1)
            {
                DrawGrid(b, (int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y, maxItems);
            }

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

                    Grid.Draw(b, pos);

                    if (maxItems > -1 && slot >= maxItems)
                        GrayedOut.Draw(b, pos);
                }
            }

            public static void DrawBackpack(SpriteBatch b, Vector2 position)
            {
                DrawBackpack(b, (int)position.X, (int)position.Y);
            }

            public static void DrawBackpack(SpriteBatch b, int x, int y)
            {
                BorderW.Draw(b, x, y + 60);
                BorderE.Draw(b, x, y + 28);
                Backpack.Draw(b, x + 24, y);
            }
        }

        public static class Icons
        {
            // General UI
            public static readonly Sprite Ok = new Sprite(Game1.mouseCursors, 46);
            public static readonly Sprite UpArrow = new Sprite(Game1.mouseCursors, 12);
            public static readonly Sprite DownArrow = new Sprite(Game1.mouseCursors, 11);
            public static readonly Sprite LeftArrow = new Sprite(Game1.mouseCursors, 44);
            public static readonly Sprite RightArrow = new Sprite(Game1.mouseCursors, 33);
            public static readonly Sprite Unchecked = new Sprite(Game1.mouseCursors, OptionsCheckbox.sourceRectUnchecked);
            public static readonly Sprite Checked = new Sprite(Game1.mouseCursors, OptionsCheckbox.sourceRectChecked);

            // Inventory Menu
            public static readonly Sprite ColorToggle = new Sprite(Game1.mouseCursors, new Rectangle(119, 469, 16, 16), scale: Game1.pixelZoom);
            public static readonly Sprite FillStacks = new Sprite(Game1.mouseCursors, new Rectangle(103, 469, 16, 16), scale: Game1.pixelZoom);
            public static readonly Sprite Organize = new Sprite(Game1.mouseCursors, new Rectangle(162, 440, 16, 16), scale: Game1.pixelZoom);

            // Star Button
            public static readonly Sprite ActiveStarIcon =
                new Sprite(Game1.mouseCursors, new Rectangle(310, 392, 16, 16), scale: Game1.pixelZoom);

            public static readonly Sprite InactiveStarIcon =
                new Sprite(Game1.mouseCursors, new Rectangle(294, 392, 16, 16), Color.White * 0.5f, Vector2.Zero, scale: Game1.pixelZoom);

            // Trash Can
            public static readonly IList<Sprite> TrashCan = Enumerable.Range(0, 4)
                .Select(n =>
                    new Sprite(Game1.mouseCursors, new Rectangle(564 + 18 * n, 102, 18, 26), scale: Game1.pixelZoom))
                .ToList();

            public static readonly IList<Sprite> TrashCanLid = Enumerable.Range(0, 4)
                .Select(n =>
                    new Sprite(Game1.mouseCursors, new Rectangle(564 + 18 * n, 129, 18, 10), Color.White,
                        new Vector2(16, 10), scale: Game1.pixelZoom))
                .ToList();
        }

        public static Rectangle GetTile(int x, int y)
        {
            return new Rectangle(
                Game1.tileSize * x,
                Game1.tileSize * y,
                Game1.tileSize,
                Game1.tileSize);
        }

        public static TemporaryAnimatedSprite CreatePoof(int x, int y)
        {
            return new TemporaryAnimatedSprite(
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
    internal class Sprite
    {
        /*********
            ** Fields
            *********/
        public Texture2D Texture;
        public Rectangle SourceRect;
        public Color Color;
        public float Rotation;
        public int Scale;
        public Vector2 Origin;

        /*********
            ** Public methods
            *********/
        public Sprite(Texture2D texture, int tilePosition, int rotation = 0, int scale = 1)
            : this(texture, Game1.getSourceRectForStandardTileSheet(texture, tilePosition), Color.White, Vector2.Zero, rotation, scale) { }
        public Sprite(Texture2D texture, int tilePosition, Color color, int rotation = 0, int scale = 1)
            : this(texture, Game1.getSourceRectForStandardTileSheet(texture, tilePosition), color, Vector2.Zero, rotation, scale) { }
        public Sprite(Texture2D texture, Rectangle sourceRect, int rotation = 0, int scale = 1)
            : this(texture, sourceRect, Color.White, Vector2.Zero, rotation, scale) { }
        public Sprite(Texture2D texture, Rectangle sourceRect, Color color, Vector2 origin, int rotation = 0, int scale = 1)
        {
            Texture = texture;
            SourceRect = sourceRect;
            Color = color;
            Rotation = MathHelper.ToRadians(rotation);
            Scale = scale;
            Origin = origin;
        }

        public void Draw(SpriteBatch b, Rectangle destRect)
        {
            b.Draw(
                Texture,
                destRect,
                SourceRect,
                Color,
                Rotation,
                Origin,
                SpriteEffects.None,
                1f);
        }
        public void Draw(SpriteBatch b, Vector2 pos)
        {
            b.Draw(
                Texture,
                pos,
                SourceRect,
                Color,
                Rotation,
                Origin,
                Scale,
                SpriteEffects.None,
                1f);
        }

        public void Draw(SpriteBatch b, int x, int y, int width, int height)
        {
            Draw(b, new Rectangle(x, y, width, height));
        }
        public void Draw(SpriteBatch b, int x, int y)
        {
            Draw(b, new Vector2(x, y));
        }
    }
}
