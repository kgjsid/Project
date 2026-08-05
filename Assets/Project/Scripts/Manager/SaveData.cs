using System;
using System.Collections.Generic;

namespace Manager
{
    [Serializable]
    public class SlotSaveData
    {
        public int slotIndex;
        public string itemId;
        public int count;
    }

    [Serializable]
    public class SaveData
    {
        public int version = 1;

        public int gold;
        public int debtRemaining;
        public int currentDay;

        public List<SlotSaveData> stash = new List<SlotSaveData>();
        public List<SlotSaveData> loadout = new List<SlotSaveData>();
    }
}