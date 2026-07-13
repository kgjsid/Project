using UnityEngine;

using Core.Interface;
using Actors.Player;

namespace World
{
    public class EscapePoint : MonoBehaviour, IInteractable
    {
        public void OnInteract(PlayerController player)
        {
            Debug.Log("End");
        }

        public string GetInteractText()
        {
            return string.Empty;
        }
    }
}