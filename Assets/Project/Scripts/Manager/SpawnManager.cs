using System.Collections.Generic;
using UnityEngine;

using Actors.Player;
using World;

namespace Manager
{
    public class SpawnManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameObject enemyPrefab;
        public GameObject lootBoxPrefab;

        public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

        public PlayerController SpawnAll()
        {
            PlayerController player = null;

            foreach(var spawnPoint in spawnPoints)
            {
                switch(spawnPoint.type)
                {
                    case SpawnPointType.Player:
                        GameObject newPlayer = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                        player = newPlayer.GetComponent<PlayerController>();
                        break;
                    case SpawnPointType.Enemy:
                        Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                        break;
                    case SpawnPointType.LootBox:
                        Instantiate(lootBoxPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                        break;
                }
            }

            return player;
        }
    }
}
