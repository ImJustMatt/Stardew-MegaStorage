using System;
using System.Collections.Generic;
using System.Globalization;

namespace MegaStorage.Framework.Models
{
    internal class ChestData
    {
        /*********
        ** Fields
        *********/
        public int ParentSheetIndex { get; protected set; }
        public string Name { get; protected set; }
        public int Capacity { get; protected set; }
        public bool EnableChestTabs { get; protected set; }
        public bool EnableRemoteStorage { get; protected set; }

        /*********
        ** Public methods
        *********/
        public ChestData(
            int parentSheetIndex,
            string name,
            int capacity,
            bool enableChestTabs,
            bool enableRemoteStorage)
        {
            ParentSheetIndex = parentSheetIndex;
            Name = name;
            Capacity = capacity;
            EnableChestTabs = enableChestTabs;
            EnableRemoteStorage = enableRemoteStorage;
        }

        public Dictionary<string, string> ToSaveData()
        {
            return new Dictionary<string, string>
        {
            { "ParentSheetIndex", ParentSheetIndex.ToString(CultureInfo.InvariantCulture) },
            { "Name", Name },
            { "Capacity", Capacity.ToString(CultureInfo.InvariantCulture) },
            { "EnableChestTabs", EnableChestTabs ? "true" : "false" },
            { "EnableRemoteStorage" , EnableRemoteStorage ? "true" : "false" }
        };
        }

        public static ChestData FromSaveData(Dictionary<string, string> saveData)
        {
            return new ChestData(
Convert.ToInt32(saveData["ParentSheetIndex"], CultureInfo.InvariantCulture),
saveData["Name"],
Convert.ToInt32(saveData["Capacity"], CultureInfo.InvariantCulture),
saveData["EnableChestTabs"] == "true",
saveData["EnableRemoteStorage"] == "true");
        }

        /*********
        ** Private methods
        *********/
    }
}
