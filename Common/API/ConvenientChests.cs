using StardewValley.Objects;

namespace MegaStorage.API
{
    internal static class ConvenientChests
    {
        public static IConvenientChestsApi API { get; set; }
        public static void CopyChestData(Chest source, Chest target) => API?.CopyChestData(source, target);
    }

    public interface IConvenientChestsApi
    {
        void CopyChestData(Chest source, Chest target);
    }
}
