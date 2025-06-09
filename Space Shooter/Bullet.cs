
using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Bullet
    {
        public Vector2 position;
        public Vector2 direction;
        private float speed = 400.0f;
        private float lifeTime = 1.5f;
        private float currentLife;
        public bool IsActive;

        private Sound? destroySound;
        private bool soundPlayed = false;

        public Bullet(Vector2 startPosition, Vector2 bulletDirection)
        {
            position = startPosition;
            direction = bulletDirection;
            currentLife = lifeTime;
            IsActive = true;
           // destroySound = optionalDestroySound;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            // Päivitä sijainti
            position.X += direction.X * speed * deltaTime;
            position.Y += direction.Y * speed * deltaTime;

            // Käsittele ruudun rajojen ylitys
            WrapAroundScreen(screenWidth, screenHeight);

            // Päivitä elinaika
            currentLife -= deltaTime;
            if (currentLife <= 0)
            {
                IsActive = false;
            }

            if (destroySound != null && !soundPlayed)
            {
                Raylib.PlaySound((Sound)destroySound);
                soundPlayed = true;
            }
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
            // Piirrä ammus
            Raylib.DrawCircle((int)position.X, (int)position.Y, 3, Color.White);
        }
    }
}

