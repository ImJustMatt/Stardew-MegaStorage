﻿using MegaStorage.Framework.Models;
using MegaStorage.Framework.Persistence;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.IO;
using MegaStorage.Framework.Interface;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace MegaStorage
{
    public class MegaStorageMod : Mod
    {
        internal static MegaStorageMod Instance { get; private set; }
        internal static IModHelper ModHelper;
        internal static IMonitor ModMonitor;
        internal static IJsonAssetsApi JsonAssets;
        internal static IConvenientChestsApi ConvenientChests;
        internal static int LargeChestId { get; private set; }
        internal static int MagicChestId { get; private set; }
        internal static int SuperMagicChestId { get; private set; }

        /*********
        ** Public methods
        *********/
        public override void Entry(IModHelper modHelper)
        {
            // Make Instance, ModHelper, and ModMonitor static for use in other classes
            Instance = this;
            ModHelper = modHelper ?? throw new ArgumentNullException(nameof(modHelper));
            ModMonitor = Monitor;

            ModMonitor.VerboseLog("Entry of MegaStorageMod");

            ModHelper.ReadConfig<ModConfig>();

            ModHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            ModHelper.Events.Display.MenuChanged += OnMenuChanged;
            ModHelper.Events.Display.WindowResized += OnWindowResized;
        }

        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = ModHelper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JsonAssets is null)
            {
                Monitor.Log("JsonAssets is needed to load Mega Storage chests", LogLevel.Error);
                return;
            }
            JsonAssets.LoadAssets(Path.Combine(ModHelper.DirectoryPath, "assets", "JsonAssets"));
            JsonAssets.IdsAssigned += OnIdsAssigned;

            ConvenientChests = ModHelper.ModRegistry.GetApi<IConvenientChestsApi>("aEnigma.ConvenientChests");
            if (!(ConvenientChests is null))
            {
                ModConfig.Instance.EnableCategories = false;
            }
        }

        private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ItemPatcher.Start();
            SaveManager.Start(new FarmhandMonitor());
        }

        private static void OnIdsAssigned(object sender, EventArgs e)
        {
            LargeChestId = JsonAssets.GetBigCraftableId("Large Chest");
            MagicChestId = JsonAssets.GetBigCraftableId("Magic Chest");
            SuperMagicChestId = JsonAssets.GetBigCraftableId("Super Magic Chest");
            ModMonitor.VerboseLog($"Large Chest ID is {LargeChestId}.");
            ModMonitor.VerboseLog($"Magic Chest ID is {MagicChestId}.");
            ModMonitor.VerboseLog($"Super Magic Chest ID is {SuperMagicChestId}.");
        }

        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("New menu: " + e.NewMenu?.GetType());
            if (e.NewMenu is MegaStorageMenu)
            {
                return;
            }

            if (!(e.NewMenu is ItemGrabMenu itemGrabMenu) || !(itemGrabMenu.context is CustomChest customChest))
            {
                return;
            }

            Game1.activeClickableMenu = customChest.GetItemGrabMenu();
        }

        private static void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if (!(Game1.activeClickableMenu is MegaStorageMenu largeItemGrabMenu))
            {
                return;
            }

            var oldBounds = new Rectangle(0, 0, e.OldSize.X, e.OldSize.Y);
            var newBounds = new Rectangle(0, 0, e.NewSize.X, e.NewSize.Y);

            largeItemGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
        }
    }
}