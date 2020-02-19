﻿using MegaStorage.Framework.Interface;
using StardewValley;

namespace MegaStorage.Framework.Models
{
    public class LargeChest : CustomChest
    {
        public override int Capacity => 72;
        public override ChestType ChestType => ChestType.LargeChest;
        protected override LargeItemGrabMenu CreateItemGrabMenu() => new LargeItemGrabMenu(this);
        public override Item getOne() => new LargeChest();

        public LargeChest() : base(MegaStorageMod.LargeChestId, ModConfig.Instance.LargeChest)
        {
            name = "Large Chest";
        }
    }
}