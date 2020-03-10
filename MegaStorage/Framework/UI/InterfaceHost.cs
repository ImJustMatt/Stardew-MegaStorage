using MegaStorage.Framework.Models;
using MegaStorage.Framework.UI.Menus;
using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InventoryMenu = MegaStorage.Framework.UI.Menus.InventoryMenu;

namespace MegaStorage.Framework.UI
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class InterfaceHost : ItemGrabMenu, IMenu
    {
        /*********
        ** Fields
        *********/
        public const int MenuWidth = 840;
        public const int MenuHeight = 736;

        public IMenu ParentMenu { get; } = null;
        public Vector2 Offset { get; set; } = -new Vector2(MenuWidth, MenuHeight) / 2f;
        public Rectangle Bounds
        {
            get => new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
            set
            {
                xPositionOnScreen = value.X;
                yPositionOnScreen = value.Y;
                width = value.Width;
                height = value.Height;
            }
        }
        public Vector2 Position
        {
            get => new Vector2(xPositionOnScreen, yPositionOnScreen);
            set
            {
                xPositionOnScreen = (int)value.X;
                yPositionOnScreen = (int)value.Y;
            }
        }
        public Vector2 Dimensions
        {
            get => new Vector2(width, height);
            set
            {
                width = (int)value.X;
                height = (int)value.Y;
            }
        }

        public bool Visible { get; set; } = true;

        internal new ChestInventoryMenu ItemsToGrabMenu { get; private set; }
        internal new PlayerInventoryMenu inventory { get; private set; }
        internal ItemPickMenu ItemPickMenu { get; private set; }

        private static readonly Vector2 ChestInventoryMenuOffset = new Vector2(-44, -68);
        private static readonly Vector2 ChestColorPickerOffset = new Vector2(32, -72);
        private static readonly Vector2 RightWidgetsOffset = new Vector2(24, -32);

        private TemporaryAnimatedSprite Poof
        {
            set => _poofReflected.SetValue(value);
        }

        private readonly IReflectedField<TemporaryAnimatedSprite> _poofReflected;

        public behaviorOnItemSelect BehaviorFunction
        {
            get => _behaviorFunction.GetValue();
            set => _behaviorFunction.SetValue(value);
        }
        private readonly IReflectedField<behaviorOnItemSelect> _behaviorFunction;

        public IList<IMenu> SubMenus { get; } = new List<IMenu>();
        public IList<IMenu> Overlays { get; } = new List<IMenu>();

        /*********
        ** Public methods
        *********/
        public InterfaceHost(CustomChest customChest)
            : base(CommonHelper.NonNull(customChest).items, customChest)
        {
            // Setup Position
            initialize(
                (Game1.viewport.Width - MenuWidth) / 2,
                (Game1.viewport.Height - MenuHeight) / 2,
                MenuWidth,
                MenuHeight);

            // Setup Properties
            allClickableComponents = new List<ClickableComponent>();
            playRightClickSound = true;
            allowRightClick = true;
            canExitOnKey = true;
            _poofReflected = MegaStorageMod.Helper.Reflection.GetField<TemporaryAnimatedSprite>(this, "poof");
            _behaviorFunction = MegaStorageMod.Helper.Reflection.GetField<behaviorOnItemSelect>(this, "behaviorFunction");

            SetupMenus();
            SetupWidgets();
        }

        public override void draw(SpriteBatch b)
        {
            // Background
            if (!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            this.Draw(b);

            // Draw Overlays
            foreach (var menu in Overlays
                .Where(m => m.Visible)
                .OfType<IClickableMenu>())
            {
                menu.draw(b);
                return;
            }

            // Hover Text
            if (!(hoveredItem is null))
            {
                // Hover Item
                IClickableMenu.drawToolTip(
                    b,
                    hoveredItem.getDescription(),
                    hoveredItem.DisplayName,
                    hoveredItem,
                    !(heldItem is null));
            }
            else if (!(hoverText is null) && hoverAmount > 0)
            {
                // Hover Text w/Amount
                IClickableMenu.drawToolTip(
                    b,
                    hoverText,
                    "",
                    null,
                    true,
                    moneyAmountToShowAtBottom: hoverAmount);
            }
            else if (!(hoverText is null))
            {
                // Hover Text
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }

            // Held Item
            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);

            // Game Cursor
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.GameWindowSizeChanged(oldBounds, newBounds);

            // Reposition Overlays
            foreach (var menu in Overlays.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                menu.gameWindowSizeChanged(oldBounds, newBounds);
            }

            // Reposition Chest Color Picker
            chestColorPicker.xPositionOnScreen = (int)(ItemsToGrabMenu.Position.X + ChestColorPickerOffset.X);
            chestColorPicker.yPositionOnScreen = (int)(ItemsToGrabMenu.Position.Y + ChestColorPickerOffset.Y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            var customChest = CommonHelper.OfType<object, CustomChest>(context);

            // Left Click Overlays
            foreach (var menu in Overlays
                .Where(m => m.Visible)
                .OfType<IClickableMenu>())
            {
                menu.receiveLeftClick(x, y, playSound);
                return;
            }

            // Left Click Chest Color Picker
            chestColorPicker.receiveLeftClick(x, y, playSound);
            customChest.playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);

            this.ReceiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            var customChest = CommonHelper.OfType<object, CustomChest>(context);

            // Right Click Overlays
            foreach (var menu in Overlays
                .Where(m => m.Visible)
                .OfType<IClickableMenu>())
            {
                menu.receiveRightClick(x, y, playSound && playRightClickSound);
                return;
            }

            // Right Click Chest Color Picker
            chestColorPicker.receiveRightClick(x, y, playSound && playRightClickSound);
            customChest.playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);

            this.ReceiveRightClick(x, y, playSound && playRightClickSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            var customChest = CommonHelper.OfType<object, CustomChest>(context);
            var x = Game1.getOldMouseX();
            var y = Game1.getOldMouseY();

            // Scroll Overlays
            foreach (var menu in Overlays
                .Where(m => m.Visible)
                .OfType<IClickableMenu>())
            {
                menu.receiveScrollWheelAction(direction);
                return;
            }

            // Scroll Chest Color Picker
            if (chestColorPicker.isWithinBounds(x, y))
            {
                if (direction < 0 && chestColorPicker.colorSelection < chestColorPicker.totalColors - 1)
                    chestColorPicker.colorSelection++;
                else if (direction > 0 && chestColorPicker.colorSelection > 0)
                    chestColorPicker.colorSelection--;
                ((Chest)chestColorPicker.itemToDrawColored).playerChoiceColor.Value =
                    chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
                customChest.playerChoiceColor.Value =
                    chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
            }

            this.ReceiveScrollWheelAction(direction);
        }

        public override void performHoverAction(int x, int y)
        {
            // Hover Overlays
            foreach (var menu in Overlays.Where(m => m.Visible).OfType<IClickableMenu>())
            {
                menu.performHoverAction(x, y);
                return;
            }

            // Hover Chest Color Picker
            chestColorPicker.performHoverAction(x, y);

            this.PerformHoverAction(x, y);
        }

        /*********
        ** Left Click
        *********/
        /// <summary>
        /// Toggles the Chest Color Picker on/off
        /// </summary>
        /// <param name="widget">The toggle button that was clicked</param>
        internal void ClickColorPickerToggleButton(IWidget widget)
        {
            Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
            chestColorPicker.visible = Game1.player.showChestColorPicker;
            Game1.playSound("drumkit6");
        }

        /*********
        ** Private methods
        *********/
        private void SetupMenus()
        {
            if (!(context is CustomChest customChest))
                return;

            // Chest Inventory Menu
            ItemsToGrabMenu = (customChest.ChestData.EnableRemoteStorage)
                ? new ChestInventoryMenu(this, ChestInventoryMenuOffset, customChest)
                : new ChestInventoryMenu(this, ChestInventoryMenuOffset, customChest);
            SubMenus.Add(ItemsToGrabMenu);

            // Player Inventory Menu
            inventory = new PlayerInventoryMenu(this, ChestInventoryMenuOffset + new Vector2(0, ItemsToGrabMenu.height));
            SubMenus.Add(inventory);

            // Color Picker (Replace with Overlay)
            chestColorPicker = new DiscreteColorPicker(
                ItemsToGrabMenu.xPositionOnScreen + (int)ChestColorPickerOffset.X,
                ItemsToGrabMenu.yPositionOnScreen + (int)ChestColorPickerOffset.Y,
                0,
                new Chest(true))
            {
                visible = Game1.player.showChestColorPicker
            };
            chestColorPicker.colorSelection =
                chestColorPicker.getSelectionFromColor(customChest.playerChoiceColor.Value);
            ((Chest)chestColorPicker.itemToDrawColored).playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);

            // Item Pick Overlay
            ItemPickMenu = new ItemPickMenu(this, ChestInventoryMenuOffset + ItemPickMenu.Padding / 2);
            Overlays.Add(ItemPickMenu);
        }

        private void SetupWidgets()
        {
            if (!(context is CustomChest customChest))
                return;

            // inventory (Clickable Component)
            for (var slot = 0; slot < ItemsToGrabMenu.inventory.Count; ++slot)
            {
                var cc = ItemsToGrabMenu.inventory.ElementAt(slot);
                var col = slot % InventoryMenu.ItemsPerRow;
                var row = slot / InventoryMenu.ItemsPerRow;

                cc.myID += 53910;
                cc.fullyImmutable = true;

                // Top row adjustment
                if (row == 0)
                    cc.upNeighborID = 4343;
                else
                    cc.upNeighborID += 53910;

                // Bottom row adjustment
                if (row == ItemsToGrabMenu.rows)
                    cc.downNeighborID = col;
                else
                    cc.downNeighborID += 53910;

                // Left column adjustment
                if (col == InventoryMenu.ItemsPerRow - 1)
                {
                    cc.rightNeighborID = row switch
                    {
                        0 => 27346, // Color Toggle Button
                        1 => 27346,
                        2 => 12952, // Fill Stacks
                        3 => 12952,
                        4 => 106, // Organize
                        5 => 106,
                        _ => 106
                    };
                }
                else
                {
                    cc.leftNeighborID += 53910;
                }

                // Right column adjustment
                if (col == 0)
                {
                    cc.leftNeighborID = row switch
                    {
                        0 => 239865, // Chest Category 1
                        1 => 239866, // Chest Category 2
                        2 => 239867, // Chest Category 3
                        3 => 239868, // Chest Category 4
                        4 => 239869, // Chest Category 5
                        5 => 239870, // Chest Category 6
                        _ => 239810
                    };
                }
                else
                {
                    cc.rightNeighborID += 53910;
                }
            }

            // inventory (Clickable Component)
            for (var slot = 0; slot < inventory.inventory.Count; ++slot)
            {
                var cc = ItemsToGrabMenu.inventory.ElementAt(slot);
                var col = slot % InventoryMenu.ItemsPerRow;
                var row = slot / InventoryMenu.ItemsPerRow;

                // Top row adjustment
                if (row == 0)
                    cc.upNeighborID = ItemsToGrabMenu.inventory.Count > slot ? 53910 + slot : 4343;

                // Right column adjustment
                if (col == InventoryMenu.ItemsPerRow - 1)
                    cc.rightNeighborID = row < 2 ? 5948 : 4857;
            }

            // Chest Color Picker (Clickable Component)
            discreteColorPickerCC = new List<ClickableComponent>();
            for (var index = 0; index < chestColorPicker.totalColors; ++index)
            {
                var discreteColorPicker = new ClickableComponent(new Rectangle(chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + index * 9 * 4, chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "")
                {
                    myID = index + 4343,
                    rightNeighborID = index < chestColorPicker.totalColors - 1 ? index + 4343 + 1 : -1,
                    leftNeighborID = index > 0 ? index + 4343 - 1 : -1,
                    downNeighborID = 53910
                };
                discreteColorPickerCC.Add(discreteColorPicker);
                allClickableComponents.Add(discreteColorPicker);
            }

            // Color Picker Toggle
            colorPickerToggleButton = new ClickableTexture(
                "colorPickerToggleButton",
                ItemsToGrabMenu,
                RightWidgetsOffset + ItemsToGrabMenu.Dimensions * new Vector2(1, 1f / 4f),
                Game1.mouseCursors,
                new Rectangle(119, 469, 16, 16),
                Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"))
            {
                myID = 27346,
                downNeighborID = 12952,
                leftNeighborID = 53933,
                region = 15923,
                LeftClickAction = ClickColorPickerToggleButton
            };
            allClickableComponents.Add(colorPickerToggleButton);

            // Add Invisible Drop Item Button?
            dropItemInvisibleButton = new ClickableComponent(
                new Rectangle(xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 164, Game1.tileSize, Game1.tileSize), "")
            {
                myID = 107
            };
            allClickableComponents.Add(dropItemInvisibleButton);
        }

        private static TemporaryAnimatedSprite CreatePoof(int x, int y) => new TemporaryAnimatedSprite(
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