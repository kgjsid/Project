using System.Collections.Generic;
using UnityEngine;

namespace World.Map
{
    public class RoomChecker
    {
        // placedRooms --> 방 리스트
        // placedRooms[0,0] -> 시작방. placedRooms[0,1] -> 몬스터방
        private Dictionary<Vector2Int, Room> placedRooms;

        public RoomChecker(Dictionary<Vector2Int, Room> placedRooms)
        {
            this.placedRooms = placedRooms;
        }

        /// <summary>
        /// 해당 좌표에 방이 비어있는지
        /// </summary>
        /// <param name="cell">위치</param>
        /// <returns>있다면 false</returns>
        public bool IsCellEmpty(Vector2Int cell)
        {
            return !placedRooms.ContainsKey(cell);
        }

        public bool CanPlace(Vector2Int origin, Vector2Int size)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (placedRooms.ContainsKey(origin + new Vector2Int(x, y)))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 비어 있는 위치 반환
        /// (ex. (0, 0) 위치 Up 방향 비어있는지 -> 비어 있다면 (0, 0+Up) 반환
        /// </summary>
        public bool TryGetEmptyNeighbor(Vector2Int from, Direction dir, out Vector2Int result)
        {
            result = from + DirectionUtil.ToOffset(dir);
            return IsCellEmpty(result);
        }

        public RoomData PickWeightedRandom(List<RoomData> candidates, int generation)
        {
            // 1. 후보지 중 해당 세대에 생성될 수 있는 방인지 검사
            List<RoomData> valid = candidates.FindAll(room => room.CanAppearAt(generation));
            if (valid.Count == 0) return null;

            // 2. 가중치 총합 계산
            float totalWeight = 0f;
            foreach (var room in valid)
            {
                totalWeight += room.weight;
            }

            // 3. 랜덤 기반 가중치 계산
            // 0 ~ total중 난수 하나를 뽑고 구간별 계산
            float rand = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach(var room in valid)
            {
                cumulative += room.weight;
                if(rand <= cumulative)
                {
                    return room;
                }
            }

            return valid[valid.Count - 1];
        }
    }
}