using UnityEngine;

namespace World.Map
{
    /// <summary>
    /// Left : 0, Right : 1, Up : 2, Down : 3
    /// </summary>
    public enum Direction { Left, Right, Up, Down, End}

    public static class DirectionUtil
    {
        public static Direction GetOppositeDir(Direction dir)
        {
            switch(dir)
            {
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                default: return dir;
            }
        }

        public static Vector2Int ToOffset(Direction dir)
        {
            switch(dir)
            {
                case Direction.Left: return Vector2Int.left;
                case Direction.Right: return Vector2Int.right;
                case Direction.Up: return Vector2Int.up;
                case Direction.Down: return Vector2Int.down;
                default: return Vector2Int.zero;
            }
        }
    }
}