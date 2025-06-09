using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Enemy
    {
        public Vector2 position;
        public Vector2 direction;
        public Vector2 velocity;
        public float angle;
        private float rotationSpeed = 2.0f;
        private float acceleration = 3.0f;
        private float maxSpeed = 200.0f;
        private float currentSpeed = 0.0f;
        private float drag = 0.99f;
        private List<Bullet> bullets;
        private float shootCooldown = 1.5f;
        private float currentCooldown = 0;
        private Texture2D enemyTexture;
        private bool textureLoaded = false;

        private Sound shootSound;
        private bool soundLoaded = false;

        private bool isActive = true;
        public bool IsActive { get { return isActive; } }

        // AI behavior variables
        private float aiDecisionTime = 2.0f;
        private float aiTimer = 0f;
        private Random random;
        private Vector2 playerPosition;

        public Enemy(Vector2 initialPosition, string texturePath)
        {
            position = initialPosition;
            direction = new Vector2(0, 1);
            velocity = new Vector2(0, 0);
            angle = 180;
            bullets = new List<Bullet>();
            random = new Random();

            // Check if texture file exists
            if (System.IO.File.Exists(texturePath))
            {
                enemyTexture = Raylib.LoadTexture(texturePath);
                textureLoaded = true;
            }
            else
            {
                Console.WriteLine($"Error loading texture: file {texturePath} not found");
                textureLoaded = false;
            }

            string soundPath = @"C:\Tiedostot\Space Shooter\shooting-star-2-104073.mp3";
            if (System.IO.File.Exists(soundPath))
            {
                shootSound = Raylib.LoadSound(soundPath);
                soundLoaded = true;
            }
            else
            {
                Console.WriteLine($"Error: Shoot sound not found: {soundPath}");
            }
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight, Vector2 playerPos)
        {
            if (!isActive) return;

            // Store player position for targeting
            playerPosition = playerPos;

            // AI behavior timer
            aiTimer -= deltaTime;
            if (aiTimer <= 0)
            {
                MakeAIDecision();
                aiTimer = aiDecisionTime;
            }

            // Update velocity based on current speed and direction
            velocity = direction * currentSpeed * deltaTime;

            // Update position
            position.X += velocity.X;
            position.Y += velocity.Y;

            // Apply drag
            currentSpeed *= drag;

            // Handle screen boundaries
            WrapAroundScreen(screenWidth, screenHeight);

            // Update shooting cooldown
            if (currentCooldown > 0)
            {
                currentCooldown -= deltaTime;
            }

            // Randomly shoot at player when cooldown is ready
            if (currentCooldown <= 0 && random.Next(100) < 5)  // 5% chance to shoot each frame if cooldown ready
            {
                Shoot();
            }

            // Update bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime, screenWidth, screenHeight);

                // Remove bullets that are no longer active
                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
                }
            }
        }

        private void MakeAIDecision()
        {
            // Choose a behavior based on random chance
            int decision = random.Next(100);

            if (decision < 30)  // 30% chance to change direction randomly
            {
                angle = random.Next(360);
                UpdateDirection();
                currentSpeed = maxSpeed * 0.6f;
            }
            else if (decision < 70)  // 40% chance to move toward player
            {
                // Calculate angle to player
                float dx = playerPosition.X - position.X;
                float dy = playerPosition.Y - position.Y;
                angle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI) + 90;
                UpdateDirection();
                currentSpeed = maxSpeed * 0.8f;
            }
            else  // 30% chance to shoot and change direction slightly
            {
                Shoot();
                angle += random.Next(-45, 45);
                UpdateDirection();
                currentSpeed = maxSpeed * 0.4f;
            }
        }

        private void UpdateDirection()
        {
            // Update direction vector using Vector2.Transform instead of sin/cos
            direction = Vector2.Transform(Vector2.UnitY, Matrix3x2.CreateRotation(-angle * Raylib.DEG2RAD));
        }

        private void WrapAroundScreen(int screenWidth, int screenHeight)
        {
            // Handle screen boundaries wrap-around
            if (position.X < 0)
                position.X = screenWidth;
            else if (position.X > screenWidth)
                position.X = 0;

            if (position.Y < 0)
                position.Y = screenHeight;
            else if (position.Y > screenHeight)
                position.Y = 0;
        }

        private void Shoot()
        {
            if (!isActive) return;

            currentCooldown = shootCooldown;

            // Create a bullet with direction toward player
            Vector2 toPlayer = Vector2.Normalize(new Vector2(
                playerPosition.X - position.X,
                playerPosition.Y - position.Y
            ));

            bullets.Add(new Bullet(position, toPlayer));

            if (soundLoaded)
            {
                Raylib.PlaySound(shootSound);
            }
        }

        public void Draw()
        {
            if (!isActive) return;

            if (textureLoaded)
            {
                // Draw enemy using texture
                Rectangle sourceRec = new Rectangle(0, 0, enemyTexture.Width, enemyTexture.Height);

                // Destination rectangle
                Rectangle destRec = new Rectangle(position.X, position.Y, enemyTexture.Width, enemyTexture.Height);

                // Image center point (needed for rotation)
                Vector2 origin = new Vector2(enemyTexture.Width / 2, enemyTexture.Height / 2);

                // Draw texture with rotation
                Raylib.DrawTexturePro(
                    enemyTexture,
                    sourceRec,
                    destRec,
                    origin,
                    angle,
                    Color.White
                );
            }
            else
            {
                // If texture not loaded, draw a triangle as fallback using Vector2.Transform
                Vector2 forward = Vector2.Transform(Vector2.UnitY, Matrix3x2.CreateRotation(-angle * Raylib.DEG2RAD));
                Vector2 right = Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(-angle * Raylib.DEG2RAD));

                Vector2 p1 = position + forward * 20.0f;
                Vector2 p2 = position + (right * 15.0f - forward * 15.0f);
                Vector2 p3 = position + (-right * 15.0f - forward * 15.0f);

                Raylib.DrawTriangle(p1, p2, p3, Color.Yellow);
            }

            // Draw bullets
            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }
        }

        public void Reset(int screenWidth, int screenHeight)
        {
            // Reset enemy position to a random location
            Random random = new Random();
            position = new Vector2(random.Next(screenWidth), random.Next(screenHeight));

            // Stop movement
            velocity = Vector2.Zero;
            currentSpeed = 0;

            // Reset angle
            angle = random.Next(360);
            UpdateDirection();

            isActive = true;
        }

        public void Destroy()
        {
            isActive = false;
            bullets.Clear();
        }

        public void UnloadResources()
        {
            if (textureLoaded)
            {
                Raylib.UnloadTexture(enemyTexture);
                textureLoaded = false;
            }

            if (soundLoaded)
            {
                Raylib.UnloadSound(shootSound);
                soundLoaded = false;
            }
        }

        // Return bullets for collision checks
        public List<Bullet> GetBullets()
        {
            return bullets;
        }
    }
}