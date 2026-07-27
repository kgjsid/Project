using System.Collections.Generic;
using UnityEngine;

namespace World.Map
{
    public class Room : MonoBehaviour
    {
        private Dictionary<Direction, List<GameObject>> doorObjects = new Dictionary<Direction, List<GameObject>>();
        private Dictionary<Direction, List<GameObject>> wallObjects = new Dictionary<Direction, List<GameObject>>();

        private RoomData roomData;
        private Vector2Int gridPosition;
        private int generation;

        public RoomData RoomData { get { return roomData; } set { roomData = value; } }
        public Vector2Int GridPosition { get { return gridPosition; } set { gridPosition = value; } }
        public int Generation { get { return generation; } }

        public void Init(RoomData roomData, Vector2Int gridPosition, int generation)
        {
            this.roomData = roomData;
            this.gridPosition = gridPosition;
            this.generation = generation;
        }

        public void RegisterDoorPair(Direction dir, GameObject door, GameObject wall)
        {
            if (!doorObjects.ContainsKey(dir)) doorObjects[dir] = new List<GameObject>();
            if (!wallObjects.ContainsKey(dir)) wallObjects[dir] = new List<GameObject>();

            doorObjects[dir].Add(door);
            wallObjects[dir].Add(wall);
        }

        /// <summary>
        /// 인접한 방이 있는지 검사 후 문처리
        /// </summary>
        /// <param name="allRooms"></param>
        public void CheckConnection(Dictionary<Vector2Int, Room> allRooms)
        {
            for (int d = 0; d < (int)Direction.End; d++)
            {
                Direction dir = (Direction)d;
                Vector2Int neighborPos = gridPosition + DirectionUtil.ToOffset(dir);
                bool hasNeighbor = allRooms.TryGetValue(neighborPos, out Room neighbor) && neighbor != this;

                if (doorObjects.TryGetValue(dir, out var doors))
                    foreach (var door in doors) door.SetActive(hasNeighbor);

                if (wallObjects.TryGetValue(dir, out var walls))
                    foreach (var wall in walls) wall.SetActive(!hasNeighbor);
            }
        }

        public SpawnPoint[] GetSpawnPoints()
        {
            return GetComponentsInChildren<SpawnPoint>(true);
        }
    }
}