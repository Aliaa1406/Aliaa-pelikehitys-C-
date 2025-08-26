using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Asteroid
    {
        private TransformComponent transform;
        private int size;
        private float radius;

        // Constants for cleaner code
        private const float SMALL_RADIUS = 25f;
        private const float BIG_RADIUS = 40f;
        private const int POLYGON_VERTICES = 8;
        private const float DEGREES_TO_RADIANS = MathF.PI / 180.0f;

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
                1 => SMALL_RADIUS,  // Small asteroid
                2 => BIG_RADIUS,    // Big asteroid
                _ => SMALL_RADIUS   // Default fallback
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

            // Try to draw with texture first, fallback to polygon
            if (TryDrawWithTexture())
                return;

            DrawAsPolygon();
        }

        private bool TryDrawWithTexture()
        {
            // Check if textures are loaded and get the appropriate texture
            if (!AreTexturesLoaded())
                return false;

            var texture = AsteroidsGame.GetAsteroidTexture(size);
            if (texture.Id == 0)
                return false;

            DrawWithTexture(texture);
            return true;
        }

        private bool AreTexturesLoaded()
        {
            // Check if any of the asteroid textures are loaded
            return AsteroidsGame.ASTEROID_BROWNTexture.Id != 0 ||
                   AsteroidsGame.ASTEROID_GREYTexture.Id != 0;
        }

        private void DrawWithTexture(Texture2D texture)
        {
            Rectangle sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);

            // Calculate scale to fit the radius
            float scale = (radius * 2) / texture.Width;
            Rectangle destRec = new Rectangle(
                transform.position.X,
                transform.position.Y,
                texture.Width * scale,
                texture.Height * scale
            );

            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f); // Use float literal

            Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, transform.rotation, Color.White);
        }

        private void DrawAsPolygon()
        {
            // Draw as octagon using constants
            float angleStep = 360.0f / POLYGON_VERTICES;

            for (int i = 0; i < POLYGON_VERTICES; i++)
            {
                float angle1 = transform.rotation + i * angleStep;
                float angle2 = transform.rotation + (i + 1) * angleStep;

                // Calculate vertices using helper method
                Vector2 p1 = transform.position + GetDirectionFromAngle(angle1) * radius;
                Vector2 p2 = transform.position + GetDirectionFromAngle(angle2) * radius;

                Raylib.DrawLineV(p1, p2, Color.White);
            }
        }

        private Vector2 GetDirectionFromAngle(float angleDegrees)
        {
            // Convert degrees to radians using constant
            float radians = angleDegrees * DEGREES_TO_RADIANS;
            return new Vector2(MathF.Cos(radians), MathF.Sin(radians));
        }

        public void Destroy()
        {
            IsActive = false;
        }

        // Public accessors
        public Vector2 GetPosition() => transform.position;
        public float GetRadius() => radius;
        public int GetSize() => size;

        // Additional useful accessors
        public Vector2 GetVelocity() => transform.velocity;
        public float GetRotation() => transform.rotation;
    }
}