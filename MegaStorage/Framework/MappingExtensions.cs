﻿using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework
{
    public static class MappingExtensions
    {
        public static Item ToObject(this Item item) =>
            item is CustomChest customChest
                ? new SObject(customChest.TileLocation, customChest.ParentSheetIndex, customChest.Stack)
                : item;

        public static Item ToObject(this Item item, ChestType chestType) =>
            item is CustomChest customChest
                ? new SObject(customChest.TileLocation, CustomChestFactory.CustomChests[chestType], customChest.Stack)
                : item;

        public static Chest ToChest(this Item item)
        {
            if (!(item is CustomChest customChest))
                throw new InvalidOperationException($"Cannot convert {item?.Name} to Chest.");

            var chest = new Chest(customChest.playerChest.Value, customChest.TileLocation)
            {
                name = customChest.name,
                Stack = customChest.Stack,
                ParentSheetIndex = customChest.ParentSheetIndex
            };

            chest.items.AddRange(customChest.items);
            chest.playerChoiceColor.Value = customChest.playerChoiceColor.Value;

            MegaStorageMod.ConvenientChests?.CopyChestData(customChest, chest);

            return chest;
        }

        public static CustomChest ToCustomChest(this Item item, Vector2? tileLocation = null) =>
            item.ToCustomChest(CustomChestFactory.CustomChests.FirstOrDefault(x => x.Value == item.ParentSheetIndex).Key, tileLocation ?? Vector2.Zero);
        public static CustomChest ToCustomChest(this Item item, ChestType chestType, Vector2? tileLocation = null)
        {
            if (!(item is Chest chest))
                throw new InvalidOperationException($"Cannot convert {item?.Name} to CustomChest");

            var customChest = CustomChestFactory.Create(chestType, tileLocation ?? chest.TileLocation);
            customChest.name = chest.name;
            customChest.Stack = chest.Stack;
            customChest.items.AddRange(chest.items);
            customChest.playerChoiceColor.Value = chest.playerChoiceColor.Value;

            MegaStorageMod.ConvenientChests?.CopyChestData(chest, customChest);

            return customChest;
        }
    }
}
