using System.Numerics;

using Raylib_cs;

namespace Pong
{
    class Ball
    {
        public Vector2 Position;
        public Vector2 Speed;
        public float Radius = 20;
        private static Random random = new Random();
        public Ball(float x, float y, float speedX, float speedY, float radius)
        {
            Position = new Vector2(x, y);
            Speed = new Vector2(speedX, speedY);
            Radius = radius;
        }

        public void Draw()
        {
            Raylib.DrawCircleV(Position, Radius, Color.White);
        }

        public void Update(int screenWidth, int screenHeight)
        {
            Position.X += Speed.X;
            Position.Y += Speed.Y;

            //if (Position.Y + Radius >= Raylib.GetScreenHeight() || Position.Y - Radius <= 0) ;
            //{
            //    Speed.Y *= -1;
            //}
            //if(Position.X +Radius >= Raylib.GetScreenWidth()    || Position.X - Radius <= 0)
            //{
            //    Speed.X *= -1;
            //}
            if (Position.Y < Radius)
            {
                Position.Y = Radius;
                Speed.Y *= -1;
            }
            else if (Position.Y > screenHeight - Radius)
            {
                Position.Y = screenHeight - Radius;
                Speed.Y *= -1;
            }
        }
       

        public Rectangle GetRect()
        {
            return new Rectangle(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        }

        public void ReflectFromPaddle(Paddle paddle)
        {
            // Reverse X direction
            Speed.X *= -1;

            // Adjust Y speed based on where ball hit the paddle for more interesting gameplay
            float hitPosition = (Position.Y - paddle.Y) / paddle.Height;
            Speed.Y = (hitPosition - 0.5f) * 10; // -5 to 5 based on hit position

            // Ensure ball doesn't get stuck in paddle
            if (Speed.X > 0)
            {
                // Ball is moving right
                Position.X = paddle.X + paddle.Width + Radius;
            }
            else
            {
                // Ball is moving left
                Position.X = paddle.X - Radius;
            }
        }

        public void Reset(int screenWidth, int screenHeight, float speedX)
        {
            Position = new Vector2(screenWidth / 2, screenHeight / 2);
            Speed = new Vector2(speedX, Raylib.GetRandomValue(-4, 4));
        }
    }
}

