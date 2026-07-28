using UnityEngine;
using UnityEngine.SceneManagement;

using Core.Interface;
using Actors.Player;

namespace World.Hub
{
    public class DungeonEntrance : MonoBehaviour, IInteractable
    {
        [SerializeField] private string dungeonSceneName = "DungeonScene";

        public void OnInteract(PlayerController player)
        {
            SceneManager.LoadScene(dungeonSceneName);
        }

        public string GetInteractText()
        {
            return string.Empty;
        }
    }
}