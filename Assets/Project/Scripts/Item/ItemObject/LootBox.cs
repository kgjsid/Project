using UnityEngine;

using Core.System;
using Core.Interface;
using Actors.Player;
using UI.Inventory;
using Item.Data;

namespace Item.ItemObject
{
    public class LootBox : MonoBehaviour, IInteractable
    {
        public enum BoxState { Closed, Opened }

        [SerializeField] private LootTable lootTable;

        private Inventory inventory;

        private int boxSize = 20;

        public Inventory Inventory { private set { inventory = value; } get { return inventory; } }

        private void Awake()
        {
            AddComponent();
        }

        public void OnInteract(PlayerController player)
        {
            LootUI.Instance.Open(Inventory);
        }

        public string GetInteractText()
        {
            return string.Empty;
        }

        public void FillFromTable()
        {
            if (lootTable == null) return;

            var rolled = lootTable.Roll();
            foreach (var (item, count) in rolled)
            {
                Inventory.AddItem(item, count);
            }
        }

        private void AddComponent()
        {
            Inventory = gameObject.AddComponent<Inventory>();
            Inventory.InitSlot(boxSize);
        }
    }
}
