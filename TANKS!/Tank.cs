using System.Numerics;
using Raylib_cs;

namespace TANKS_
{
    internal class Tank
    {

        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        public Color Color { get; private set; }
        public Bullet Bullet { get; private set; }
        private float Speed = 2.0f;
        private float lastShootTime;
        private const float shootInterval = 1.0f;
        private const int tankWidth = 40;
        private const int tankHeight = 30;
        private Rectangle tankBody;
        private Vector2 lastPosition;

        public Tank(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
            Direction = new Vector2(1, 0);
            UpdateTankBody();
        }

        public void Update(KeyboardKey up, KeyboardKey down, KeyboardKey left, KeyboardKey right, KeyboardKey shoot)
        {
            lastPosition = Position;
            Vector2 newDirection = Vector2.Zero;
            Vector2 movement = Vector2.Zero;

            if (Raylib.IsKeyDown(up))
            {
                movement.Y -= Speed;
                newDirection = new Vector2(0, -1);
            }
            if (Raylib.IsKeyDown(down))
            {
                movement.Y += Speed;
                newDirection = new Vector2(0, 1);
            }
            if (Raylib.IsKeyDown(left))
            {
                movement.X -= Speed;
                newDirection = new Vector2(-1, 0);
            }
            if (Raylib.IsKeyDown(right))
            {
                movement.X += Speed;
                newDirection = new Vector2(1, 0);
            }

            if (movement != Vector2.Zero)
            {
                Position += movement;
                Direction = Vector2.Normalize(newDirection);
                UpdateTankBody();
            }

            if (Raylib.IsKeyPressed(shoot) &&
                Raylib.GetTime() - lastShootTime > shootInterval &&
                (Bullet == null || !Bullet.Active))
            {
                lastShootTime = (float)Raylib.GetTime();
                Vector2 bulletPos = Position + Direction * 20;
                Bullet = new Bullet(bulletPos, Direction);
            }

            Bullet?.Update();
        }

        public void Draw()
        {
            Raylib.DrawRectangleRec(tankBody, Color);

            Vector2 barrelStart = new Vector2(Position.X + tankWidth / 2, Position.Y + tankHeight / 2);
            Vector2 barrelEnd = barrelStart + Direction * 20;
            Raylib.DrawLineEx(barrelStart, barrelEnd, 4, Color.Green);

            Color trackColor = Color.Gray;
            Raylib.DrawRectangle((int)Position.X, (int)Position.Y, tankWidth, 5, trackColor);
            Raylib.DrawRectangle((int)Position.X, (int)(Position.Y + tankHeight - 5), tankWidth, 5, trackColor);

            Bullet?.Draw();
        }

        private void UpdateTankBody()
        {
            tankBody = new Rectangle(Position.X, Position.Y, tankWidth, tankHeight);
        }

        public void ClampPosition(int screenWidth, int screenHeight)
        {
            Position = new Vector2(
                Math.Clamp(Position.X, 0, screenWidth - tankWidth),
                Math.Clamp(Position.Y, 0, screenHeight - tankHeight)
            );
            UpdateTankBody();
        }

        public Rectangle GetBounds()
        {
            return tankBody;
        }

        public void RevertLastMove()
        {
            Position = lastPosition;
            UpdateTankBody();
        }
    }


}
