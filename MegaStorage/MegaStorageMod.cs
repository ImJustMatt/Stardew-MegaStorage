using MegaStorage.API;
using MegaStorage.Framework;
using MegaStorage.Framework.Models;
using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MegaStorage
{
    public class MegaStorageMod : Mod
    {
        internal new static IModHelper Helper;
        internal static InterfaceHost ActiveItemGrabMenu { get; set; }
        internal static CustomChest MainChest { get; set; }
        internal static IList<ChestData> CustomChests = new List<ChestData>();

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper modHelper)
        {
            // Make Instance, ModHelper, and Log static for use in other classes
            Log.Monitor = Monitor;
            Helper = CommonHelper.NonNull(modHelper);

            Helper.ReadConfig<ModConfig>();
            MegaStorageAPI.Instance = new MegaStorageAPI();

            // Events
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.World.ObjectListChanged += OnObjectListChanged;
            Helper.Events.Display.MenuChanged += OnMenuChanged;
            Helper.Events.Display.WindowResized += OnWindowResized;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        public override object GetApi() => MegaStorageAPI.Instance;

        internal static void StashItems()
        {
            var items = Game1.player.Items.Where(ModConfig.Instance.StashItems.BelongsTo);

            foreach (var item in items)
            {
                if (item.Stack == 0)
                    item.Stack = 1;

                var addedItem = MainChest.addItem(item);
                if (addedItem is null)
                    Game1.player.removeItemFromInventory(item);
                else
                    addedItem = Game1.player.addItemToInventory(addedItem);

                MainChest.clearNulls();
            }
        }

        /*********
        ** Private methods
        *********/
        private static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Load APIs
            ConvenientChests.API = Helper.ModRegistry.GetApi<IConvenientChestsApi>("aEnigma.ConvenientChests");
            JsonAssets.API = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            SaveAnywhere.API = Helper.ModRegistry.GetApi<ISaveAnywhereApi>("Omegasis.SaveAnywhere");

            if (JsonAssets.API is null)
            {
                Log.Error("JsonAssets is needed to load Mega Storage chests");
                return;
            }

            JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));
            JsonAssets.API.IdsAssigned += OnIdsAssigned;
        }

        private static void OnIdsAssigned(object sender, EventArgs e)
        {
            // Large Chest
            CustomChests.Add(
                new ChestData(
                    JsonAssets.GetBigCraftableId("Large Chest"),
                    "LargeChest",
                    72,
                    false,
                    false));

            // Magic Chest
            CustomChests.Add(
                new ChestData(JsonAssets.GetBigCraftableId("Magic Chest"),
                    "Magic Chest",
                    int.MaxValue,
                    true,
                    false));

            // Super Magic Chest
            CustomChests.Add(
                new ChestData(
                    JsonAssets.GetBigCraftableId("Super Magic Chest"),
                    "Super Magic Chest",
                    int.MaxValue,
                    true,
                    true));
        }

        private static void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            Log.Verbose("OnObjectListChanged");

            if (e.Added.Count() != 1)
                return;

            var itemPosition = e.Added.Single();
            var pos = itemPosition.Key;
            var item = itemPosition.Value;

            if (item is CustomChest || CustomChests.All(c => c.ParentSheetIndex != item.ParentSheetIndex))
                return;

            Log.Verbose("OnObjectListChanged: converting");
            var customChest = item.ToCustomChest(pos);
            e.Location.objects[pos] = customChest;
        }

        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            Log.Verbose("New menu: " + e.NewMenu?.GetType());
            switch (e.NewMenu)
            {
                case InterfaceHost customItemGrabMenu:
                    ActiveItemGrabMenu = customItemGrabMenu;
                    return;
                case ItemGrabMenu itemGrabMenu:
                    if (itemGrabMenu.context is CustomChest customChest)
                    {
                        ActiveItemGrabMenu = customChest.CreateItemGrabMenu();
                        Game1.activeClickableMenu = ActiveItemGrabMenu;
                    }
                    break;
                case null:
                    ActiveItemGrabMenu = null;
                    break;
            }
        }

        private static void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if (!(Game1.activeClickableMenu is InterfaceHost customItemGrabMenu))
                return;
            var oldBounds = new Rectangle(0, 0, e.OldSize.X, e.OldSize.Y);
            var newBounds = new Rectangle(0, 0, e.NewSize.X, e.NewSize.Y);
            customItemGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
        }

        private static void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Equals(ModConfig.Instance.StashKey)
                || e.Button.Equals(ModConfig.Instance.StashButton))
            {
                //ActiveItemGrabMenu?.StashItems();
            }
            else if (e.Button.Equals(ModConfig.Instance.StashAnywhereKey) && !(MainChest is null))
            {
                StashItems();
            }
        }
    }
}