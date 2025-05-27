
using System.Numerics;
using Raylib_cs;

namespace EnemyReader_JSON
{
    internal class Artillery
    {
        public Vector2 Position;
        public Color Color;
        public int Width;
        public int Height;
        public int BarrelLength;
        public int BarrelWidth;

        public Artillery(Vector2 position, Color color, int width, int height, int barrelLength, int barrelWidth)
        {
            Position = position;
            Color = color;
            Width = width;
            Height = height;
            BarrelLength = barrelLength;
            BarrelWidth = barrelWidth;
        }

        public void Draw(float angle)
        {
            // Draw tank body
            Raylib.DrawRectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Height),
                Width,
                Height,
                Color
            );

            // Draw tank barrel
            float radians = (float)(angle * Math.PI / 180.0f);
            float endX = Position.X + (float)Math.Cos(radians) * BarrelLength;
            float endY = Position.Y - (float)Math.Sin(radians) * BarrelLength;

            Raylib.DrawLineEx(Position, new Vector2(endX, endY), BarrelWidth, Color);
        }
    }
}