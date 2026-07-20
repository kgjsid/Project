using UnityEngine;
using Unity.Cinemachine;

using Actors.Player;

namespace Manager
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera playerCamera;

        private void Start()
        {
            RunManager.Instance.SubscribeToPlayerSpawnEvent(FollowPlayer);
        }

        public void FollowPlayer(PlayerController player)
        {
            playerCamera.Follow = player.transform;
        }
    }
}
