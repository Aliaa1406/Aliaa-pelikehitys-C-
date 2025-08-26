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
        private float speed = 150.0f;
        private float rotationSpeed = 100.0f;
        private float shootCooldown = 2.0f;
        private float currentCooldown = 0;
        private Random random;
        private float aiTimer = 0;
        private float aiDecisionTime = 3.0f;
        public bool IsActive { get; private set; } = true;

        public Enemy(Vector2 startPosition)
        {
            transform = new TransformComponent(startPosition, Vector2.Zero, 0, 0);
            bullets = new List<Bullet>();
            random = new Random();
            currentCooldown = shootCooldown;
        }

        public void Update(float deltaTime, Vector2 playerPosition)
        {
            if (!IsActive) return;

            aiTimer += deltaTime;
            if (aiTimer >= aiDecisionTime)
            {
                MakeAIDecision(playerPosition);
                aiTimer = 0;
            }

            transform.Update(deltaTime);
            transform.WrapAroundScreen(AsteroidsGame.GetScreenWidth(), AsteroidsGame.GetScreenHeight());

            // Update shooting
            currentCooldown -= deltaTime;
            if (currentCooldown <= 0 && random.Next(100) < 10) 
            {
                ShootAtPlayer(playerPosition);
                currentCooldown = shootCooldown;
            }

            UpdateBullets(deltaTime);
        }

        private void MakeAIDecision(Vector2 playerPosition)
        {
            // Calculate direction to player
            Vector2 toPlayer = playerPosition - transform.position;
            float distanceToPlayer = toPlayer.Length();

            if (distanceToPlayer > 0)
            {
                // Face the player
                float targetAngle = MathF.Atan2(toPlayer.X, -toPlayer.Y) * (180.0f / MathF.PI);
                transform.rotation = targetAngle;

                // Move towards player with some randomness
                Vector2 direction = Vector2.Normalize(toPlayer);
                if (random.Next(100) < 30) // 30% chance to move randomly
                {
                    float randomAngle = (float)random.NextDouble() * MathF.PI * 2;
                    direction = new Vector2(MathF.Cos(randomAngle), MathF.Sin(randomAngle));
                }

                transform.velocity = direction * speed;
            }
        }

        private void ShootAtPlayer(Vector2 playerPosition)
        {
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

            foreach (var bullet in bullets)
                bullet.Draw();
        }

        private void DrawWithTexture(Texture2D texture)
        {
            Rectangle sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle destRec = new Rectangle(transform.position.X, transform.position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, transform.rotation, Color.White);
        }

        private void DrawAsTriangle()
        {
            Vector2 direction = transform.GetDirectionVector();
            Vector2 right = new Vector2(-direction.Y, direction.X);

            Vector2 p1 = transform.position + direction * 15.0f;
            Vector2 p2 = transform.position + (right * 12.0f - direction * 12.0f);
            Vector2 p3 = transform.position + (-right * 12.0f - direction * 12.0f);

            Raylib.DrawTriangle(p1, p2, p3, Color.Red);
        }

        public void Destroy()
        {
            IsActive = false;
        }

        public Vector2 GetPosition() => transform.position;
        public List<Bullet> GetBullets() => bullets;
        public float GetRadius() => 15.0f;
    }
}