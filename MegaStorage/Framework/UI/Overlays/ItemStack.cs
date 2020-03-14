using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;

namespace MegaStorage.Framework.UI.Overlays
{
    internal class ItemStack : BaseOverlay
    {
        /*********
        ** Fields
        *********/


        /*********
        ** Public methods
        *********/
        protected ItemStack(IMenu parentMenu, Vector2 offset)
            : base(parentMenu, offset)
        {
        }
    }
}
