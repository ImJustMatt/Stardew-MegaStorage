using System.Linq;
using MegaStorage.API;
using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace MegaStorage.Framework
{
    internal static class ChestExtensions
    {
        public static CustomChest ToCustomChest(this Item item, Vector2 tileLocation)
        {
            var chestData = MegaStorageMod.CustomChests.Single(c => c.ParentSheetIndex == item.ParentSheetIndex);

            var customChest = new CustomChest(chestData, tileLocation)
            {
                Name = item.Name,
                Stack = item.Stack,
                ParentSheetIndex = item.ParentSheetIndex
            };

            if (!(item is Chest chest))
                return customChest;
            customChest.items.CopyFrom(chest.items);
            customChest.playerChoiceColor.Value = chest.playerChoiceColor.Value;
            ConvenientChests.CopyChestData(chest, customChest);

            return customChest;
        }
    }
}
