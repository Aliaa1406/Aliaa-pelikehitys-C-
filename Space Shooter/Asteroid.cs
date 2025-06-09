using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Asteroid
    {
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float rotationSpeed;
        public float radius;
        public bool IsActive;
        public int Size;
        private Texture2D asteroidTexture;
        private bool textureLoaded = false;

        public Asteroid(Vector2 startPosition, Vector2 startVelocity, float startRadius, int size, string texturePath = null)
        {
            position = startPosition;
            velocity = startVelocity;
            radius = startRadius;
            Size = size;
            rotation = (float)new Random().NextDouble() * 360;
            rotationSpeed = (float)(new Random().NextDouble() * 100 - 50);
            IsActive = true;

            if (texturePath != null && System.IO.File.Exists(texturePath))
            {
                asteroidTexture = Raylib.LoadTexture(texturePath);
                textureLoaded = true;
            }
            else
            {
                textureLoaded = false;
            }
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            // Päivitä sijainti
            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;

            // Päivitä pyöriminen
            rotation += rotationSpeed * deltaTime;

            // Käsittele ruudun rajojen ylitys
            WrapAroundScreen(screenWidth, screenHeight);
        }

        private void WrapAroundScreen(int screenWidth, int screenHeight)
        {
            // Käsittele ruudun rajojen ylitys
            if (position.X < 0)
                position.X = screenWidth;
            else if (position.X > screenWidth)
                position.X = 0;

            if (position.Y < 0)
                position.Y = screenHeight;
            else if (position.Y > screenHeight)
                position.Y = 0;
        }

        public void Draw()
        {
            if (textureLoaded)
            {
                // Piirrä asteroidi käyttäen tekstuuria
                Rectangle sourceRec = new Rectangle(0, 0, asteroidTexture.Width, asteroidTexture.Height);

                // Scale texture based on asteroid size
                float scale = radius * 2 / asteroidTexture.Width;

                // Kohdesuorakulmio
                Rectangle destRec = new Rectangle(
                    position.X,
                    position.Y,
                    asteroidTexture.Width * scale,
                    asteroidTexture.Height * scale
                );

                // Kuvan keskipiste (tarvitaan kierrättämiseen)
                Vector2 origin = new Vector2(asteroidTexture.Width / 2, asteroidTexture.Height / 2);

                // Piirrä tekstuuri kierrettynä
                Raylib.DrawTexturePro(
                    asteroidTexture,
                    sourceRec,
                    destRec,
                    origin,
                    rotation,
                    Color.White
                );
            }
            else
            {
                // Piirrä asteroidi monikulmiona käyttäen Vector2.Transform sin/cos sijaan
                int vertices = 8;
                float angleStep = 360.0f / vertices;

                for (int i = 0; i < vertices; i++)
                {
                    float angle1 = rotation + i * angleStep;
                    float angle2 = rotation + (i + 1) * angleStep;

                    Vector2 dir1 = Vector2.Transform(Vector2.UnitY, Matrix3x2.CreateRotation(-angle1 * Raylib.DEG2RAD));
                    Vector2 dir2 = Vector2.Transform(Vector2.UnitY, Matrix3x2.CreateRotation(-angle2 * Raylib.DEG2RAD));

                    Vector2 p1 = position + dir1 * radius;
                    Vector2 p2 = position + dir2 * radius;

                    Raylib.DrawLineV(p1, p2, Color.White);
                }
            }
        }

        public void UnloadResources()
        {
            if (textureLoaded)
            {
                Raylib.UnloadTexture(asteroidTexture);
                textureLoaded = false;
            }
        }

        public void Destroy()
        {
            UnloadResources();
            IsActive = false;
        }

        // Tarkista törmäys ammusten kanssa
        public bool CheckCollision(Vector2 point)
        {
            float distance = Vector2.Distance(position, point);
            return distance <= radius;
        }
    }
}