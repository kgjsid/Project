using UnityEngine;

namespace World.Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Transform[] doorAnchors = new Transform[4];

        private RoomData roomData;
        private Vector2Int gridPosition;

        public RoomData RoomData { get { return roomData; } set { roomData = value; } }
        public Vector2Int GridPosition { get { return gridPosition; } set { gridPosition = value; } }

        public void Init(RoomData roomData, Vector2Int gridPosition)
        {
            this.roomData = roomData;
            this.gridPosition = gridPosition;
        }

        public bool HasDoor(Direction dir)
        {
            return doorAnchors[(int)dir] != null;
        }

        public Transform GetDoorAnchor(Direction dir)
        {
            return doorAnchors[(int)dir];
        }
    }
}