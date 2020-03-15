﻿using MegaStorage.Framework.Models;
using MegaStorage.Framework.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework.UI.Menus
{
    internal class ChestInventory : BaseInventory
    {
        /*********
        ** Fields
        *********/
        public static readonly Vector2 RightWidgetsOffset = new Vector2(24, -32);

        public event EventHandler ChestTabChanged;

        public ChestTab CurrentTab
        {
            get => _currentTab;
            set
            {
                _currentTab = value;
                SyncItems();
                InvokeChestTabChanged();
            }
        }

        public CustomChest CustomChest;
        public IList<Item> VisibleItems => actualInventory
            .Where(_currentTab.BelongsToCategory)
            .ToList();
        public TemporaryAnimatedSprite Poof
        {
            set
            {
                _poofReflected ??= MegaStorageMod.Helper.Reflection.GetField<TemporaryAnimatedSprite>(BaseMenu, "poof");
                _poofReflected.SetValue(value);
            }
        }

        protected internal BaseWidget UpArrow;
        protected internal BaseWidget DownArrow;
        protected internal Checkbox StarButton;

        private ChestTab _currentTab;
        private int _currentRow;
        private int _maxRows;

        private IReflectedField<TemporaryAnimatedSprite> _poofReflected;

        /*********
        ** Public methods
        *********/
        public ChestInventory(IMenu parentMenu, Vector2 offset, CustomChest customChest)
            : base(
                parentMenu,
                offset,
                customChest.items,
                6)
        {
            CustomChest = customChest;
            CustomChest.items.OnElementChanged += SyncItem;
            BaseMenu.BehaviorFunction = customChest.grabItemFromInventory;
            BaseMenu.behaviorOnItemGrab = customChest.grabItemFromChest;
            SetupWidgets();
            SyncItems();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, false);

            if (HeldItem is null)
                return;
            BaseMenu.behaviorOnItemGrab?.Invoke(HeldItem, Game1.player);

            if (HeldItem is SObject obj)
            {
                switch (obj.ParentSheetIndex)
                {
                    case 326:
                        HeldItem = null;
                        Game1.player.canUnderstandDwarves = true;
                        Poof = Sprites.CreatePoof(x, y);
                        Game1.playSound("fireball");
                        break;
                    case 102:
                        HeldItem = null;
                        Game1.player.foundArtifact(102, 1);
                        Poof = Sprites.CreatePoof(x, y);
                        Game1.playSound("fireball");
                        break;
                    default:
                        if (Utility.IsNormalObjectAtParentSheetIndex(HeldItem, 434))
                        {
                            HeldItem = null;
                            exitThisMenu(false);
                            Game1.player.eatObject(obj, true);
                        }
                        else if (obj.IsRecipe)
                        {
                            var key = HeldItem.Name.Substring(0,
                                HeldItem.Name.IndexOf("Recipe",
                                    StringComparison.InvariantCultureIgnoreCase) - 1);
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

                                Poof = Sprites.CreatePoof(x, y);
                                Game1.playSound("newRecipe");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.Message);
                                throw;
                            }
                            HeldItem = null;
                        }
                        break;
                }
            }

            if (!(HeldItem is null) && Game1.player.addItemToInventoryBool(HeldItem))
            {
                HeldItem = null;
                Game1.playSound("coin");
            }

            SyncItems();
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, false);

            if (HeldItem is null)
                return;
            BaseMenu.behaviorOnItemGrab?.Invoke(HeldItem, Game1.player);

            if (HeldItem is SObject obj)
            {
                switch (obj.ParentSheetIndex)
                {
                    case 326:
                        HeldItem = null;
                        Game1.player.canUnderstandDwarves = true;
                        Poof = Sprites.CreatePoof(x, y);
                        Game1.playSound("fireball");
                        break;
                    case 102:
                        HeldItem = null;
                        Game1.player.foundArtifact(102, 1);
                        Poof = Sprites.CreatePoof(x, y);
                        Game1.playSound("fireball");
                        break;
                    default:
                        if (Utility.IsNormalObjectAtParentSheetIndex(HeldItem, 434))
                        {
                            HeldItem = null;
                            exitThisMenu(false);
                            Game1.player.eatObject(obj, true);
                        }
                        else if (obj.IsRecipe)
                        {
                            var key = HeldItem.Name.Substring(0,
                                HeldItem.Name.IndexOf("Recipe",
                                    StringComparison.InvariantCultureIgnoreCase) - 1);
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

                                Poof = Sprites.CreatePoof(x, y);
                                Game1.playSound("newRecipe");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.Message);
                                throw;
                            }
                            HeldItem = null;
                        }
                        break;
                }
            }

            if (!(HeldItem is null) && Game1.player.addItemToInventoryBool(HeldItem))
            {
                HeldItem = null;
                Game1.playSound("coin");
            }

            SyncItems();
        }

        public sealed override void SyncItems()
        {
            _maxRows = (int)Math.Ceiling((double)VisibleItems.Count / ItemsPerRow);
            UpArrow.visible = _currentRow > 0;
            DownArrow.visible = _currentRow < _maxRows - rows;

            var enumerator = VisibleItems.Skip(ItemsPerRow * _currentRow).GetEnumerator();
            foreach (var itemSlot in allClickableComponents.OfType<ItemSlot>())
            {
                if (enumerator.MoveNext())
                {
                    itemSlot.item = enumerator.Current;
                    itemSlot.Slot = actualInventory.IndexOf(enumerator.Current);
                }
                else
                {
                    itemSlot.item = null;
                    itemSlot.Slot = -1;
                }
            }
            enumerator.Dispose();
        }

        /*********
        ** Private methods
        *********/
        private void SetupWidgets()
        {
            // Up Arrow
            UpArrow = new BaseWidget(
                "upArrow",
                this,
                new Vector2(width - Game1.tileSize + 8, 36),
                Sprites.Icons.UpArrow)
            {
                myID = 88,
                downNeighborID = 89,
                visible = false
            };
            UpArrow.Events.LeftClick = ScrollUp;
            allClickableComponents.Add(UpArrow);

            // Down Arrow
            DownArrow = new BaseWidget(
                "downArrow",
                this,
                new Vector2(width - Game1.tileSize + 8, height - Game1.tileSize - 36),
                Sprites.Icons.DownArrow)
            {
                myID = 89,
                upNeighborID = 88,
                visible = _currentRow <= _maxRows - rows,
            };
            DownArrow.Events.LeftClick = ScrollDown;
            allClickableComponents.Add(DownArrow);

            if (!CustomChest.ChestData.EnableChestTabs)
                return;

            // Color Picker Toggle
            var colorPickerToggleButton = new BaseWidget(
                "colorPickerToggleButton",
                this,
                RightWidgetsOffset + this.GetDimensions() * new Vector2(1, 1f / 4f),
                Sprites.Icons.ColorToggle,
                Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"))
            {
                myID = 27346,
                downNeighborID = 12952,
                leftNeighborID = 53933,
                region = 15923
            };
            colorPickerToggleButton.Events.LeftClick = ClickColorPickerToggleButton;
            colorPickerToggleButton.Events.Hover = WidgetEvents.HoverPixelZoom;
            allClickableComponents.Add(colorPickerToggleButton);
            BaseMenu.colorPickerToggleButton = colorPickerToggleButton;

            // Fill Stacks
            var fillStacksButton = new BaseWidget(
                "fillStacks",
                this,
                RightWidgetsOffset + this.GetDimensions() * new Vector2(1, 2f / 4f),
                Sprites.Icons.FillStacks,
                Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"))
            {
                myID = 12952,
                upNeighborID = 27346,
                downNeighborID = 106,
                leftNeighborID = 53957,
                region = 15923
            };
            fillStacksButton.Events.LeftClick = ClickFillStacksButton;
            fillStacksButton.Events.Hover = WidgetEvents.HoverPixelZoom;
            allClickableComponents.Add(fillStacksButton);
            BaseMenu.fillStacksButton = fillStacksButton;

            // Organize
            var organizeButton = new BaseWidget(
                "organize",
                this,
                RightWidgetsOffset + this.GetDimensions() * new Vector2(1, 3f / 4f),
                Sprites.Icons.Organize,
                Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"))
            {
                myID = 106,
                upNeighborID = 12952,
                downNeighborID = 5948,
                leftNeighborID = 53969,
                region = 15923
            };
            organizeButton.Events.LeftClick = ClickOrganizeButton;
            organizeButton.Events.Hover = WidgetEvents.HoverPixelZoom;
            allClickableComponents.Add(organizeButton);
            BaseMenu.organizeButton = organizeButton;

            // Chest Tabs
            for (var index = 0; index < Math.Min(7, ModConfig.Instance.ChestTabs.Count); ++index)
            {
                var chestTabConfig = ModConfig.Instance.ChestTabs.ElementAt(index);
                var chestTab = chestTabConfig.ChestTab(this, index);

                chestTab.myID = index + 239865;
                chestTab.upNeighborID = index > 0 || CustomChest.ChestData.EnableRemoteStorage ? index + 239864 : 4343;
                chestTab.downNeighborID = index + 239866;
                chestTab.rightNeighborID = index switch
                {
                    0 => 53910, // ChestInventoryMenu.inventory Row 1 Col 1
                    1 => 53922, // ChestInventoryMenu.inventory Row 2 Col 1
                    2 => 53934, // ChestInventoryMenu.inventory Row 3 Col 1
                    3 => 53946, // ChestInventoryMenu.inventory Row 4 Col 1
                    4 => 53946, // ChestInventoryMenu.inventory Row 4 Col 1
                    5 => 53958, // ChestInventoryMenu.inventory Row 5 Col 1
                    6 => 53970, // ChestInventoryMenu.inventory Row 6 Col 1
                    _ => 53970
                };
                chestTab.Events.RightClick = RightClickChestTab;
                chestTab.Events.Scroll = ScrollChestTab;
                chestTab.BelongsToCategory = chestTab.name switch
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
                            _ => chestTabConfig.BelongsTo(item)
                        };
                    }
                    ,
                    _ => chestTabConfig.BelongsTo
                };

                allClickableComponents.Add(chestTab);
            }

            CurrentTab = allClickableComponents.OfType<ChestTab>().First();

            // Star Button
            if (!CustomChest.ChestData.EnableRemoteStorage)
                return;

            StarButton = new Checkbox(
                "starButton",
                this,
                new Vector2(-1, -1) * Game1.tileSize,
                Sprites.Icons.InactiveStarIcon,
                Sprites.Icons.ActiveStarIcon)
            {
                myID = 123
            };
            StarButton.Events.Draw = DrawStarButton;
            StarButton.Events.LeftClick = ClickStarButton;
            allClickableComponents.Add(StarButton);
        }

        private void SyncItem(NetList<Item, NetRef<Item>> list, int slot, Item oldItem, Item currentItem)
        {
            var oldBelongsToCategory = _currentTab.BelongsToCategory(oldItem);
            var newBelongsToCategory = _currentTab.BelongsToCategory(currentItem);

            if (!oldBelongsToCategory && !newBelongsToCategory)
                return;

            if (oldBelongsToCategory && !newBelongsToCategory)
            {
                ItemSlot.item = null;
                ItemSlot.Slot = -1;
                return;
            }

            var itemSlot = allClickableComponents
                               .OfType<ItemSlot>()
                               .FirstOrDefault(cc => cc.Slot == slot)
                           ?? allClickableComponents
                               .OfType<ItemSlot>()
                               .FirstOrDefault(cc => cc.Slot == -1);

            if (itemSlot is null)
                return;

            itemSlot.item = currentItem;
            itemSlot.Slot = slot;
        }

        private void DrawStarButton(SpriteBatch b, IWidget widget)
        {
            var cc = CommonHelper.OfType<ClickableTextureComponent>(widget);
            cc.sourceRect = ActualChest.Equals(CustomChest)
                ? Sprites.Icons.ActiveStarIcon.SourceRect
                : Sprites.Icons.InactiveStarIcon.SourceRect;
            cc.draw(
                b,
                ActualChest.Equals(CustomChest) ? Color.White : Color.Gray * 0.8f,
                0.860000014305115f + cc.bounds.Y / 20000f);
        }

        private void ClickStarButton(IWidget widget)
        {
            if (!Context.IsMainPlayer || ActualChest.items.Count > 0)
                return;

            var cc = CommonHelper.OfType<ClickableTextureComponent>(widget);

            if (ActualChest.Equals(CustomChest))
                return;

            cc.sourceRect = Sprites.Icons.ActiveStarIcon.SourceRect;

            if (CustomChest.Equals(MegaStorageMod.MainChest))
            {
                // Move items from main chest to this chest
                CustomChest.items.OnElementChanged -= SyncItem;
                ActualChest.items.CopyFrom(CustomChest.items);
                CustomChest.items.Clear();
            }

            // Assign Main Chest to Current Chest
            CustomChest = ActualChest;
            CustomChest.items.OnElementChanged += SyncItem;

            // Update behavior functions
            BaseMenu.behaviorOnItemGrab = CustomChest.grabItemFromChest;
            BaseMenu.BehaviorFunction = CustomChest.grabItemFromInventory;

            // Reassign top inventory
            actualInventory = CustomChest.items;
            SyncItems();
        }

        private void ClickColorPickerToggleButton(IWidget widget)
        {
            Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
            BaseMenu.chestColorPicker.visible = Game1.player.showChestColorPicker;
            Game1.playSound("drumkit6");
        }

        private void ClickFillStacksButton(IWidget widget)
        {
            BaseMenu.FillOutStacks();
            //Game1.player.Items = inventory.actualInventory;
            Game1.playSound("Ship");
        }

        private void ClickOrganizeButton(IWidget widget)
        {
            StardewValley.Menus.ItemGrabMenu.organizeItemsInList(actualInventory);
            Game1.playSound("Ship");
        }

        internal void RightClickChestTab(IWidget widget)
        {
            var chestTab = CommonHelper.OfType<ChestTab>(widget);
            BaseMenu.ItemPickMenu.SelectedChestTab = chestTab;
            BaseMenu.ItemPickMenu.Visible = true;
        }

        private void ScrollDown(IWidget widget)
        {
            if (_currentRow >= _maxRows - rows)
                return;
            ++_currentRow;
            SyncItems();
        }

        private void ScrollUp(IWidget widget)
        {
            if (_currentRow <= 0)
                return;
            --_currentRow;
            SyncItems();
        }

        private void ScrollChestTab(int direction, IWidget widget)
        {
            ChestTab savedTab = null;
            ChestTab prevTab = null;
            ChestTab nextTab = null;
            foreach (var chestTab in allClickableComponents.OfType<ChestTab>())
            {
                if (savedTab == CurrentTab)
                {
                    nextTab = chestTab;
                    break;
                }
                prevTab = savedTab;
                savedTab = chestTab;
            }

            if (direction < 0 && !(nextTab is null))
            {
                CurrentTab = nextTab;
            }
            else if (direction > 0 && !(prevTab is null))
            {
                CurrentTab = prevTab;
            }
        }

        private void InvokeChestTabChanged()
        {
            ChestTabChanged?.Invoke(null, null);
        }
    }
}
