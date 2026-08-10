using UnityEngine;

using Core.System;
using Actors.Player;
using Manager;

namespace UI.HUD
{
    public class PlayerHUDBinder : MonoBehaviour
    {
        [SerializeField] private HealthBarUI healthBarUI;
        [SerializeField] private WeightDisplayUI weightDisplay;

        private void Start()
        {
            RunManager.Instance.SubscribeToPlayerSpawnEvent(Bind);
        }

        private void Bind(PlayerController player)
        {
            healthBarUI?.SetTarget(player.GetComponent<Health>());
            weightDisplay?.SetTarget(player.GetPlayerInventory());
        }
    }
}