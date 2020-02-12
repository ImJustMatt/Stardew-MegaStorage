﻿using System;
using System.Collections.Generic;
using System.Linq;
using MegaStorage.Models;
using StardewValley;
using Object = StardewValley.Object;

namespace MegaStorage.Mapping
{
    public static class CustomChestFactory
    {
        private static List<CustomChest> _customChests;
        public static List<CustomChest> CustomChests =>
            _customChests ?? (_customChests = new List<CustomChest>
            {
                new LargeChest(),
                new MagicChest()
            });

        public static bool ShouldBeCustomChest(Item item)
        {
            if (!(item is Object))
                return false;
            if (!((Object)item).bigCraftable.Value)
                return false;
            return CustomChests.Any(x => x.ParentSheetIndex == item.ParentSheetIndex);
        }

        public static CustomChest Create(int id)
        {
            var chestType = CustomChests.Single(x => x.ParentSheetIndex == id).ChestType;
            return Create(chestType);
        }

        public static CustomChest Create(ChestType chestType)
        {
            switch (chestType)
            {
                case ChestType.LargeChest:
                    return new LargeChest();
                case ChestType.MagicChest:
                    return new MagicChest();
                default:
                    throw new InvalidOperationException("Invalid ChestType");
            }
        }

    }
}
