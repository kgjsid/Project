using UnityEngine;

namespace World
{
    public enum SpawnPointType { Player, Enemy, LootBox }

    public class SpawnPoint : MonoBehaviour
    {
        public SpawnPointType type;
    }
}