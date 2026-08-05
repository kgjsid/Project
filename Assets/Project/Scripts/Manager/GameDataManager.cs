using UnityEngine;

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

        public Inventory StashInventory { get { return stashInventory; } }
        public Inventory LoadoutInventory { get { return loadoutInventory; } }

        public int Gold { get { return gold; } }
        public int DebtRemaining { get { return debtRemaining; } }
        public int CurrentDay { get { return currentDay; } }

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

        public void StoreRunResult(Inventory playerInventory)
        {
            ClearLoadout();

            foreach (var slot in playerInventory.Slots)
            {
                if (slot.IsEmpty()) continue;
                loadoutInventory.AddItem(slot.item, slot.count);
            }
        }

        public void ClearLoadout()
        {
            for (int i = 0; i < loadoutInventory.Slots.Length; i++)
            {
                loadoutInventory.Slots[i].Clear();
            }
            loadoutInventory.NotifyChange();
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
        }

        private void StartNewGame()
        {
            gold = config.startingGold;
            debtRemaining = config.startingDebt;
            currentDay = config.startingDay;
        }
    }
}