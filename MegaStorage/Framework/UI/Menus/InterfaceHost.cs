using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MegaStorage.Framework.UI.Menus
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class InterfaceHost : ItemGrabMenu, IMenu
    {
        /*********
        ** Fields
        *********/
        public const int MenuWidth = 840;
        public const int MenuHeight = 736;
        public static readonly Vector2 ChestInventoryMenuOffset = new Vector2(-44, -68);
        public static readonly Vector2 ChestColorPickerOffset = new Vector2(32, -72);

        public IMenu ParentMenu { get; } = null;
        public MenuType Type { get; } = MenuType.Base;
        public Vector2 Offset { get; set; } = -new Vector2(MenuWidth, MenuHeight) / 2f;
        public Vector2 Position
        {
            get => new Vector2(xPositionOnScreen, yPositionOnScreen);
            set
            {
                xPositionOnScreen = Math.Max(0, (int)value.X);
                yPositionOnScreen = Math.Max(spaceToClearTopBorder, (int)value.Y);
            }
        }

        public bool Visible { get; set; } = true;
        public bool FadedBackground => !Game1.options.showMenuBackground;
        public IList<IMenu> SubMenus { get; } = new List<IMenu>();

        internal new ChestInventoryMenu ItemsToGrabMenu { get; private set; }
        internal new PlayerInventoryMenu inventory { get; private set; }
        internal ItemPickMenu ItemPickMenu { get; private set; }

        public behaviorOnItemSelect BehaviorFunction
        {
            get => _behaviorFunction.GetValue();
            set => _behaviorFunction.SetValue(value);
        }
        private readonly IReflectedField<behaviorOnItemSelect> _behaviorFunction;

        /*********
        ** Public methods
        *********/
        public InterfaceHost(CustomChest customChest)
            : base(CommonHelper.NonNull(customChest).items, customChest)
        {
            Position = Offset + (new Vector2(Game1.viewport.Width, Game1.viewport.Height) / 2);
            width = MenuWidth;
            height = MenuHeight;

            // Setup Properties
            allClickableComponents.Clear();
            playRightClickSound = true;
            allowRightClick = true;
            canExitOnKey = true;
            _behaviorFunction = MegaStorageMod.Helper.Reflection.GetField<behaviorOnItemSelect>(this, "behaviorFunction");

            SetupMenus();
            SetupWidgets();
        }

        public override void draw(SpriteBatch b)
        {
            // Chest color picker
            chestColorPicker.draw(b);

            this.Draw(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.GameWindowSizeChanged(oldBounds, newBounds);

            // Reposition Chest Color Picker
            chestColorPicker.xPositionOnScreen = (int)(ItemsToGrabMenu.Position.X + ChestColorPickerOffset.X);
            chestColorPicker.yPositionOnScreen = (int)(ItemsToGrabMenu.Position.Y + ChestColorPickerOffset.Y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.ReceiveLeftClick(x, y, playSound))
                return;

            // Left Click Chest Color Picker
            var customChest = CommonHelper.OfType<CustomChest>(context);
            chestColorPicker.receiveLeftClick(x, y, playSound);
            customChest.playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.ReceiveRightClick(x, y, playSound && playRightClickSound))
                return;

            // Right Click Chest Color Picker
            var customChest = CommonHelper.OfType<CustomChest>(context);
            chestColorPicker.receiveRightClick(x, y, playSound && playRightClickSound);
            customChest.playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (this.ReceiveScrollWheelAction(direction))
                return;

            // Scroll Chest Color Picker
            var customChest = CommonHelper.OfType<CustomChest>(context);
            var x = Game1.getOldMouseX();
            var y = Game1.getOldMouseY();

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
        }

        public override void performHoverAction(int x, int y)
        {
            if (this.PerformHoverAction(x, y))
                return;

            // Hover Chest Color Picker
            chestColorPicker.performHoverAction(x, y);
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
            base.ItemsToGrabMenu = ItemsToGrabMenu;

            // Player Inventory Menu
            inventory = new PlayerInventoryMenu(this, ChestInventoryMenuOffset + new Vector2(0, ItemsToGrabMenu.height));
            base.inventory = inventory;

            SubMenus.Add(inventory);
            SubMenus.Add(ItemsToGrabMenu);

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
            SubMenus.Add(ItemPickMenu);
        }

        private void SetupWidgets()
        {
            if (!(context is CustomChest customChest))
                return;

            // inventory (Clickable Component)
            for (var slot = 0; slot < ItemsToGrabMenu.inventory.Count; ++slot)
            {
                var cc = ItemsToGrabMenu.inventory.ElementAt(slot);
                var col = slot % BaseInventoryMenu.ItemsPerRow;
                var row = slot / BaseInventoryMenu.ItemsPerRow;

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
                if (col == BaseInventoryMenu.ItemsPerRow - 1)
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
                var col = slot % BaseInventoryMenu.ItemsPerRow;
                var row = slot / BaseInventoryMenu.ItemsPerRow;

                // Top row adjustment
                if (row == 0)
                    cc.upNeighborID = ItemsToGrabMenu.inventory.Count > slot ? 53910 + slot : 4343;

                // Right column adjustment
                if (col == BaseInventoryMenu.ItemsPerRow - 1)
                    cc.rightNeighborID = row < 2 ? 5948 : 4857;
            }

            // Chest Color Picker (Clickable Component)
            discreteColorPickerCC = new List<ClickableComponent>();
            for (var index = 0; index < chestColorPicker.totalColors; ++index)
            {
                var discreteColorPicker = new ClickableComponent(new Rectangle(chestColorPicker.xPositionOnScreen + borderWidth / 2 + index * 9 * 4, chestColorPicker.yPositionOnScreen + borderWidth / 2, 36, 28), "")
                {
                    myID = index + 4343,
                    rightNeighborID = index < chestColorPicker.totalColors - 1 ? index + 4343 + 1 : -1,
                    leftNeighborID = index > 0 ? index + 4343 - 1 : -1,
                    downNeighborID = 53910
                };
                discreteColorPickerCC.Add(discreteColorPicker);
                allClickableComponents.Add(discreteColorPicker);
            }

            // Add Invisible Drop Item Button?
            dropItemInvisibleButton = new ClickableComponent(
                new Rectangle(xPositionOnScreen - borderWidth - spaceToClearSideBorder - 128, yPositionOnScreen + spaceToClearTopBorder + borderWidth + 164, Game1.tileSize, Game1.tileSize), "")
            {
                myID = 107
            };
            allClickableComponents.Add(dropItemInvisibleButton);
        }
    }
}