
using System.Numerics;
using Raylib_cs;

namespace EnemyReader_JSON
{
    internal class Explosion
    {
        public Vector2 Position;
        public float MaxRadius;
        public float Duration;
        public bool IsComplete;

        private float currentRadius;
        private float timer;

        public Explosion(Vector2 position, float maxRadius, float duration)
        {
            Position = position;
            MaxRadius = maxRadius;
            Duration = duration;

            currentRadius = 2.0f;
            timer = 0.0f;
            IsComplete = false;
        }

        public void Update()
        {
            timer += Raylib.GetFrameTime();

            // Calculate radius based on timer
            float progress = timer / Duration;
            if (progress < 0.5f)
            {
                // Expand
                currentRadius = 2.0f + (MaxRadius - 2.0f) * (progress * 2.0f);
            }
            else
            {
                // Fade out
                currentRadius = MaxRadius * (1.0f - ((progress - 0.5f) * 2.0f));
            }

            if (timer >= Duration)
            {
                IsComplete = true;
            }
        }

        public void Draw()
        {
            if (!IsComplete)
            {
                // Calculate opacity based on time
                int alpha = (int)(255 * (1.0f - (timer / Duration)));

                // Draw explosion circle
                Color explosionColor = new Color(255, 200, 50, alpha);
                Raylib.DrawCircle((int)Position.X, (int)Position.Y, currentRadius, explosionColor);

                // Draw inner circle
                Color innerColor = new Color(255, 100, 0, alpha);
                Raylib.DrawCircle((int)Position.X, (int)Position.Y, currentRadius * 0.6f, innerColor);
            }
        }

    }
}
