using UnityEngine;

namespace World
{
    public enum SpawnPointType { Player, Enemy, Boss, LootBox, EscapePoint }

    public class SpawnPoint : MonoBehaviour
    {
        public SpawnPointType type;
    }
}