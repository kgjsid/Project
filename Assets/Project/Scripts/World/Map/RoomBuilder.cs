using UnityEngine;
using UnityEngine.Tilemaps;

using World;

namespace World.Map
{
    public class RoomBuilder : MonoBehaviour
    {
        [SerializeField] private Tilemap floorTilemap;
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private TileBase floorTile;
        [SerializeField] private TileBase wallTile;

        [SerializeField] private GameObject doorPrefab;      // 통과 가능 (콜라이더 없음)
        [SerializeField] private GameObject wallBlockPrefab; // 문 자리를 막는 벽 (콜라이더 있음)

        public void Build(RoomLayout layout, Room room)
        {
            for (int x = 0; x < layout.Width; x++)
            {
                for (int y = 0; y < layout.Height; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var glyph = layout.Tiles[x, y];

                    if (glyph == TileGlyph.Wall)
                    {
                        wallTilemap.SetTile(pos, wallTile);
                        continue;
                    }

                    floorTilemap.SetTile(pos, floorTile);

                    Vector3 worldPos = floorTilemap.GetCellCenterWorld(pos);

                    switch (glyph)
                    {
                        case TileGlyph.Door:
                            CreateDoorPair(worldPos, layout, x, y, room);
                            break;
                        case TileGlyph.PlayerSpawn:
                            CreateSpawnPoint(worldPos, SpawnPointType.Player);
                            break;
                        case TileGlyph.MonsterSpawn:
                            CreateSpawnPoint(worldPos, SpawnPointType.Enemy);
                            break;
                        case TileGlyph.LootSpawn:
                            CreateSpawnPoint(worldPos, SpawnPointType.LootBox);
                            break;
                    }
                }
            }
        }

        private void CreateDoorPair(Vector3 worldPos, RoomLayout layout, int x, int y, Room room)
        {
            Direction dir = DetectDirection(layout, x, y);

            GameObject door = Instantiate(doorPrefab, worldPos, Quaternion.identity, transform);
            GameObject wall = Instantiate(wallBlockPrefab, worldPos, Quaternion.identity, transform);

            room.RegisterDoorPair(dir, door, wall);
        }

        private Direction DetectDirection(RoomLayout layout, int x, int y)
        {
            if (x == 0) return Direction.Left;
            if (x == layout.Width - 1) return Direction.Right;
            if (y == layout.Height - 1) return Direction.Up;
            return Direction.Down;
        }

        private void CreateSpawnPoint(Vector3 worldPos, SpawnPointType type)
        {
            var obj = new GameObject($"Spawn_{type}");
            obj.transform.SetParent(transform);
            obj.transform.position = worldPos;
            obj.AddComponent<SpawnPoint>().type = type;
        }
    }
}