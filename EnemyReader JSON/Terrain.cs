
using Raylib_cs;

namespace EnemyReader_JSON
{
    internal class Terrain
    {
        private int[] heights;
        private int screenWidth;
        private int screenHeight;
        private Color terrainColor = new Color(52, 152, 144, 255);
        private int segmentWidth = 1; // Width of each terrain segment

        public Terrain(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
            heights = new int[screenWidth];
        }

        public void Generate()
        {
            // Generate random terrain
            Random random = new Random();
            int height = screenHeight - 100;

            for (int i = 0; i < screenWidth; i++)
            {
                // Create wavy terrain
                if (i > 0)
                {
                    int diff = random.Next(-2, 3);
                    height += diff;

                    // Ensure height is within acceptable range
                    height = Math.Max(screenHeight / 2, Math.Min(screenHeight - 50, height));
                }

                heights[i] = height;
            }
        }

        public int GetHeightAt(int x)
        {
            if (x >= 0 && x < screenWidth)
            {
                return heights[x];
            }
            return screenHeight;
        }

        public void CreateCrater(int x, int y, int radius)
        {
            // Create explosion effect on terrain using simple index-based approach
            for (int i = x - radius; i <= x + radius; i++)
            {
                if (i >= 0 && i < screenWidth)
                {
                    
                    int distanceFromCenter = Math.Abs(i - x);
                    int depthEffect = radius - distanceFromCenter;

                    // Apply the crater effect (raise the terrain)
                    heights[i] = Math.Min(screenHeight, heights[i] + depthEffect);
                }
            }
        }

        public void Draw()
        {
            for (int x = 0; x < screenWidth; x++)
            {
                Raylib.DrawLine(x, heights[x], x, screenHeight, terrainColor);
            }
        }

        // Helper method to get terrain segment index from x position
        public int GetTerrainIndexAtPosition(int xPosition)
        {
            // Division to get the index
            return xPosition / segmentWidth;
        }
    }
}