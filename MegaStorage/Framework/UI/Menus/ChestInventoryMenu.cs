using MegaStorage.Framework.Models;
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
    internal class ChestInventoryMenu : InventoryMenu
    {
        /*********
        ** Fields
        *********/
        public static readonly Vector2 RightWidgetsOffset = new Vector2(24, -32);
        public static readonly Rectangle FillStacksSourceRect = new Rectangle(103, 469, 16, 16);
        public static readonly Rectangle OrganizeSourceRect = new Rectangle(162, 440, 16, 16);

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
        public IList<Item> VisibleItems;

        private ChestTab _currentTab;
        private int _currentRow;
        private int _maxRows;

        private ClickableTexture _upArrow;
        private ClickableTexture _downArrow;

        /*********
        ** Public methods
        *********/
        public ChestInventoryMenu(IMenu parentMenu, Vector2 offset, CustomChest customChest)
            : base(
                parentMenu,
                offset,
                customChest.items,
                6)
        {
            CustomChest = customChest;
            SetupWidgets();
            SyncItems();
        }
        public sealed override void SyncItems()
        {
            VisibleItems = (_currentTab?.Filter(actualInventory) ?? actualInventory)
                .Skip(ItemsPerRow * _currentRow)
                .ToList();

            _maxRows = (int)Math.Ceiling((double)VisibleItems.Count / ItemsPerRow);
            _upArrow.visible = _currentRow > 0;
            _downArrow.visible = _currentRow < _maxRows - rows;

            for (var slot = 0; slot < capacity; ++slot)
            {
                var itemSlot = allClickableComponents
                    .OfType<ItemSlot>()
                    .Single(cc => cc.Slot == slot);

                itemSlot.item = (slot < VisibleItems.Count)
                    ? VisibleItems.ElementAt(slot)
                    : null;

                itemSlot.visible = !(itemSlot.item is null);
            }
        }

        /*********
        ** Private methods
        *********/
        private void SetupWidgets()
        {
            // Up Arrow
            _upArrow = new ClickableTexture(
                "upArrow",
                this,
                new Vector2(width - Game1.tileSize + 8, 36),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12),
                scale: 1f)
            {
                myID = 88,
                downNeighborID = 89,
                visible = false,
                LeftClickAction = ScrollUp
            };
            allClickableComponents.Add(_upArrow);

            // Down Arrow
            _downArrow = new ClickableTexture(
                "downArrow",
                this,
                new Vector2(width - Game1.tileSize + 8, height - Game1.tileSize - 36),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11),
                scale: 1f)
            {
                myID = 89,
                upNeighborID = 88,
                visible = _currentRow <= _maxRows - rows,
                LeftClickAction = ScrollDown
            };
            allClickableComponents.Add(_downArrow);

            if (!CustomChest.ChestData.EnableChestTabs)
                return;

            // Fill Stacks
            ItemGrabMenu.fillStacksButton = new ClickableTexture(
                "fillStacks",
                this,
                RightWidgetsOffset + Dimensions * new Vector2(1, 2f / 4f),
                Game1.mouseCursors,
                FillStacksSourceRect,
                Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"))
            {
                myID = 12952,
                upNeighborID = 27346,
                downNeighborID = 106,
                leftNeighborID = 53957,
                region = 15923,
                LeftClickAction = ClickFillStacksButton,
                HoverAction = CommonHelper.HoverPixelZoom
            };
            allClickableComponents.Add(ItemGrabMenu.fillStacksButton);

            // Organize
            ItemGrabMenu.organizeButton = new ClickableTexture(
                "organize",
                this,
                RightWidgetsOffset + Dimensions * new Vector2(1, 3f / 4f),
                Game1.mouseCursors,
                OrganizeSourceRect,
                Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"))
            {
                myID = 106,
                upNeighborID = 12952,
                downNeighborID = 5948,
                leftNeighborID = 53969,
                region = 15923,
                LeftClickAction = ClickOrganizeButton,
                HoverAction = CommonHelper.HoverPixelZoom
            };
            allClickableComponents.Add(ItemGrabMenu.organizeButton);

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
                    0 => 53910, // ItemsToGrabMenu.inventory Row 1 Col 1
                    1 => 53922, // ItemsToGrabMenu.inventory Row 2 Col 1
                    2 => 53934, // ItemsToGrabMenu.inventory Row 3 Col 1
                    3 => 53946, // ItemsToGrabMenu.inventory Row 4 Col 1
                    4 => 53946, // ItemsToGrabMenu.inventory Row 4 Col 1
                    5 => 53958, // ItemsToGrabMenu.inventory Row 5 Col 1
                    6 => 53970, // ItemsToGrabMenu.inventory Row 6 Col 1
                    _ => 53970
                };
                chestTab.RightClickAction = RightClickChestTab;
                chestTab.ScrollAction = ScrollChestTab;
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

            var starButton = new StarButton(this, new Vector2(-1, -1) * Game1.tileSize)
            {
                DrawAction = DrawStarButton,
                LeftClickAction = ClickStarButton
            };
            allClickableComponents.Add(starButton);
        }

        private void SyncItem(NetList<Item, NetRef<Item>> list, int slot, Item oldValue, Item currentItem) =>
            SyncItems();

        private void DrawStarButton(SpriteBatch b, IWidget widget)
        {
            var cc = CommonHelper.OfType<ClickableTextureComponent>(widget);
            cc.sourceRect = ActualChest.Equals(CustomChest)
                ? CommonHelper.StarButtonActive
                : CommonHelper.StarButtonInactive;
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

            cc.sourceRect = CommonHelper.StarButtonActive;

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
            ItemGrabMenu.behaviorOnItemGrab = CustomChest.grabItemFromChest;
            ItemGrabMenu.BehaviorFunction = CustomChest.grabItemFromInventory;

            // Reassign top inventory
            actualInventory = CustomChest.items;
            SyncItems();
        }

        private void ClickFillStacksButton(IWidget widget)
        {
            ItemGrabMenu.FillOutStacks();
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
            ItemGrabMenu.ItemPickMenu.SelectedChestTab = chestTab;
            ItemGrabMenu.ItemPickMenu.Visible = true;
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
