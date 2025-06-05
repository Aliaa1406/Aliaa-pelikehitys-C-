
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
            Vector2 turretDir = Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(-angle * Raylib.DEG2RAD));
            Raylib.DrawLineEx(Position, Position + turretDir * BarrelLength, BarrelWidth, Color);
        }
    }
}