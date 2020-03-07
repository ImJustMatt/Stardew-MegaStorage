using MegaStorage.Framework.Models;
using MegaStorage.Framework.Persistence;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace MegaStorage.Framework
{
    internal static class ItemPatcher
    {
        public static void Start()
        {
            MegaStorageMod.ModHelper.Events.Player.InventoryChanged += OnInventoryChanged;
            MegaStorageMod.ModHelper.Events.World.ChestInventoryChanged += OnChestInventoryChanged;
            MegaStorageMod.ModHelper.Events.World.DebrisListChanged += OnDebrisListChanged;
            MegaStorageMod.ModHelper.Events.World.ObjectListChanged += OnObjectListChanged;
        }

        private static void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("OnInventoryChanged");
            if (!e.IsLocalPlayer || e.Added.Count() != 1)
                return;

            var addedItem = e.Added.Single();
            if (!(addedItem is CustomChest customChest))
                return;

            var index = Game1.player.Items.IndexOf(addedItem);
            MegaStorageMod.ModMonitor.VerboseLog($"OnInventoryChanged: converting at {index}");
            if (index > -1)
            {
                Game1.player.Items[index] = customChest.items.Count == 0
                    ? customChest.ToObject()
                    : customChest.ToChest();
            }
        }

        private static void OnChestInventoryChanged(object sender, ChestInventoryChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("OnChestInventoryChanged");
            if (e.Added.Count() != 1)
                return;

            var addedItem = e.Added.Single();
            if (!(addedItem is CustomChest customChest))
                return;

            var index = e.Chest.items.IndexOf(addedItem);
            MegaStorageMod.ModMonitor.VerboseLog($"OnChestInventoryChanged: converting at {index}");
            if (index > -1)
            {
                e.Chest.items[index] = customChest.items.Count == 0
                    ? customChest.ToObject()
                    : customChest.ToChest();
            }
        }

        private static void OnDebrisListChanged(object sender, DebrisListChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("OnDebrisListChanged");
            if (e.Added.Count() != 1)
                return;

            var debris = e.Added.Single();
            if (!(debris.item is CustomChest customChest))
                return;

            MegaStorageMod.ModMonitor.VerboseLog("OnDebrisListChanged: converting");
            debris.item = customChest.items.Count == 0
                ? customChest.ToObject()
                : customChest.ToChest();
        }

        private static void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            MegaStorageMod.ModMonitor.VerboseLog("OnObjectListChanged");

            if (e.Added.Count() != 1 && e.Removed.Count() != 1)
                return;

            var itemPosition = e.Added.Count() == 1
                ? e.Added.Single()
                : e.Removed.Single();

            var pos = itemPosition.Key;
            var item = itemPosition.Value;
            var key = new Tuple<GameLocation, Vector2>(e.Location, pos);

            if (e.Added.Count() == 1
                && !(item is CustomChest)
                && CustomChestFactory.ShouldBeCustomChest(item))
            {
                MegaStorageMod.ModMonitor.VerboseLog("OnObjectListChanged: converting");
                var customChest = item.ToCustomChest(pos);
                e.Location.objects[pos] = customChest;
                if (!StateManager.PlacedChests.ContainsKey(key))
                    StateManager.PlacedChests.Add(key, customChest);
                else
                    StateManager.PlacedChests[key] = customChest;
            }
            else if (e.Removed.Count() == 1 && item is CustomChest)
            {
                MegaStorageMod.ModMonitor.VerboseLog("OnObjectListChanged: untrack");
                if (StateManager.PlacedChests.ContainsKey(key))
                    StateManager.PlacedChests.Remove(new Tuple<GameLocation, Vector2>(e.Location, pos));
            }
        }
    }
}
