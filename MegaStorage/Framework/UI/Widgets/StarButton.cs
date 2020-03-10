using MegaStorage.Framework.UI.Menus;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MegaStorage.Framework.UI.Widgets
{
    internal class StarButton : ClickableTexture
    {
        /*********
        ** Fields
        *********/


        /*********
        ** Public methods
        *********/
        public StarButton(IMenu parentMenu, Vector2 offset)
            : base("starButton", parentMenu, offset, Game1.mouseCursors, CommonHelper.StarButtonInactive, "Make Main Chest")
        {
            myID = 239864;
            downNeighborID = 239865;
            rightNeighborID = 4343;
            HoverAction = CommonHelper.HoverPixelZoom;
        }

        /*********
        ** Private methods
        *********/
    }
}
