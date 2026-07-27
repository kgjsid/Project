using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace World.Map
{
    public enum TileGlyph { Wall, Floor, Door, PlayerSpawn, MonsterSpawn, LootSpawn }

    public class RoomLayout
    {
        public TileGlyph[,] Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public static RoomLayout Parse(TextAsset roomLayout)
        {
            string[] lines = roomLayout.text.Replace("\r", "").Split('\n');
            List<string> valid = new List<string>();
            foreach(var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line)) valid.Add(line);
            }

            var layout = new RoomLayout();

            layout.Height = valid.Count;
            layout.Width = 0;

            foreach(var line in valid)
            {
                if (line.Length > layout.Width) layout.Width = line.Length;
            }

            layout.Tiles = new TileGlyph[layout.Width, layout.Height];

            for(int y = 0; y < layout.Height; y++)
            {
                string line = valid[layout.Height - 1 - y];

                for(int x = 0; x < layout.Width; x++)
                {
                    char c = x < line.Length ? line[x] : '#';
                    layout.Tiles[x, y] = ToGlyph(c);
                }
            }

            return layout;
        }

        private static TileGlyph ToGlyph(char c)
        {
            switch (c)
            {
                case '.': return TileGlyph.Floor;
                case 'D': return TileGlyph.Door;
                case 'P': return TileGlyph.PlayerSpawn;
                case 'M': return TileGlyph.MonsterSpawn;
                case 'L': return TileGlyph.LootSpawn;
                default: return TileGlyph.Wall;
            }
        }
    }
}