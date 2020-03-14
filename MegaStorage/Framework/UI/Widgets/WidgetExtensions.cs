namespace MegaStorage.Framework.UI.Widgets
{
    internal static class WidgetExtensions
    {
        public static void GameWindowSizeChanged(this IWidget widget) =>
            widget.Position = widget.ParentMenu.Position + widget.Offset;
    }
}
