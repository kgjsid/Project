using UnityEngine;

namespace World
{
    public enum SpawnPointType { Player, Enemy, LootBox, EscapePoint }

    public class SpawnPoint : MonoBehaviour
    {
        public SpawnPointType type;
    }
}