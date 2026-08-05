using UnityEngine;

namespace Manager
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Start")]
        public int startingGold = 0;
        public int startingDebt = 1000;
        public int startingDay = 1;

        [Header("Inventory")]
        public int stashCapacity = 40;
        public int loadoutCapacity = 20;
        public float loadoutMaxWeight = 20f;
    }
}