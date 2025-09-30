using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    public enum AsteroidSize
    {
        Small = 0,
        Medium = 1,
        Large = 2
    }

    internal class TransformComponent
    {
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float rotationSpeed;

        public TransformComponent(Vector2 pos, Vector2 vel = default, float rot = 0, float rotSpeed = 0)
        {
            position = pos;
            velocity = vel;
            rotation = rot;
            rotationSpeed = rotSpeed;
        }

        public void Update(float deltaTime)
        {
            position += velocity * deltaTime;
            rotation += rotationSpeed * deltaTime;
            WrapAroundScreen();
        }

        private void WrapAroundScreen()
        {
            if (position.X < 0) position.X += AsteroidsGame.SCREEN_WIDTH;
            if (position.X > AsteroidsGame.SCREEN_WIDTH) position.X -= AsteroidsGame.SCREEN_WIDTH;
            if (position.Y < 0) position.Y += AsteroidsGame.SCREEN_HEIGHT;
            if (position.Y > AsteroidsGame.SCREEN_HEIGHT) position.Y -= AsteroidsGame.SCREEN_HEIGHT;
        }

        public Vector2 GetDirectionVector()
        {
            float rad = (rotation - 90) * Raylib.DEG2RAD;
            return new Vector2(MathF.Cos(rad), MathF.Sin(rad));
        }
    }
}