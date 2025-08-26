using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Enemy
    {
        private TransformComponent transform;
        private List<Bullet> bullets;
        private Random random;

        // Constants for cleaner code
        private const float SPEED = 150.0f;
        private const float ROTATION_SPEED = 100.0f;
        private const float SHOOT_COOLDOWN = 2.0f;
        private const float AI_DECISION_TIME = 3.0f;
        private const float ENEMY_RADIUS = 15.0f;
        private const float TRIANGLE_SIZE = 15.0f;
        private const float TRIANGLE_WIDTH = 12.0f;
        private const int RANDOM_MOVE_CHANCE = 30;
        private const int SHOOT_CHANCE = 10;

        private float currentCooldown = 0;
        private float aiTimer = 0;

        public bool IsActive { get; private set; } = true;

        public Enemy(Vector2 startPosition)
        {
            transform = new TransformComponent(startPosition, Vector2.Zero, 0, 0);
            bullets = new List<Bullet>();
            random = new Random();
            currentCooldown = SHOOT_COOLDOWN;
        }

        public void Update(float deltaTime, Vector2 playerPosition)
        {
            if (!IsActive) return;

            UpdateAI(deltaTime, playerPosition);
            UpdateMovement(deltaTime);
            UpdateShooting(deltaTime, playerPosition);
            UpdateBullets(deltaTime);
        }

        private void UpdateAI(float deltaTime, Vector2 playerPosition)
        {
            aiTimer += deltaTime;
            if (aiTimer >= AI_DECISION_TIME)
            {
                MakeAIDecision(playerPosition);
                aiTimer = 0;
            }
        }

        private void UpdateMovement(float deltaTime)
        {
            transform.Update(deltaTime);
            transform.WrapAroundScreen(AsteroidsGame.GetScreenWidth(), AsteroidsGame.GetScreenHeight());
        }

        private void UpdateShooting(float deltaTime, Vector2 playerPosition)
        {
            currentCooldown -= deltaTime;
            if (currentCooldown <= 0 && random.Next(100) < SHOOT_CHANCE)
            {
                ShootAtPlayer(playerPosition);
                currentCooldown = SHOOT_COOLDOWN;
            }
        }

        private void MakeAIDecision(Vector2 playerPosition)
        {
            // Calculate direction to player using Vector2 operations
            Vector2 toPlayer = playerPosition - transform.position;
            float distanceToPlayer = toPlayer.Length(); // Using Vector2.Length() instead of manual calculation

            if (distanceToPlayer > 0)
            {
                // Face the player - simplified angle calculation
                float targetAngle = MathF.Atan2(toPlayer.X, -toPlayer.Y) * (180.0f / MathF.PI);
                transform.rotation = targetAngle;

                // Move towards player with some randomness
                Vector2 direction = Vector2.Normalize(toPlayer); // Using Vector2.Normalize()

                if (random.Next(100) < RANDOM_MOVE_CHANCE) // Use constant instead of magic number
                {
                    // Create random direction vector
                    float randomAngle = (float)random.NextDouble() * MathF.PI * 2;
                    direction = new Vector2(MathF.Cos(randomAngle), MathF.Sin(randomAngle));
                }

                transform.velocity = direction * SPEED; // Use constant
            }
        }

        private void ShootAtPlayer(Vector2 playerPosition)
        {
            // Using Vector2.Normalize() for direction calculation
            Vector2 direction = Vector2.Normalize(playerPosition - transform.position);
            bullets.Add(new Bullet(transform.position, direction));
        }

        private void UpdateBullets(float deltaTime)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime);
                if (!bullets[i].IsActive)
                    bullets.RemoveAt(i);
            }
        }

        public void Draw()
        {
            if (!IsActive) return;

            var texture = AsteroidsGame.GetUfoTexture();
            if (texture.Id != 0)
            {
                DrawWithTexture(texture);
            }
            else
            {
                DrawAsTriangle();
            }

            // Draw all bullets
            foreach (var bullet in bullets)
                bullet.Draw();
        }

        private void DrawWithTexture(Texture2D texture)
        {
            Rectangle sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle destRec = new Rectangle(transform.position.X, transform.position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f); // Use float literal

            Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, transform.rotation, Color.White);
        }

        private void DrawAsTriangle()
        {
            Vector2 direction = transform.GetDirectionVector();
            Vector2 right = new Vector2(-direction.Y, direction.X); // Perpendicular vector

            // Calculate triangle points using constants
            Vector2 p1 = transform.position + direction * TRIANGLE_SIZE;
            Vector2 p2 = transform.position + (right * TRIANGLE_WIDTH - direction * TRIANGLE_WIDTH);
            Vector2 p3 = transform.position + (-right * TRIANGLE_WIDTH - direction * TRIANGLE_WIDTH);

            Raylib.DrawTriangle(p1, p2, p3, Color.Red);
        }

        public void Destroy()
        {
            IsActive = false;
        }

        // Public accessors with cleaner implementation
        public Vector2 GetPosition() => transform.position;
        public List<Bullet> GetBullets() => bullets;
        public float GetRadius() => ENEMY_RADIUS; // Use constant instead of magic number
    }
}