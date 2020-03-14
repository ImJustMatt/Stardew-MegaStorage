using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Globalization;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ItemSlot : BaseWidget
    {
        /*********
        ** Fields
        *********/
        public int Slot { get; set; }
        protected internal BaseInventory InventoryMenu => CommonHelper.OfType<BaseInventory>(ParentMenu);

        /*********
        ** Public methods
        *********/
        public ItemSlot(
            IMenu parentMenu,
            Vector2 offset,
            int slot,
            Item item = null)
            : base(
                    slot.ToString(CultureInfo.InvariantCulture),
                    parentMenu,
                    offset,
                    Game1.menuTexture,
                    Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57))
        {
            Slot = slot;
            Color = Color.White * 0.0f;
            this.item = item;
            Events.Draw = Draw;
            Events.LeftClick = LeftClick;
            Events.RightClick = RightClick;
        }

        /*********
        ** Private methods
        *********/
        protected internal void LeftClick(IWidget widget)
        {
            InventoryMenu.ItemSlot ??= this;
        }

        protected internal void RightClick(IWidget widget)
        {
            InventoryMenu.ItemSlot ??= this;
        }
    }
}
