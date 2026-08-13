using UnityEngine;

using Actors.Player;
using Core.Interface;
using UI.Hub;

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
            ShopUI.Instance.Open();
        }
    }
}