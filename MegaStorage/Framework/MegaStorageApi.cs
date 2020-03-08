using MegaStorage.Framework.UI;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace MegaStorage.Framework
{
    public class MegaStorageApi : IMegaStorageApi
    {
        public static MegaStorageApi Instance { get; private set; }

        /*********
        ** Events
        *********/
        public event EventHandler<ICustomChestEventArgs> BeforeVisibleItemsRefreshed;
        public event EventHandler<ICustomChestEventArgs> AfterVisibleItemsRefreshed;
        public event EventHandler<ICustomChestEventArgs> ColorPickerToggleButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeFillStacksButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterFillStacksButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeOrganizeButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterOrganizeButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeStarButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterStarButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeOkButtonClicked;
        public event EventHandler<ICustomChestEventArgs> AfterOkButtonClicked;
        public event EventHandler<ICustomChestEventArgs> BeforeTrashCanClicked;
        public event EventHandler<ICustomChestEventArgs> AfterTrashCanClicked;

        /*********
        ** Public methods
        *********/
        public MegaStorageApi()
        {
            Instance = this;
        }
        public Rectangle? GetItemsToGrabMenuBounds() => I?.ItemsToGrabMenu.Bounds;
        public Rectangle? GetInventoryBounds() => I?.inventory.Bounds;
        public Vector2? GetItemsToGrabMenuDimensions() => I?.ItemsToGrabMenu.Dimensions;
        public Vector2? GetInventoryDimensions() => I?.inventory.Dimensions;
        public Vector2? GetItemsToGrabMenuPosition() => I?.ItemsToGrabMenu.Position;
        public Vector2? GetInventoryPosition() => I?.inventory.Position;
        public void RefreshItems() => I?.ItemsToGrabMenu.RefreshItems();
        public void StashItems()
        {
            if (MegaStorageMod.ActiveItemGrabMenu is null)
            {
                MegaStorageMod.StashItems();
            }
            else
            {
                I?.StashItems();
            }
        }

        public void ClickColorPickerToggleButton() => I?.ClickColorPickerToggleButton();
        public void ClickFillStacksButton() => I?.ClickFillStacksButton();
        public void ClickOrganizeButton() => I?.ClickOrganizeButton();
        public void ClickStarButton() => I?.ClickStarButton();
        public void ClickOkButton() => I?.ClickOkButton();
        public void ClickTrashCan() => I?.ClickTrashCan();
        public void ClickCategoryButton(string categoryName) => I?.ClickCategoryButton(categoryName);
        public void ScrollCategory(int direction) => I?.ScrollCategory(direction);

        /*********
        ** Private methods
        *********/
        private static CustomItemGrabMenu I =>
            (Game1.activeClickableMenu is CustomItemGrabMenu customItemGrabMenu) ? customItemGrabMenu : null;

        internal static void InvokeBeforeVisibleItemsRefreshed(CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeVisibleItemsRefreshed?.Invoke(null, customChestEventArgs);

        internal static void InvokeAfterVisibleItemsRefreshed(CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterVisibleItemsRefreshed?.Invoke(null, customChestEventArgs);

        internal static void InvokeColorPickerToggleButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.ColorPickerToggleButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeBeforeFillStacksButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeFillStacksButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeAfterFillStacksButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterFillStacksButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeBeforeOrganizeButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeOrganizeButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeAfterOrganizeButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterOrganizeButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeBeforeStarButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeStarButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeAfterStarButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterStarButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeBeforeOkButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeOkButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeAfterOkButtonClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterOkButtonClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeBeforeTrashCanClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.BeforeTrashCanClicked?.Invoke(null, customChestEventArgs);

        internal static void InvokeAfterTrashCanClicked(CustomChestEventArgs customChestEventArgs) =>
            Instance.AfterTrashCanClicked?.Invoke(null, customChestEventArgs);
    }

    public class CustomChestEventArgs : EventArgs, ICustomChestEventArgs
    {
        public IList<Item> VisibleItems { get; set; }
        public IList<Item> AllItems { get; set; }
        public string CurrentCategory { get; set; }
        public Item HeldItem { get; set; }
        public Chest RemoteChest { get; set; }
    }
}
