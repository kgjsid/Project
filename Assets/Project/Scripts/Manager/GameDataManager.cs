using UnityEngine;

using Item.Data;
using Core.System;

namespace Manager
{
    public class GameDataManager : MonoBehaviour
    {
        private static GameDataManager instance;
        public static GameDataManager Instance { get { return instance; } }

        [Header("Inventory Capacity")]
        [SerializeField] private int stashCapacity = 40;
        [SerializeField] private int loadoutCapacity = 20;
        [SerializeField] private float loadoutMaxWeight = 20f;

        [Header("Progress")]
        [SerializeField] private int gold = 0;
        [SerializeField] private int debtRemaining = 1000;
        [SerializeField] private int currentDay = 1;

        [SerializeField] private SaveManager saveManager;
        [SerializeField] private GameConfig config;

        private Inventory stashInventory;               // 창고 인벤토리
        private Inventory loadoutInventory;             // 허브용 인벤토리
        private Inventory loadoutEquipInventory;        // 허브용 장비 인벤토리

        public Inventory StashInventory { get { return stashInventory; } }
        public Inventory LoadoutInventory { get { return loadoutInventory; } }
        public Inventory LoadoutEquipInventory { get { return loadoutEquipInventory; } }

        public int Gold { get { return gold; } }
        public int DebtRemaining { get { return debtRemaining; } }
        public int CurrentDay { get { return currentDay; } }

        private const int LOADOUT_EQUIP_SLOT_COUNT = 7;

        private void Awake()
        {
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            CreateInventories();

            saveManager?.Load(this);
        }

        public void ApplyLoadoutTo(Inventory playerInventory)
        {
            foreach(var slot in loadoutInventory.Slots)
            {
                if(slot.IsEmpty()) continue;
                playerInventory.AddItem(slot.item, slot.count);
            }
        }

        public void StoreRunResult(Inventory playerBagInventory, Inventory playerEquipInventory)
        {
            ClearLoadout();

            foreach (var slot in playerBagInventory.Slots)
            {
                if (slot.IsEmpty()) continue;
                loadoutInventory.AddItem(slot.item, slot.count);
            }
            loadoutInventory.NotifyChange();

            for(int i = 0; i < playerEquipInventory.Slots.Length; i++)
            {
                var slot = playerEquipInventory.Slots[i];
                if (slot.IsEmpty()) continue;
                loadoutEquipInventory.Slots[i] = new ItemSlot(slot.item, slot.count);
            }
            loadoutEquipInventory.NotifyChange();
        }

        public void ClearLoadout()
        {
            for (int i = 0; i < loadoutInventory.Slots.Length; i++)
            {
                loadoutInventory.Slots[i].Clear();
            }
            loadoutInventory.NotifyChange();

            for (int i = 0; i < loadoutEquipInventory.Slots.Length; i++)
            {
                loadoutEquipInventory.Slots[i].Clear();
            }
            loadoutEquipInventory.NotifyChange();
        }

        public void AddGold(int amount)
        {
            gold += amount;
        }

        public bool TryPayDebt(int amount)
        {
            if (amount <= 0 || gold < amount) return false;

            gold -= amount;
            debtRemaining -= amount;
            return true;
        }

        public void AdvanceDay()
        {
            currentDay++;
        }

        public void RestoreProgress(int gold, int debtRemaining, int currentDay)
        {
            this.gold = gold;
            this.debtRemaining = debtRemaining;
            this.currentDay = currentDay;
        }

        public void Save()
        {
            saveManager?.Save(this);
        }

        private void CreateInventories()
        {
            stashInventory = gameObject.AddComponent<Inventory>();
            stashInventory.InitSlot(stashCapacity);
            stashInventory.BaseMaxWeight = float.MaxValue;

            loadoutInventory = gameObject.AddComponent<Inventory>();
            loadoutInventory.InitSlot(loadoutCapacity);
            loadoutInventory.BaseMaxWeight = loadoutMaxWeight;

            loadoutEquipInventory = gameObject.AddComponent<Inventory>();
            loadoutEquipInventory.InitSlot(LOADOUT_EQUIP_SLOT_COUNT);
            loadoutEquipInventory.BaseMaxWeight = float.MaxValue;
        }

        private void StartNewGame()
        {
            gold = config.startingGold;
            debtRemaining = config.startingDebt;
            currentDay = config.startingDay;
        }
    }
}