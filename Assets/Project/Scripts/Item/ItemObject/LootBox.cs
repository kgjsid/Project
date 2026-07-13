using UnityEngine;

using Core.System;
using Core.Interface;
using Actors.Player;
using Actors.UI;

namespace Item.ItemObject
{
    public class LootBox : MonoBehaviour, IInteractable
    {
        public enum BoxState { Closed, Opened }

        private Inventory inventory;

        private int boxSize = 12;
        private BoxState currentState = BoxState.Closed;

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
       
        private void AddComponent()
        {
            Inventory = gameObject.AddComponent<Inventory>();
            Inventory.InitSlot(boxSize);
        }
    }
}
