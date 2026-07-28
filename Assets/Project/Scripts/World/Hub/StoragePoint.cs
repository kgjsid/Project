using UnityEngine;

using Actors.Player;
using Core.Interface;
using UI.Inventory;
using Manager;

namespace World.Hub
{
    public class StoragePoint : MonoBehaviour, IInteractable
    {
        public string GetInteractText()
        {
            return string.Empty;
        }

        public void OnInteract(PlayerController player)
        {
            LootUI.Instance.Open(GameDataManager.Instance.StashInventory);
        }
    }
}