using System.Collections.Generic;
using UnityEngine;

using World;
using World.Map;

namespace Manager
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MapGenerator.MapSettings mapSettings;
        [SerializeField] private Transform mapRoot;
        [SerializeField] private float cellSize = 20f;

        [SerializeField] private bool useFixedSeed = false;
        [SerializeField] private int seed = 0;

        private MapGenerator generator;

        public Room StartRoom { get { return generator?.StartRoom; } }
        public Room BossRoom { get { return generator?.BossRoom; } }

        public void GenerateMap()
        {
            if (useFixedSeed) Random.InitState(seed);

            generator = new MapGenerator(mapSettings, mapRoot, cellSize);
            generator.Generate();
        }

        public List<SpawnPoint> CollectSpawnPoints()
        {
            List<SpawnPoint> result = new List<SpawnPoint>();

            foreach (Room room in generator.GetAllRooms())
            {
                result.AddRange(room.GetSpawnPoints());
            }

            return result;
        }
    }
}