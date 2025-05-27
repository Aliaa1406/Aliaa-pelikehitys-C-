using System;

using System.Numerics;
using Raylib_cs;

namespace Pong
{
    class Paddle
    {

        public float X;
        public float Y;
        public float Width;
        public float Height;
        public int Speed;


        public Paddle(float x, float y, float width, float height, int speed, Color color)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;

        }

        public void Update(KeyboardKey upKey, KeyboardKey downKey, int screenHeight)
        {
            if (Raylib.IsKeyDown(upKey))
                Y -= Speed;
            if (Raylib.IsKeyDown(downKey))
                Y += Speed;

            // Screen boundaries
            if (Y < 0)
                Y = 0;
            if (Y + Height > screenHeight)
                Y = screenHeight - Height;
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)X, (int)Y, (int)Width, (int)Height, Color.DarkPurple);
        }

        public Rectangle GetRect()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }
}
