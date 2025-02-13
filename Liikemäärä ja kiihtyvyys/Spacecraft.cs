using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Liikemäärä_ja_kiihtyvyys
{
    internal class Spacecraft
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public bool ThrustersOn { get; private set; }
        private float fuelLevel = 100.0f;
        private const float THRUST_FORCE = 200.0f;
        private const float FUEL_BURN_RATE = 10.0f;

        public Spacecraft(Vector2 startPosition)
        {
            Position = startPosition;
            Velocity = Vector2.Zero;
        }

        public void Update(float deltaTime)
        {
            ThrustersOn = Raylib.IsKeyDown(KeyboardKey.Up) && fuelLevel > 0;

            if (ThrustersOn)
            {
                Velocity -= new Vector2(0, THRUST_FORCE * deltaTime);
                fuelLevel -= FUEL_BURN_RATE * deltaTime;
            }

            Position += Velocity * deltaTime;
        }

        public void Draw()
        {
            // Draw spacecraft
            Vector2 top = Position + new Vector2(0, -20);
            Vector2 left = Position + new Vector2(-15, 10);
            Vector2 right = Position + new Vector2(15, 10);
            Raylib.DrawTriangle(top, left, right, Color.Blue);

            // Draw flame if thrusters are active
            if (ThrustersOn && fuelLevel > 0)
            {
                float flameBaseY = Position.Y + 15;
                float flamePointY = Position.Y + 30;

                Raylib.DrawTriangle(
                    new Vector2(Position.X, flamePointY),
                    new Vector2(Position.X + 8, flameBaseY),
                    new Vector2(Position.X - 8, flameBaseY),
                    Color.Orange
                );
            }

            DrawHUD();
        }

        private void DrawHUD()
        {
            Raylib.DrawRectangle(10, 10, (int)(fuelLevel * 2), 20, Color.Green);
            string speedText = $"Speed: {Math.Abs(Velocity.Y):F1} m/s";
            string fuelText = $"Fuel: {Math.Max(fuelLevel, 0):F1}%";
            Raylib.DrawText(speedText, 10, 40, 20, Color.Lime);
            Raylib.DrawText(fuelText, 10, 70, 20, Color.Lime);
        }
    }
}
