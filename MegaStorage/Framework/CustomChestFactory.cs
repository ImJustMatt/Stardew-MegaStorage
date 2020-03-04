﻿using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace MegaStorage.Framework
{
    public static class CustomChestFactory
    {
        public static IDictionary<ChestType, string> CustomChests =>
            _chestNames ??= new Dictionary<ChestType, string>
            {
                {ChestType.LargeChest, "Large Chest"},
                {ChestType.MagicChest, "Magic Chest"},
                {ChestType.SuperMagicChest, "Super Magic Chest"}
            };

        private static IDictionary<ChestType, string> _chestNames;

        public static bool ShouldBeCustomChest(Item item) =>
            item is SObject obj
            && obj.bigCraftable.Value
            && CustomChests.Any(c =>
                c.Value.Equals(obj.Name, StringComparison.InvariantCultureIgnoreCase)
                || MegaStorageMod.JsonAssets.GetBigCraftableId(c.Value) == obj.ParentSheetIndex);

        public static CustomChest Create(ChestType chestType, Vector2 tileLocation) =>
            chestType switch
            {
                ChestType.LargeChest => new LargeChest(tileLocation),
                ChestType.MagicChest => new MagicChest(tileLocation),
                ChestType.SuperMagicChest => new SuperMagicChest(tileLocation),
                ChestType.InvalidChest => throw new InvalidOperationException("Invalid ChestType"),
                _ => throw new InvalidOperationException("Invalid ChestType")
            };
    }
}
