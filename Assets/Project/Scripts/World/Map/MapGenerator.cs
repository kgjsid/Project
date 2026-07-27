using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;

namespace World.Map
{
    public class MapGenerator
    {
        [System.Serializable]
        public class MapSettings
        {
            public RoomData startRoomData;
            public List<RoomData> normalRoomPool;
            public RoomData bossRoomData;
            public int maxGeneration = 5;
            public float roomsPerGenerationPercent = 0.7f;
        }

        private MapSettings mapSettings;
        private Dictionary<Vector2Int, Room> placedRooms = new Dictionary<Vector2Int, Room>();
        private RoomChecker roomChecker;
        private Transform mapRoot;
        private float cellSize;

        private Room startRoom;
        private Room bossRoom;

        public Room StartRoom { get { return startRoom; } set { startRoom = value; } }
        public Room BossRoom { get { return bossRoom; } set { bossRoom = value; } }

        public MapGenerator(MapSettings mapSettings, Transform mapRoot, float cellSize)
        {
            this.mapSettings = mapSettings;
            this.mapRoot = mapRoot;
            this.cellSize = cellSize;
            roomChecker = new RoomChecker(placedRooms);
        }

        public void Generate()
        {
            startRoom = PlaceRoom(mapSettings.startRoomData, Vector2Int.zero, 0);

            List<Room> currentGeneration = new List<Room> { startRoom };

            for(int generationIndex = 1; generationIndex <= mapSettings.maxGeneration; generationIndex++)
            {
                if(generationIndex == mapSettings.maxGeneration)
                {
                    PlaceBossRoom(currentGeneration, generationIndex);
                    break;
                }
                currentGeneration = ExpandGeneration(currentGeneration, generationIndex);
            }

            // 모든 방 배치가 끝난 뒤에 문/벽 결정
            foreach (Room room in GetAllRooms())
            {
                room.CheckConnection(placedRooms);
            }
        }

        /// <summary>
        /// placedRooms는 여러 칸짜리 방이 중복 등록되므로, 중복 제거한 실제 방 목록을 반환.
        /// </summary>
        public List<Room> GetAllRooms()
        {
            HashSet<Room> uniqueRoom = new HashSet<Room>(placedRooms.Values);
            return new List<Room>(uniqueRoom);
        }

        private List<Room> ExpandGeneration(List<Room> previousGeneration, int generation)
        {
            List<Room> newRooms = new List<Room>();

            foreach(Room parent in previousGeneration)
            {
                for(int dirIndex = 0; dirIndex < (int)Direction.End; dirIndex++)
                {
                    Direction dir = (Direction)dirIndex;

                    // 맵이 생성될 확률 계산
                    if (Random.value > mapSettings.roomsPerGenerationPercent) continue;
                    // 맵이 생성될 수 있는지 확인
                    if (!roomChecker.TryGetEmptyNeighbor(parent.GridPosition, dir, out Vector2Int newPos)) continue;

                    RoomData picked = roomChecker.PickWeightedRandom(mapSettings.normalRoomPool, generation);
                    if (picked == null) continue;

                    newRooms.Add(PlaceRoom(picked, newPos, generation));
                }
            }

            // 만약 확률 계산에서 전부 0이 나온 경우 --> 강제로 하나 생성
            if (newRooms.Count == 0)
            {
                Room forced = ForcePlaceOne(previousGeneration, generation);
                if (forced != null) newRooms.Add(forced);
            }

            return newRooms;
        }

        private Room ForcePlaceOne(List<Room> candidates, int generation)
        {
            foreach (Room parent in candidates)
            {
                for (int d = 0; d < (int)Direction.End; d++)
                {
                    if (!roomChecker.TryGetEmptyNeighbor(parent.GridPosition, (Direction)d, out Vector2Int pos)) continue;

                    RoomData picked = roomChecker.PickWeightedRandom(mapSettings.normalRoomPool, generation);
                    if (picked == null) continue;

                    return PlaceRoom(picked, pos, generation);
                }
            }
            return null;
        }

        /// <summary>
        /// 실제 Grid위치에 방 생성
        /// </summary>
        /// <param name="data"></param>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        private Room PlaceRoom(RoomData data, Vector2Int gridPos, int generation)
        {
            Vector3 worldPos = new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0f);
            GameObject newRoomObject = Object.Instantiate(data.roomPrefab, worldPos, Quaternion.identity, mapRoot);

            Room room = newRoomObject.GetComponent<Room>();
            room.Init(data, gridPos, generation);

            if(data.layoutFile != null)
            {
                var layout = RoomLayout.Parse(data.layoutFile);
                newRoomObject.GetComponent<RoomBuilder>().Build(layout, room);
            }

            for (int x = 0; x < data.size.x; x++)
            {
                for (int y = 0; y < data.size.y; y++)
                {
                    placedRooms[gridPos + new Vector2Int(x, y)] = room;
                }
            }

            return room;
        }

        /// <summary>
        /// 보스 방 생성
        /// </summary>
        private void PlaceBossRoom(List<Room> lastGeneration, int generation)
        {
            foreach(Room room in lastGeneration)
            {
                for(int dir = 0; dir < (int)Direction.End; dir++)
                {
                    Vector2Int origin = room.GridPosition + DirectionUtil.ToOffset((Direction)dir);

                    if(roomChecker.CanPlace(origin, mapSettings.bossRoomData.size))
                    {
                        BossRoom = PlaceRoom(mapSettings.bossRoomData, origin, generation);
                        return;
                    }
                }
            }
            
            // 만약 보스 방을 생성하지 못한 경우. 모든 방에서 한 군대라도 검색해서 생성하도록 처리.
            foreach (Room room in GetAllRooms())
            {
                for (int dir = 0; dir < (int)Direction.End; dir++)
                {
                    Vector2Int origin = room.GridPosition + DirectionUtil.ToOffset((Direction)dir);

                    if (roomChecker.CanPlace(origin, mapSettings.bossRoomData.size))
                    {
                        bossRoom = PlaceRoom(mapSettings.bossRoomData, origin, generation);
                        return;
                    }
                }
            }
        }
    }
}