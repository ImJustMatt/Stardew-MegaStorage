using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                    Sprites.Inventory.GrayedOut)
        {
            Slot = slot;
            scale = Game1.pixelZoom;
            this.item = item;
            Events.LeftClick = LeftClick;
            Events.RightClick = RightClick;
        }

        /*********
        ** Private methods
        *********/
        protected internal override void Draw(SpriteBatch b, IWidget widget)
        {
            if (!(item is null))
                drawItem(b);
        }
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
