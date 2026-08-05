using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Core.System;
using Item.Data;

namespace Manager
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private ItemDatabase itemDatabase;

        private const string FILE_NAME = "save.json";

        public void Save(GameDataManager gameDataManager)
        {
            SaveData data = new SaveData
            {
                gold = gameDataManager.Gold,
                debtRemaining = gameDataManager.DebtRemaining,
                currentDay = gameDataManager.CurrentDay
            };

            WriteInventory(gameDataManager.StashInventory, data.stash);
            WriteInventory(gameDataManager.LoadoutInventory, data.loadout);

            try
            {
                File.WriteAllText(GetFilePath(), JsonUtility.ToJson(data, true));
            }
            catch(IOException e)
            {
                Debug.LogError($"SaveManager : 저장 실패 - {e.Message}");
            }
        }

        public void Load(GameDataManager gameDataManager)
        {
            if (!HasSave()) return;

            SaveData save = null;

            try
            {
                save = JsonUtility.FromJson<SaveData>(File.ReadAllText(GetFilePath()));
            }
            catch(System.Exception e)
            {
                Debug.LogError($"SaveManager : 세이브 파일이 손상되었습니다 - {e.Message}");
                return;
            }

            if (save == null) return;

            ReadInventory(save.stash, gameDataManager.StashInventory);
            ReadInventory(save.loadout, gameDataManager.LoadoutInventory);
        }

        public void DeleteSave()
        {
            if (HasSave()) File.Delete(GetFilePath());
        }

        public bool HasSave()
        {
            return File.Exists(GetFilePath());
        }

        private void WriteInventory(Inventory inventory, List<SlotSaveData> target)
        {
            for (int slotIndex = 0; slotIndex < inventory.Slots.Length; slotIndex++)
            {
                if (inventory.Slots[slotIndex].IsEmpty()) continue;

                target.Add(new SlotSaveData
                {
                    slotIndex = slotIndex,
                    itemId = inventory.Slots[slotIndex].item.id,
                    count = inventory.Slots[slotIndex].count
                });
            }
        }

        private void ReadInventory(List<SlotSaveData> source, Inventory inventory)
        {
            for(int slotIndex = 0; slotIndex < inventory.Slots.Length; slotIndex++)
            {
                inventory.Slots[slotIndex].Clear();
            }

            foreach(SlotSaveData data in source)
            {
                ItemData item = itemDatabase.GetById(data.itemId);

                if (item == null)
                {
                    Debug.LogWarning($"SaveManager : 알 수 없는 아이템 ID '{data.itemId}'");
                    continue;
                }

                if(data.slotIndex < 0 || data.slotIndex >= inventory.Slots.Length)
                {
                    inventory.AddItem(item, data.count);
                    continue;
                }

                inventory.Slots[data.slotIndex] = new ItemSlot(item, data.count);
            }

            inventory.NotifyChange();
        }

        private string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, FILE_NAME);
        }
    }
}