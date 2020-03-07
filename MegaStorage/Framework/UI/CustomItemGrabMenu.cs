using MegaStorage.Framework.Models;
using MegaStorage.Framework.Persistence;
using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework.UI
{
    internal enum InventoryType
    {
        Player = 0,
        Chest = 1
    }
    public class CustomItemGrabMenu : ItemGrabMenu
    {
        /*********
        ** Fields
        *********/
        public const int MenuWidth = 840;
        public const int MenuHeight = 736;

        public static readonly Dictionary<string, Rectangle> DefaultCategories = new Dictionary<string, Rectangle>()
        {
            {"All", Rectangle.Empty},
            {"Crops", new Rectangle(640, 80, 16, 16)},
            {"Seeds", new Rectangle(656, 64, 16, 16)},
            {"Materials", new Rectangle(672, 64, 16, 16)},
            {"Cooking", new Rectangle(688, 64, 16, 16)},
            {"Fishing", new Rectangle(640, 64, 16, 16)},
            {"Misc", new Rectangle(672, 80, 16, 16)}
        };

        internal CustomChest ActiveChest { get; private set; }
        internal readonly CustomChest ActualChest;
        internal CustomClickableTextureComponent StarButton;
        internal new CustomInventoryMenu ItemsToGrabMenu { get; private set; }

#pragma warning disable IDE1006 // Naming Styles
        // ReSharper disable once InconsistentNaming
        internal new CustomInventoryMenu inventory { get; private set; }
#pragma warning restore IDE1006 // Naming Styles
        internal bool ShowRealInventory { get; private set; }

        // Offsets to ItemsToGrabMenu and inventory
        private static readonly Vector2 Offset = new Vector2(-44, -68);

        // Offsets to Color Picker
        private static readonly Vector2 TopOffset = new Vector2(32, -72);

        // Offsets to DefaultCategories
        private static readonly Vector2 LeftOffset = new Vector2(-48, 24);

        // Offsets to Color Toggle, Organize, Stack, OK, and Trash
        private static readonly Vector2 RightOffset = new Vector2(24, -32);

        private TemporaryAnimatedSprite Poof
        {
            set => _poofReflected.SetValue(value);
        }

        private readonly IReflectedField<TemporaryAnimatedSprite> _poofReflected;
        private behaviorOnItemSelect BehaviorFunction => _behaviorFunction.GetValue();
        private readonly IReflectedField<behaviorOnItemSelect> _behaviorFunction;

        /*********
        ** Public methods
        *********/
        public CustomItemGrabMenu(CustomChest actualChest, bool showRealInventory = false)
            : base(CommonHelper.NonNull(actualChest).items, actualChest)
        {
            initialize(
                (Game1.viewport.Width - MenuWidth) / 2,
                (Game1.viewport.Height - MenuHeight) / 2,
                MenuWidth,
                MenuHeight);
            if (yPositionOnScreen < IClickableMenu.spaceToClearTopBorder)
                yPositionOnScreen = IClickableMenu.spaceToClearTopBorder;
            if (xPositionOnScreen < 0)
                xPositionOnScreen = 0;

            var leftShiftState = MegaStorageMod.ModHelper.Input.GetState(SButton.LeftShift);
            var rightShiftState = MegaStorageMod.ModHelper.Input.GetState(SButton.RightShift);
            var shiftHeld = leftShiftState == SButtonState.Pressed
                            || leftShiftState == SButtonState.Held
                            || rightShiftState == SButtonState.Pressed
                            || rightShiftState == SButtonState.Held;
            ShowRealInventory = showRealInventory || shiftHeld;

            ActualChest = actualChest;
            ActiveChest = !ActualChest.EnableRemoteStorage || ShowRealInventory
                ? ActualChest
                : StateManager.MainChest
                ?? ActualChest;
            allClickableComponents = new List<ClickableComponent>();
            playRightClickSound = true;
            allowRightClick = true;
            canExitOnKey = true;

            _poofReflected = MegaStorageMod.Instance.Helper.Reflection.GetField<TemporaryAnimatedSprite>(this, "poof");
            _behaviorFunction = MegaStorageMod.Instance.Helper.Reflection.GetField<behaviorOnItemSelect>(this, "behaviorFunction");

#pragma warning disable AvoidNetField // Avoid Netcode types when possible
            Game1.player.items.OnElementChanged += Inventory_Changed;
#pragma warning restore AvoidNetField // Avoid Netcode types when possible
            if (!(StateManager.MainChest is null))
            {
                _behaviorFunction.SetValue(ActiveChest.grabItemFromInventory);
                behaviorOnItemGrab = ActiveChest.grabItemFromChest;
                ActiveChest.items.OnElementChanged += Items_Changed;
            }

            SetupItemsMenu();
            SetupInventoryMenu();
        }

        public override void draw(SpriteBatch b)
        {
            if (b is null)
                return;

            // Background
            if (!Game1.options.showMenuBackground)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            }

            ItemsToGrabMenu.draw(b);
            inventory.draw(b);
            chestColorPicker.draw(b);

            // inventory Icon
            CommonHelper.DrawInventoryIcon(b, inventory.xPositionOnScreen - 48, inventory.yPositionOnScreen + 96);

            // Custom Draw
            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => !(c.DrawAction is null)))
            {
                clickableComponent.DrawAction(b, clickableComponent);
            }

            // Default Draw
            foreach (var clickableComponent in allClickableComponents
                .OfType<ClickableTextureComponent>()
                .Where(c => !(c is CustomClickableTextureComponent customClickableTextureComponent)
                            || customClickableTextureComponent.DrawAction is null))
            {
                clickableComponent.draw(b);
            }

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

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            heldItem = inventory.leftClick(x, y, heldItem, playSound);

            chestColorPicker.receiveLeftClick(x, y);
            ActualChest.playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);

            if (ActualChest.EnableRemoteStorage && StateManager.MainChest is null)
            {
                // Cannot use chest
            }
            else if (heldItem is null)
            {
                heldItem = ItemsToGrabMenu.leftClick(x, y, heldItem, false);
                if (!(heldItem is null))
                {
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (Game1.options.SnappyMenus)
                        snapCursorToCurrentSnappedComponent();
                }

                if (heldItem is SObject obj)
                {
                    switch (obj.ParentSheetIndex)
                    {
                        case 326:
                            heldItem = null;
                            Game1.player.canUnderstandDwarves = true;
                            Poof = CreatePoof(x, y);
                            Game1.playSound("fireball");
                            break;
                        case 102:
                            heldItem = null;
                            Game1.player.foundArtifact(102, 1);
                            Poof = CreatePoof(x, y);
                            Game1.playSound("fireball");
                            break;
                        default:
                            if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, 434))
                            {
                                heldItem = null;
                                exitThisMenu(false);
                                Game1.player.eatObject(obj, true);
                            }
                            else if (obj.IsRecipe)
                            {
                                var key = heldItem.Name.Substring(0,
                                    heldItem.Name.IndexOf("Recipe",
                                        StringComparison.InvariantCultureIgnoreCase) -
                                    1);
                                try
                                {
                                    if (obj.Category == -7)
                                    {
                                        Game1.player.cookingRecipes.Add(key, 0);
                                    }
                                    else
                                    {
                                        Game1.player.craftingRecipes.Add(key, 0);
                                    }

                                    Poof = CreatePoof(x, y);
                                    Game1.playSound("newRecipe");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }

                                heldItem = null;
                            }

                            break;
                    }
                }

                if (!(heldItem is null) && Game1.player.addItemToInventoryBool(heldItem))
                {
                    heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else if (isWithinBounds(x, y))
            {
                BehaviorFunction(heldItem, Game1.player);
            }

            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => c.containsPoint(x, y) && !(c.LeftClickAction is null)))
            {
                clickableComponent.LeftClickAction(clickableComponent);
            }

            ItemsToGrabMenu.receiveLeftClick(x, y, playSound);
            inventory.receiveLeftClick(x, y, playSound);
            RefreshItems();
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!allowRightClick)
            {
                heldItem = inventory.rightClick(x, y, heldItem, playSound && playRightClickSound, true);
                return;
            }

            heldItem = inventory.rightClick(x, y, heldItem, playSound && playRightClickSound);
            if (ActualChest.EnableRemoteStorage && StateManager.MainChest is null)
            {
                // Cannot use chest
            }
            else if (heldItem is null)
            {
                heldItem = ItemsToGrabMenu.rightClick(x, y, heldItem, false);
                if (!(heldItem is null))
                {
                    behaviorOnItemGrab(heldItem, Game1.player);
                    if (Game1.options.SnappyMenus)
                        snapCursorToCurrentSnappedComponent();
                }

                if (heldItem is SObject obj)
                {
                    switch (obj.ParentSheetIndex)
                    {
                        case 326:
                            heldItem = null;
                            Game1.player.canUnderstandDwarves = true;
                            Poof = CreatePoof(x, y);
                            Game1.playSound("fireball");
                            break;
                        case 102:
                            heldItem = null;
                            Game1.player.foundArtifact(102, 1);
                            Poof = CreatePoof(x, y);
                            Game1.playSound("fireball");
                            break;
                        default:
                            if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, 434))
                            {
                                heldItem = null;
                                exitThisMenu(false);
                                Game1.player.eatObject(obj, true);
                            }
                            else if (obj.IsRecipe)
                            {
                                var key = heldItem.Name.Substring(0,
                                    heldItem.Name.IndexOf("Recipe",
                                        StringComparison.InvariantCultureIgnoreCase) -
                                    1);
                                try
                                {
                                    if (obj.Category == -7)
                                    {
                                        Game1.player.cookingRecipes.Add(key, 0);
                                    }
                                    else
                                    {
                                        Game1.player.craftingRecipes.Add(key, 0);
                                    }

                                    Poof = CreatePoof(x, y);
                                    Game1.playSound("newRecipe");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }

                                heldItem = null;
                            }

                            break;
                    }
                }

                if (!(heldItem is null) && Game1.player.addItemToInventoryBool(heldItem))
                {
                    heldItem = null;
                    Game1.playSound("coin");
                }
            }
            else if (isWithinBounds(x, y))
            {
                BehaviorFunction(heldItem, Game1.player);
            }

            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => c.containsPoint(x, y) && !(c.RightClickAction is null)))
            {
                clickableComponent.RightClickAction(clickableComponent);
            }

            ItemsToGrabMenu.receiveRightClick(x, y, playSound && playRightClickSound);
            inventory.receiveRightClick(x, y, playSound && playRightClickSound);
            RefreshItems();
        }

        public override void receiveScrollWheelAction(int direction)
        {
            var mouseX = Game1.getOldMouseX();
            var mouseY = Game1.getOldMouseY();
            if (chestColorPicker.isWithinBounds(mouseX, mouseY))
            {
                if (direction < 0 && chestColorPicker.colorSelection < chestColorPicker.totalColors - 1)
                {
                    chestColorPicker.colorSelection++;
                }
                else if (direction > 0 && chestColorPicker.colorSelection > 0)
                {
                    chestColorPicker.colorSelection--;
                }

                ((Chest)chestColorPicker.itemToDrawColored).playerChoiceColor.Value =
                    chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
                ActualChest.playerChoiceColor.Value =
                    chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
            }
            else if (ItemsToGrabMenu.isWithinBounds(mouseX, mouseY))
            {
                ItemsToGrabMenu.receiveScrollWheelAction(direction);
            }
            else
            {
                foreach (var clickableComponent in allClickableComponents
                    .OfType<CustomClickableTextureComponent>()
                    .Where(c => c.containsPoint(mouseX, mouseY) && !(c.ScrollAction is null)))
                {
                    clickableComponent.ScrollAction(direction, clickableComponent);
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                applyMovementKey(key);

            if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                if (areAllItemsTaken() && readyToClose())
                {
                    exitThisMenu();
                    if (!(Game1.currentLocation.currentEvent is null)
                        && Game1.currentLocation.currentEvent.CurrentCommand > 0)
                    {
                        ++Game1.currentLocation.currentEvent.CurrentCommand;
                    }
                }
                else if (!(heldItem is null))
                {
                    Game1.setMousePosition(trashCan.bounds.Center);
                }
            }
            else if (key == Keys.Delete && !(heldItem is null) && heldItem.canBeTrashed())
            {
                Utility.trashItem(heldItem);
                heldItem = null;
            }
            else
            {
                var stashKeyState = MegaStorageMod.ModHelper.Input.GetState(ModConfig.Instance.StashKey);
                var stashButtonState = MegaStorageMod.ModHelper.Input.GetState(ModConfig.Instance.StashButton);
                var stashPressed =
                    stashKeyState == SButtonState.Pressed
                    || stashButtonState == SButtonState.Pressed;
                if (stashPressed)
                {
                    StashItems();
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            hoveredItem = inventory.hover(x, y, heldItem) ?? ItemsToGrabMenu.hover(x, y, heldItem);
            hoverText = inventory.hoverText ?? ItemsToGrabMenu.hoverText;
            hoverAmount = 0;
            chestColorPicker.performHoverAction(x, y);

            // Hover Text
            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => !(c.hoverText is null) && c.containsPoint(x, y)))
            {
                hoverText = clickableComponent.hoverText;
            }

            // Hover Action
            foreach (var clickableComponent in allClickableComponents
                .OfType<CustomClickableTextureComponent>()
                .Where(c => !(c.HoverAction is null)))
            {
                clickableComponent.HoverAction(x, y, clickableComponent);
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            initialize(
                (Game1.viewport.Width - MenuWidth) / 2,
                (Game1.viewport.Height - MenuHeight) / 2,
                MenuWidth,
                MenuHeight);
            if (yPositionOnScreen < IClickableMenu.spaceToClearTopBorder)
                yPositionOnScreen = IClickableMenu.spaceToClearTopBorder;
            if (xPositionOnScreen < 0)
                xPositionOnScreen = 0;

            ItemsToGrabMenu.GameWindowSizeChanged();
            inventory.GameWindowSizeChanged();

            chestColorPicker.xPositionOnScreen = ItemsToGrabMenu.xPositionOnScreen + (int)TopOffset.X;
            chestColorPicker.yPositionOnScreen = ItemsToGrabMenu.yPositionOnScreen + (int)TopOffset.Y;

            foreach (var clickableComponent in allClickableComponents.OfType<CustomClickableTextureComponent>())
            {
                clickableComponent.GameWindowSizeChanged();
            }
        }

        internal CustomChestEventArgs CustomChestEventArgs => new CustomChestEventArgs()
        {
            VisibleItems = ItemsToGrabMenu?.VisibleItems,
            AllItems = ItemsToGrabMenu?.actualInventory,
            CurrentCategory = ItemsToGrabMenu?.SelectedCategory?.name ?? "All",
            HeldItem = heldItem
        };

        internal void RefreshItems()
        {
            if (ActualChest.EnableRemoteStorage && !ShowRealInventory && !(ActiveChest is null) && !ActiveChest.Equals(StateManager.MainChest))
            {
                // ReSync to Main Chest
                ActiveChest.items.OnElementChanged -= Items_Changed;
                ActiveChest = StateManager.MainChest;
                ActiveChest.items.OnElementChanged += Items_Changed;

                // Update behavior functions
                behaviorOnItemGrab = ActiveChest.grabItemFromChest;
                _behaviorFunction.SetValue(ActiveChest.grabItemFromInventory);

                // Reassign top inventory
                ItemsToGrabMenu.actualInventory = ActiveChest.items;
            }
            ItemsToGrabMenu.RefreshItems();
            inventory.RefreshItems();
        }

        internal void StashItems()
        {
            var items = (!(ItemsToGrabMenu.SelectedCategory is null) &&
                         !ItemsToGrabMenu.SelectedCategory.name.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                ? ItemsToGrabMenu.SelectedCategory.Filter(Game1.player.Items)
                : Game1.player.Items.Where(i =>
                {
                    var includes = ModConfig.Instance.StashItems.IncludesAsList;
                    var excludes = ModConfig.Instance.StashItems.ExcludesAsList;
                    return !(i is null)
                        && (includes is null || includes.Contains(i.Category) || includes.Contains(i.ParentSheetIndex))
                        && (excludes is null || !(excludes.Contains(i.Category) || excludes.Contains(i.ParentSheetIndex)));
                });

            foreach (var item in items)
            {
                BehaviorFunction(item, Game1.player);
            }

            RefreshItems();
        }

        /*********
        ** Draw
        *********/
        /// <summary>
        /// Draws the Category tab and indents active category
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="clickableComponent">The category being drawn</param>
        internal void DrawCategory(SpriteBatch b, CustomClickableTextureComponent clickableComponent)
        {
            if (clickableComponent is ChestCategory chestCategory)
                chestCategory.Draw(b, chestCategory.Equals(ItemsToGrabMenu.SelectedCategory));
        }

        /// <summary>
        /// Draws the Star Button and gray out if inactive
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="clickableComponent">The category being drawn</param>
        internal void DrawStarButton(SpriteBatch b, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.sourceRect = ActualChest.Equals(ActiveChest)
                ? CommonHelper.StarButtonActive
                : CommonHelper.StarButtonInactive;
            clickableComponent.draw(b, ActualChest.Equals(ActiveChest) ? Color.White : Color.Gray * 0.8f, (float)(0.860000014305115 + clickableComponent.bounds.Y / 20000.0));
        }

        /// <summary>
        /// Draws the Trash Can and the lid
        /// </summary>
        /// <param name="b">The SpriteBatch to draw to</param>
        /// <param name="clickableComponent">The trash can being drawn</param>
        internal void DrawTrashCan(SpriteBatch b, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.draw(b);
            b.Draw(
                Game1.mouseCursors,
                new Vector2(clickableComponent.bounds.X + 60, clickableComponent.bounds.Y + 40),
                new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10),
                Color.White,
                trashCanLidRotation,
                new Vector2(16f, 10f),
                Game1.pixelZoom,
                SpriteEffects.None,
                0.86f);
        }

        /*********
        ** Left Click
        *********/
        /// <summary>
        /// Toggles the Chest Color Picker on/off
        /// </summary>
        /// <param name="clickableComponent">The toggle button that was clicked</param>
        internal void ClickColorPickerToggleButton(CustomClickableTextureComponent clickableComponent = null)
        {
            Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
            chestColorPicker.visible = Game1.player.showChestColorPicker;
            Game1.playSound("drumkit6");
            MegaStorageApi.InvokeColorPickerToggleButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Fills chest inventory from player inventory for items that stack
        /// </summary>
        /// <param name="clickableComponent">The fill button that was clicked</param>
        internal void ClickFillStacksButton(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeFillStacksButtonClicked(this, CustomChestEventArgs);
            FillOutStacks();
            Game1.player.Items = inventory.actualInventory;
            Game1.playSound("Ship");
            MegaStorageApi.InvokeAfterFillStacksButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Sorts chest inventory
        /// </summary>
        /// <param name="clickableComponent">The organize button that was clicked</param>
        internal void ClickOrganizeButton(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeOrganizeButtonClicked(this, CustomChestEventArgs);
            organizeItemsInList(ItemsToGrabMenu.actualInventory);
            Game1.playSound("Ship");
            MegaStorageApi.InvokeAfterOrganizeButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Makes this the main chest for remote storage
        /// </summary>
        /// <param name="clickableComponent">The star button that was clicked</param>
        internal void ClickStarButton(CustomClickableTextureComponent clickableComponent = null)
        {
            if (!Context.IsMainPlayer || ActualChest.items.Count > 0 || ShowRealInventory)
                return;

            MegaStorageApi.InvokeBeforeStarButtonClicked(this, CustomChestEventArgs);
            if (!ActualChest.Equals(ActiveChest))
            {
                clickableComponent.sourceRect = CommonHelper.StarButtonActive;

                if (ActiveChest.Equals(StateManager.MainChest))
                {
                    // Move items from main chest to this chest
                    ActiveChest.items.OnElementChanged -= Items_Changed;
                    ActualChest.items.CopyFrom(ActiveChest.items);
                    ActiveChest.items.Clear();
                }

                // Assign Main Chest to Current Chest
                StateManager.MainChest = ActualChest;
                ActiveChest = ActualChest;
                ActiveChest.items.OnElementChanged += Items_Changed;

                // Update behavior functions
                behaviorOnItemGrab = ActiveChest.grabItemFromChest;
                _behaviorFunction.SetValue(ActiveChest.grabItemFromInventory);

                // Reassign top inventory
                ItemsToGrabMenu.actualInventory = ActiveChest.items;
                ItemsToGrabMenu.RefreshItems();
            }
            MegaStorageApi.InvokeAfterStarButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Exits the chest menu
        /// </summary>
        /// <param name="clickableComponent">The ok button that was clicked</param>
        internal void ClickOkButton(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeOkButtonClicked(this, CustomChestEventArgs);
            exitThisMenu();
            if (!(Game1.currentLocation.currentEvent is null))
                ++Game1.currentLocation.currentEvent.CurrentCommand;
            Game1.playSound("bigDeSelect");
            MegaStorageApi.InvokeAfterOkButtonClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Trashes the currently held item
        /// </summary>
        /// <param name="clickableComponent">The trash can that was clicked</param>
        internal void ClickTrashCan(CustomClickableTextureComponent clickableComponent = null)
        {
            MegaStorageApi.InvokeBeforeTrashCanClicked(this, CustomChestEventArgs);
            if (heldItem is null)
                return;
            Utility.trashItem(heldItem);
            heldItem = null;
            MegaStorageApi.InvokeAfterTrashCanClicked(this, CustomChestEventArgs);
        }

        /// <summary>
        /// Switches the chest menu's currently selected category
        /// </summary>
        /// <param name="categoryName">The name of the category to switch to</param>
        internal void ClickCategoryButton(string categoryName)
        {
            var clickableComponent = allClickableComponents
                .OfType<ChestCategory>()
                .First(c => c.name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));
            if (!(clickableComponent is null))
                ClickCategoryButton(clickableComponent);
        }

        /// <summary>
        /// Switches the chest menu's currently selected category
        /// </summary>
        /// <param name="clickableComponent">The category button that was clicked</param>
        internal void ClickCategoryButton(CustomClickableTextureComponent clickableComponent)
        {
            if (clickableComponent is ChestCategory chestCategory)
                ItemsToGrabMenu.SelectedCategory = chestCategory;
        }

        /*********
        ** Right Click
        *********/
        /// <summary>
        /// Right click on a category to customize
        /// </summary>
        /// <param name="clickableComponent">The category button that was clicked</param>
        internal void RightClickCategoryButton(CustomClickableTextureComponent clickableComponent)
        {
            if (clickableComponent is ChestCategory chestCategory)
                ItemsToGrabMenu.SelectedCategory = chestCategory;
        }

        /*********
        ** Scroll
        *********/
        /// <summary>
        /// Scrolls the chest menu's currently selected category
        /// </summary>
        /// <param name="direction">The direction to scroll in</param>
        /// <param name="clickableComponent">The category that is being hovered over</param>
        internal void ScrollCategory(int direction, CustomClickableTextureComponent clickableComponent = null)
        {
            ChestCategory savedCategory = null;
            ChestCategory beforeCategory = null;
            ChestCategory nextCategory = null;
            foreach (var currentCategory in allClickableComponents.OfType<ChestCategory>())
            {
                if (savedCategory == ItemsToGrabMenu.SelectedCategory)
                {
                    nextCategory = currentCategory;
                    break;
                }
                beforeCategory = savedCategory;
                savedCategory = currentCategory;
            }
            if (direction < 0 && !(nextCategory is null))
            {
                ItemsToGrabMenu.SelectedCategory = nextCategory;
            }
            else if (direction > 0 && !(beforeCategory is null))
            {
                ItemsToGrabMenu.SelectedCategory = beforeCategory;
            }
        }

        /*********
        ** Hover
        *********/
        /// <summary>
        /// Zooms in on the hovered component
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="clickableComponent">The item being hovered over</param>
        internal void HoverZoom(int x, int y, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.scale = clickableComponent.containsPoint(x, y)
                ? Math.Min(1.1f, clickableComponent.scale + 0.05f)
                : Math.Max(1f, clickableComponent.scale - 0.05f);
        }

        /// <summary>
        /// Zooms in on the hovered component (scaled up by Game1.pixelZoom)
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="clickableComponent">The item being hovered over</param>
        internal void HoverPixelZoom(int x, int y, CustomClickableTextureComponent clickableComponent)
        {
            clickableComponent.scale = clickableComponent.containsPoint(x, y)
                ? Math.Min(Game1.pixelZoom * 1.1f, clickableComponent.scale + 0.05f)
                : Math.Max(Game1.pixelZoom * 1f, clickableComponent.scale - 0.05f);
        }

        /// <summary>
        /// Rotates the trash can lid while hovering over the trash can
        /// </summary>
        /// <param name="x">The X-coordinate of the mouse</param>
        /// <param name="y">The Y-coordinate of the mouse</param>
        /// <param name="clickableComponent">The trash can being hovered over</param>
        internal void HoverTrashCan(int x, int y, CustomClickableTextureComponent clickableComponent)
        {
            if (!clickableComponent.containsPoint(x, y))
            {
                trashCanLidRotation = Math.Max(trashCanLidRotation - (float)Math.PI / 48f, 0.0f);
                return;
            }

            if (trashCanLidRotation <= 0f)
                Game1.playSound("trashcanlid");
            trashCanLidRotation = Math.Min(trashCanLidRotation + (float)Math.PI / 48f, 1.570796f);

            if (heldItem is null || Utility.getTrashReclamationPrice(heldItem, Game1.player) <= 0)
                return;
            hoverText = Game1.content.LoadString("Strings\\UI:TrashCanSale");
            hoverAmount = Utility.getTrashReclamationPrice(heldItem, Game1.player);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Configures all UI elements related to the top menu
        /// </summary>
        private void SetupItemsMenu()
        {
            ItemsToGrabMenu = new CustomInventoryMenu(
                this,
                Offset,
                InventoryType.Chest);
            //base.ItemsToGrabMenu = ItemsToGrabMenu;

            // inventory (Clickable Component)
            for (var slot = 0; slot < ItemsToGrabMenu.inventory.Count; ++slot)
            {
                var cc = ItemsToGrabMenu.inventory.ElementAt(slot);
                var col = slot % CustomInventoryMenu.ItemsPerRow;
                var row = slot / CustomInventoryMenu.ItemsPerRow;

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
                if (col == CustomInventoryMenu.ItemsPerRow - 1)
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

            // Color Picker
            chestColorPicker = new DiscreteColorPicker(
                ItemsToGrabMenu.xPositionOnScreen + (int)TopOffset.X,
                ItemsToGrabMenu.yPositionOnScreen + (int)TopOffset.Y,
                0,
                new Chest(true))
            {
                visible = false
            };
            chestColorPicker.colorSelection =
                chestColorPicker.getSelectionFromColor(ActualChest.playerChoiceColor.Value);
            ((Chest)chestColorPicker.itemToDrawColored).playerChoiceColor.Value =
                chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);

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
            colorPickerToggleButton = new CustomClickableTextureComponent(
                "colorPickerToggleButton",
                ItemsToGrabMenu,
                RightOffset + ItemsToGrabMenu.Dimensions * new Vector2(1, 1f / 4f),
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

            // Stack
            fillStacksButton = new CustomClickableTextureComponent(
                "fillStacksButton",
                ItemsToGrabMenu,
                RightOffset + ItemsToGrabMenu.Dimensions * new Vector2(1, 2f / 4f),
                Game1.mouseCursors,
                new Rectangle(103, 469, 16, 16),
                Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"))
            {
                myID = 12952,
                upNeighborID = 27346,
                downNeighborID = 106,
                leftNeighborID = 53957,
                region = 15923,
                LeftClickAction = ClickFillStacksButton,
                HoverAction = HoverPixelZoom
            };
            allClickableComponents.Add(fillStacksButton);

            // Organize
            organizeButton = new CustomClickableTextureComponent(
                "organizeButton",
                ItemsToGrabMenu,
                RightOffset + ItemsToGrabMenu.Dimensions * new Vector2(1, 3f / 4f),
                Game1.mouseCursors,
                new Rectangle(162, 440, 16, 16),
                Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"))
            {
                myID = 106,
                upNeighborID = 12952,
                downNeighborID = 5948,
                leftNeighborID = 53969,
                region = 15923,
                LeftClickAction = ClickOrganizeButton,
                HoverAction = HoverPixelZoom
            };
            allClickableComponents.Add(organizeButton);

            // Star
            if (ActualChest.EnableRemoteStorage && !ShowRealInventory)
            {
                StarButton = new CustomClickableTextureComponent(
                    "starButton",
                    ItemsToGrabMenu,
                    new Vector2(-Game1.tileSize, -Game1.tileSize),
                    Game1.mouseCursors,
                    StateManager.MainChest == ActualChest
                        ? CommonHelper.StarButtonActive
                        : CommonHelper.StarButtonInactive)
                {
                    myID = 239864,
                    downNeighborID = 239865,
                    rightNeighborID = 4343,
                    DrawAction = DrawStarButton,
                    LeftClickAction = ClickStarButton,
                    HoverAction = HoverPixelZoom
                };
                allClickableComponents.Add(StarButton);
            }

            // DefaultCategories
            if (!ActualChest.EnableCategories)
                return;

            for (var index = 0; index < Math.Min(7, ModConfig.Instance.Categories.Count); ++index)
            {
                var categoryConfig = ModConfig.Instance.Categories.ElementAt(index);
                DefaultCategories.TryGetValue(categoryConfig.CategoryName, out var defaultRect);

                var texture = !(string.IsNullOrWhiteSpace(categoryConfig.Image))
                    ? MegaStorageMod.Instance.Helper.Content.Load<Texture2D>(
                        Path.Combine("assets", categoryConfig.Image))
                    : Game1.mouseCursors;
                var sourceRect = !(string.IsNullOrWhiteSpace(categoryConfig.Image))
                    ? Rectangle.Empty
                    : defaultRect;

                var categoryCC = new ChestCategory(
                    categoryConfig.CategoryName,
                    ItemsToGrabMenu,
                    LeftOffset + new Vector2(0, index * 60),
                    texture,
                    sourceRect,
                    categoryConfig);

                categoryCC.BelongsToCategory = categoryConfig.CategoryName switch
                {
                    "All" => item => true,
                    "Misc" => item =>
                    {
                        if (item is null || string.IsNullOrWhiteSpace(item.getCategoryName()))
                            return true;
                        if (item is SObject obj && !(obj.Type is null) &&
                            obj.Type.Equals("Arch", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }

                        return item switch
                        {
                            Tool _ => true,
                            Boots _ => true,
                            Ring _ => true,
                            Furniture _ => true,
                            _ => categoryCC.BelongsToCategoryDefault(item)
                        };
                    }
                    ,
                    _ => item => categoryCC.BelongsToCategoryDefault(item)
                };

                categoryCC.myID = index + 239865;
                categoryCC.upNeighborID = index > 0 || ActualChest.EnableRemoteStorage ? index + 239864 : 4343;
                categoryCC.downNeighborID = index < DefaultCategories.Count - 1 ? index + 239866 : 1;
                categoryCC.rightNeighborID = index switch
                {
                    0 => 53910, // ItemsToGrabMenu.inventory Row 1 Col 1
                    1 => 53922, // ItemsToGrabMenu.inventory Row 2 Col 1
                    2 => 53934, // ItemsToGrabMenu.inventory Row 3 Col 1
                    3 => 53946, // ItemsToGrabMenu.inventory Row 4 Col 1
                    4 => 53946, // ItemsToGrabMenu.inventory Row 4 Col 1
                    5 => 53958, // ItemsToGrabMenu.inventory Row 5 Col 1
                    6 => 53970, // ItemsToGrabMenu.inventory Row 6 Col 1
                    _ => 53970
                };
                categoryCC.DrawAction = DrawCategory;
                categoryCC.LeftClickAction = ClickCategoryButton;
                categoryCC.RightClickAction = RightClickCategoryButton;
                categoryCC.ScrollAction = ScrollCategory;

                allClickableComponents.Add(categoryCC);
            }
            ItemsToGrabMenu.SelectedCategory = allClickableComponents.OfType<ChestCategory>().First();
        }
        /// <summary>
        /// Configures all the UI elements related to the bottom menu
        /// </summary>
        private void SetupInventoryMenu()
        {
            inventory = new CustomInventoryMenu(
                this,
                new Vector2(0, ItemsToGrabMenu.height) + Offset,
                InventoryType.Player);
            //((MenuWithInventory) this).inventory = inventory;

            // inventory (Clickable Component)
            for (var slot = 0; slot < inventory.inventory.Count; ++slot)
            {
                var cc = ItemsToGrabMenu.inventory.ElementAt(slot);
                var col = slot % CustomInventoryMenu.ItemsPerRow;
                var row = slot / CustomInventoryMenu.ItemsPerRow;

                // Top row adjustment
                if (row == 0)
                    cc.upNeighborID = ItemsToGrabMenu.inventory.Count > slot ? 53910 + slot : 4343;

                // Right column adjustment
                if (col == CustomInventoryMenu.ItemsPerRow - 1)
                    cc.rightNeighborID = row < 2 ? 5948 : 4857;
            }

            // OK Button
            okButton = new CustomClickableTextureComponent(
                "okButton",
                inventory,
                new Vector2(inventory.width, 204) + RightOffset,
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
                scale: 1f)
            {
                myID = 4857,
                upNeighborID = 5948,
                leftNeighborID = 11,
                LeftClickAction = ClickOkButton,
                HoverAction = HoverZoom
            };
            allClickableComponents.Add(okButton);

            // Trash Can
            trashCan = new CustomClickableTextureComponent(
                "trashCan",
                inventory,
                new Vector2(inventory.width, 68) + RightOffset,
                Game1.mouseCursors,
                new Rectangle(564 + Game1.player.trashCanLevel * 18, 102, 18, 26),
                width: Game1.tileSize,
                height: 104)
            {
                myID = 5948,
                downNeighborID = 4857,
                leftNeighborID = 23,
                upNeighborID = 106,
                DrawAction = DrawTrashCan,
                LeftClickAction = ClickTrashCan,
                HoverAction = HoverTrashCan
            };
            allClickableComponents.Add(trashCan);

            // Add Invisible Drop Item Button?
            dropItemInvisibleButton = new ClickableComponent(
                new Rectangle(xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 128, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 164, Game1.tileSize, Game1.tileSize), "")
            {
                myID = 107
            };
            allClickableComponents.Add(dropItemInvisibleButton);
        }

        private void Items_Changed(Netcode.NetList<Item, Netcode.NetRef<Item>> list, int index, Item oldValue, Item newValue)
        {
            ItemsToGrabMenu.RefreshItems();
        }

        private void Inventory_Changed(Netcode.NetList<Item, Netcode.NetRef<Item>> list, int index, Item oldValue, Item newValue)
        {
            inventory.RefreshItems();
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