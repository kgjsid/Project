using UnityEngine;

namespace World.Map
{
    [CreateAssetMenu(fileName = "NewRoomData", menuName = "Map/Room")]
    public class RoomData : ScriptableObject
    {
        public RoomType roomType;
        public GameObject roomPrefab;
        public Vector2Int size = Vector2Int.one;

        [Header("생성 가중치")]
        [Tooltip("같은 세대 안에서 이 방이 뽑힐 확률 가중치. 0이면 안 뽑힘")]
        public float weight = 1f;

        [Header("등장 가능 세대 범위")]
        [Tooltip("이 값보다 이른 세대에는 등장 안 함 (예: 2 ~ 4 --> 0, 1세대, 5 이상 세대 x)")]
        public int minGeneration = 0;
        public int maxGeneration = 99;

        public bool CanAppearAt(int generation)
        {
            return generation >= minGeneration && generation <= maxGeneration;
        }
    }
}