using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Globalization;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class ItemSlot : ClickableTexture
    {
        /*********
        ** Fields
        *********/
        public int Slot { get; set; }

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
            DrawAction = Draw;
            LeftClickAction = Click;
            RightClickAction = RightClick;
        }

        /*********
        ** Private methods
        *********/
        protected internal void Click(IWidget widget)
        {
            InventoryMenu.ItemSlot ??= this;
        }

        protected internal void RightClick(IWidget widget)
        {
            InventoryMenu.ItemSlot ??= this;
        }
    }
}
