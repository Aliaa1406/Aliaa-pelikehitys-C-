using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Asteroid
    {
        private TransformComponent transform;
        private int size; 
        private float radius;
        public bool IsActive { get; private set; } = true;

        public Asteroid(Vector2 startPosition, Vector2 startVelocity, int asteroidSize, float rotationSpeed)
        {
            size = asteroidSize;
            radius = GetRadiusForSize(size);
            transform = new TransformComponent(startPosition, startVelocity, 0, rotationSpeed);
        }

        private float GetRadiusForSize(int size)
        {
            return size switch
            {
                1 => 25f,  // Small
                2 => 40f,  // Big
                
            };
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;
            transform.Update(deltaTime);
            transform.WrapAroundScreen(AsteroidsGame.GetScreenWidth(), AsteroidsGame.GetScreenHeight());
        }

        public void Draw()
        {
            if (!IsActive) return;

            // Check if textures are loaded by checking if any texture has a valid ID
            if (AreTexturesLoaded())
            {
                var texture = AsteroidsGame.GetAsteroidTexture(size);
                if (texture.Id != 0)
                {
                    DrawWithTexture(texture);
                    return;
                }
            }
            DrawAsPolygon();
        }

        // Helper method to check if textures are loaded
        private bool AreTexturesLoaded()
        {
            // Check if any of the asteroid textures are loaded
            return AsteroidsGame.ASTEROID_BROWNTexture.Id != 0 ||
                   AsteroidsGame.ASTEROID_GREYTexture.Id != 0;
        }

        private void DrawWithTexture(Texture2D texture)
        {
            Rectangle sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);
            float scale = (radius * 2) / texture.Width;
            Rectangle destRec = new Rectangle(
                transform.position.X,
                transform.position.Y,
                texture.Width * scale,
                texture.Height * scale
            );
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, transform.rotation, Color.White);
        }

        private void DrawAsPolygon()
        {
            // Draw as octagon
            int vertices = 8;
            float angleStep = 360.0f / vertices;
            for (int i = 0; i < vertices; i++)
            {
                float angle1 = transform.rotation + i * angleStep;
                float angle2 = transform.rotation + (i + 1) * angleStep;
                Vector2 p1 = transform.position + GetDirectionFromAngle(angle1) * radius;
                Vector2 p2 = transform.position + GetDirectionFromAngle(angle2) * radius;
                Raylib.DrawLineV(p1, p2, Color.White);
            }
        }

        private Vector2 GetDirectionFromAngle(float angleDegrees)
        {
            float radians = angleDegrees * (MathF.PI / 180.0f);
            return new Vector2(MathF.Cos(radians), MathF.Sin(radians));
        }

        public void Destroy()
        {
            IsActive = false;
        }

        public Vector2 GetPosition() => transform.position;
        public float GetRadius() => radius;
        public int GetSize() => size;
    }
}