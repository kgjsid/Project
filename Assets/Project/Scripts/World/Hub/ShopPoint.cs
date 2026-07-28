using UnityEngine;

using Actors.Player;
using Core.Interface;

namespace World.Hub
{
    public class ShopPoint : MonoBehaviour, IInteractable
    {
        public string GetInteractText()
        {
            return string.Empty;
        }

        public void OnInteract(PlayerController player)
        {
            // Open Shop UI
        }
    }
}