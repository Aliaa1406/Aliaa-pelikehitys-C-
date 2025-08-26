using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class TransformComponent
    {

        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float rotationSpeed;

        // Add angle property for compatibility
        public float angle;
     
        public TransformComponent(Vector2 startPos, Vector2 startVel = default, float startRot = 0, float rotSpeed = 0)
        {
            position = startPos;
            velocity = startVel;
            rotation = startRot;
            rotationSpeed = rotSpeed;
        }

        public void Update(float deltaTime)
        {
            position += velocity * deltaTime;
            rotation += rotationSpeed * deltaTime;
        }

        public void WrapAroundScreen(int screenWidth, int screenHeight)
        {
            if (position.X < 0) position.X = screenWidth;
            else if (position.X > screenWidth) position.X = 0;

            if (position.Y < 0) position.Y = screenHeight;
            else if (position.Y > screenHeight) position.Y = 0;
        }

        public Vector2 GetDirectionVector()
        {
            float radians = rotation * (MathF.PI / 180.0f);
            return new Vector2(MathF.Sin(radians), -MathF.Cos(radians));
        }
    }

   
}



