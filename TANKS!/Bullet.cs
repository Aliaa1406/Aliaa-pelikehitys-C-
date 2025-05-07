using System;
using System.Numerics;
using Raylib_cs;

namespace TANKS_
{
    public class Bullet
    {
        public Vector2 Position;
        public Vector2 Direction;
        public bool Active = true;
        public float Speed = 300.0f; // Nostettiin ammuksen nopeutta
        private Rectangle bulletRect;
        private const int bulletSize = 8;

        public Bullet(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;
            UpdateBulletRect();
        }

        public void Update()
        {
            if (!Active) return;

            // Käytä GetFrameTime()-funktiota siirtymän normalisointiin
            float deltaTime = Raylib.GetFrameTime();
            Position += Direction * Speed * deltaTime;
            UpdateBulletRect();
        }

        private void UpdateBulletRect()
        {
            bulletRect = new Rectangle(Position.X - bulletSize / 2, Position.Y - bulletSize / 2, bulletSize, bulletSize);
        }

        public void Draw()
        {
            if (!Active) return;
            Raylib.DrawRectangleRec(bulletRect, Color.Red);
        }

        public bool CheckCollision(Rectangle other)
        {
            return Active && Raylib.CheckCollisionRecs(bulletRect, other);
        }

        public void Deactivate()
        {
            Active = false;
        }
    }
}