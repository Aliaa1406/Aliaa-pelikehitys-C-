using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
    internal class Map
    {
        public enum Tile : sbyte
        {
            Floor,
            Wall,
            Door,
            Monster,
            Item,
            Player,
            Stairs
        }
        public int width;
        public int height;
        public Tile[] Tiles;


        static void PlaceShopToMap(Map level, Shop shop, Random random)
        {
            // Find a suitable location for the shop (ideally near a wall but not blocked)
            bool placed = false;
            for (int attempts = 0; attempts < 100 && !placed; attempts++)
            {
                int x = random.Next(2, level.width - 2);
                int y = random.Next(2, level.height - 2);

                int ti = y * level.width + x;
                if (level.Tiles[ti] == Map.Tile.Floor)
                {
                    // Check if there's a wall nearby (for aesthetics)
                    bool wallNearby = false;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && nx < level.width && ny >= 0 && ny < level.height)
                            {
                                int nti = ny * level.width + nx;
                                if (level.Tiles[nti] == Map.Tile.Wall)
                                {
                                    wallNearby = true;
                                    break;
                                }
                            }
                        }
                        if (wallNearby) break;
                    }

                    if (wallNearby)
                    {
                        shop.position = new Vector2(x, y);
                        placed = true;
                    }
                }
            }
        }

    }
   
}