using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class BossEnemy
    {
        public Vector2 position;
        public Vector2 direction;
        public Vector2 velocity;
        public float angle;
        private float rotationSpeed = 2.0f;
        private float speed = 100.0f;
        private List<Bullet> bullets;
        private float shootCooldown = 0.8f; // Boss shoots slower but more dangerous
        private float currentCooldown = 0;
        private Texture2D bossTexture;
        private bool textureLoaded = false;
        private Sound shootSound;
        private bool soundLoaded = false;

        // Boss specific properties
        public int health = 4; // Boss has 4 health points
        public int maxHealth = 4; // Maximum health for percentage calculation
        public bool isActive = true;
        private float scale = 1.0f; // Boss is 1x bigger than player
        private Vector2 targetPosition; // For movement patterns
        private float movementTimer = 0;
        private int movementPhase = 0;

        // Boss counter and timer properties
        public float bossTimer = 0f; // Time since boss spawned
        public int bossNumber = 1; // Which boss this is (1st, 2nd, etc.)
        public static int totalBossesSpawned = 0; // Track total bosses spawned
        public static int totalBossesDefeated = 0; // Track total bosses defeated
        private float nextPhaseTimer = 3.0f; // Time until next movement phase

        public BossEnemy(Vector2 initialPosition, string texturePath)
        {
            position = initialPosition;
            direction = new Vector2(-1, 0); // Initially moving left
            velocity = new Vector2(0, 0);
            angle = 90; // Face left initially
            bullets = new List<Bullet>();
            targetPosition = position;

            // Set boss number and increment counter
            totalBossesSpawned++;
            bossNumber = totalBossesSpawned;
            bossTimer = 0f;

            // Load texture
            if (System.IO.File.Exists(texturePath))
            {
                bossTexture = Raylib.LoadTexture(texturePath);
                textureLoaded = true;
                Console.WriteLine($"Boss #{bossNumber} texture loaded successfully: {texturePath}");
            }
            else
            {
                Console.WriteLine($"Error loading boss #{bossNumber} texture: file {texturePath} not found");
                textureLoaded = false;
            }

            // Load shoot sound
            string soundPath = @"C:\Tiedostot\Space Shooter\sounds\shoot.wav";
            if (System.IO.File.Exists(soundPath))
            {
                if (!Raylib.IsAudioDeviceReady())
                {
                    Console.WriteLine("Audio device not initialized, initializing now...");
                    Raylib.InitAudioDevice();
                }
                shootSound = Raylib.LoadSound(soundPath);
                soundLoaded = true;
                Console.WriteLine($"Boss #{bossNumber} shoot sound loaded successfully");
            }
            else
            {
                Console.WriteLine($"Error: Boss #{bossNumber} shoot sound not found: {soundPath}");
                soundLoaded = false;
            }
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight, Vector2 playerPosition)
        {
            if (!isActive) return;

            // Update boss timer
            bossTimer += deltaTime;
            movementTimer += deltaTime;
            nextPhaseTimer -= deltaTime;

            // Boss movement patterns
            UpdateMovementPattern(deltaTime, screenWidth, screenHeight, playerPosition);

            // Update position
            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;

            // Keep boss on screen with proper boundaries
            float bossRadius = (bossTexture.Width * scale) / 2;
            if (position.X < bossRadius) position.X = bossRadius;
            if (position.X > screenWidth - bossRadius) position.X = screenWidth - bossRadius;
            if (position.Y < bossRadius) position.Y = bossRadius;
            if (position.Y > screenHeight - bossRadius) position.Y = screenHeight - bossRadius;

            // Update shooting cooldown
            if (currentCooldown > 0)
            {
                currentCooldown -= deltaTime;
            }

            // Shoot at player
            if (currentCooldown <= 0)
            {
                ShootAtPlayer(playerPosition);
                currentCooldown = shootCooldown;
            }

            // Update bullets
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime, screenWidth, screenHeight);

                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
                }
            }

            // Check if boss is destroyed
            if (health <= 0)
            {
                isActive = false;
                totalBossesDefeated++;
                Console.WriteLine($"Boss #{bossNumber} destroyed! Total defeated: {totalBossesDefeated}");
            }
        }

        private void UpdateMovementPattern(float deltaTime, int screenWidth, int screenHeight, Vector2 playerPosition)
        {
            // Boss movement patterns - changes every 3 seconds
            if (nextPhaseTimer <= 0)
            {
                nextPhaseTimer = 3.0f;
                movementTimer = 0;
                movementPhase = (movementPhase + 1) % 4;

                switch (movementPhase)
                {
                    case 0: // Move towards player horizontally
                        targetPosition = new Vector2(playerPosition.X, position.Y);
                        break;
                    case 1: // Move to upper part of screen
                        targetPosition = new Vector2(position.X, screenHeight * 0.2f);
                        break;
                    case 2: // Move to lower part of screen
                        targetPosition = new Vector2(position.X, screenHeight * 0.8f);
                        break;
                    case 3: // Move towards player completely
                        targetPosition = new Vector2(playerPosition.X, playerPosition.Y + 100);
                        break;
                }
            }

            // Move towards target
            Vector2 directionToTarget = targetPosition - position;
            if (directionToTarget.Length() > 10)
            {
                directionToTarget = Vector2.Normalize(directionToTarget);
                velocity = directionToTarget * speed;

                // Use Vector2.Transform instead of Math.Atan2 for angle calculation
                // Calculate angle from direction vector - convert direction to angle in degrees
                angle = MathF.Atan2(directionToTarget.X, -directionToTarget.Y) * Raylib.RAD2DEG;
            }
            else
            {
                velocity = Vector2.Zero;
            }
        }

        private void ShootAtPlayer(Vector2 playerPosition)
        {
            Vector2 directionToPlayer = playerPosition - position;
            if (directionToPlayer.Length() > 0)
            {
                directionToPlayer = Vector2.Normalize(directionToPlayer);
                bullets.Add(new Bullet(position, directionToPlayer));

                if (soundLoaded)
                {
                    Raylib.PlaySound(shootSound);
                }
            }
        }

        // Method to create bullets in multiple directions (boss special attack)
        private void ShootMultipleDirections(int bulletCount = 8)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angleStep = 360.0f / bulletCount;
                float bulletAngle = i * angleStep;

                // Use Vector2.Transform to create direction from angle
                Vector2 bulletDirection = Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(bulletAngle * Raylib.DEG2RAD));
                bullets.Add(new Bullet(position, bulletDirection));
            }

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
                Rectangle sourceRec = new Rectangle(0, 0, bossTexture.Width, bossTexture.Height);
                Rectangle destRec = new Rectangle(
                    position.X,
                    position.Y,
                    bossTexture.Width * scale,
                    bossTexture.Height * scale
                );
                Vector2 origin = new Vector2(
                    (bossTexture.Width * scale) / 2,
                    (bossTexture.Height * scale) / 2
                );

                Raylib.DrawTexturePro(
                    bossTexture,
                    sourceRec,
                    destRec,
                    origin,
                    angle,
                    Color.White
                );
            }
            else
            {
                Raylib.DrawCircle((int)position.X, (int)position.Y, 40.0f, Color.Red);
                Raylib.DrawCircleLines((int)position.X, (int)position.Y, 40.0f, Color.White);
            }

            DrawHealthBar();

            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }
        }

        private void DrawHealthBar()
        {
            float barWidth = 100;
            float barHeight = 12;
            float healthPercentage = (float)health / (float)maxHealth;

            Vector2 barPosition = new Vector2(position.X - barWidth / 2, position.Y - 70);

            Raylib.DrawRectangle((int)barPosition.X, (int)barPosition.Y, (int)barWidth, (int)barHeight, Color.DarkGray);

            Color healthColor = Color.Red;
            if (healthPercentage > 0.6f)
                healthColor = Color.Green;
            else if (healthPercentage > 0.3f)
                healthColor = Color.Yellow;

            Raylib.DrawRectangle((int)barPosition.X, (int)barPosition.Y, (int)(barWidth * healthPercentage), (int)barHeight, healthColor);
            Raylib.DrawRectangleLines((int)barPosition.X, (int)barPosition.Y, (int)barWidth, (int)barHeight, Color.White);

            string healthText = $"{health}/{maxHealth}";
            Vector2 textPosition = new Vector2(position.X - 15, position.Y - 90);
            Raylib.DrawText(healthText, (int)textPosition.X, (int)textPosition.Y, 16, Color.White);
        }

        // Draw boss counter and timer display
        public void DrawBossInfo(int screenWidth, int screenHeight)
        {
            if (!isActive) return;

            // Position for boss info display (top-right corner)
            Vector2 infoPosition = new Vector2(screenWidth - 250, 20);

            // Background panel
            Raylib.DrawRectangle((int)infoPosition.X - 10, (int)infoPosition.Y - 10, 240, 120, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines((int)infoPosition.X - 10, (int)infoPosition.Y - 10, 240, 120, Color.Yellow);

            // Boss number and status
            string bossTitle = $"BOSS #{bossNumber}";
            Raylib.DrawText(bossTitle, (int)infoPosition.X, (int)infoPosition.Y, 20, Color.Red);

            // Boss timer
            int minutes = (int)(bossTimer / 60);
            int seconds = (int)(bossTimer % 60);
            string timerText = $"Time: {minutes:D2}:{seconds:D2}";
            Raylib.DrawText(timerText, (int)infoPosition.X, (int)infoPosition.Y + 25, 16, Color.White);

            // Next phase timer
            string phaseText = $"Next Phase: {nextPhaseTimer:F1}s";
            Raylib.DrawText(phaseText, (int)infoPosition.X, (int)infoPosition.Y + 45, 14, Color.Lime);

            // Boss statistics
            string statsText = $"Spawned: {totalBossesSpawned} | Defeated: {totalBossesDefeated}";
            Raylib.DrawText(statsText, (int)infoPosition.X, (int)infoPosition.Y + 65, 12, Color.LightGray);

            // Current movement phase
            string[] phaseNames = { "Track Player", "Move Up", "Move Down", "Approach Player" };
            string currentPhaseText = $"Phase: {phaseNames[movementPhase]}";
            Raylib.DrawText(currentPhaseText, (int)infoPosition.X, (int)infoPosition.Y + 85, 12, Color.Orange);
        }

        public void TakeDamage(int damage = 1)
        {
            health -= damage;
            if (health < 0) health = 0;

            Console.WriteLine($"Boss #{bossNumber} took {damage} damage! Health: {health}/{maxHealth}");

            if (health == 0)
            {
                Console.WriteLine($"Boss #{bossNumber} defeated!");
            }
        }

        public List<Bullet> GetBullets()
        {
            return bullets;
        }

        public void UnloadResources()
        {
            if (textureLoaded)
            {
                Raylib.UnloadTexture(bossTexture);
                textureLoaded = false;
                Console.WriteLine($"Boss #{bossNumber} texture unloaded");
            }

            if (soundLoaded)
            {
                Raylib.UnloadSound(shootSound);
                soundLoaded = false;
                Console.WriteLine($"Boss #{bossNumber} sound unloaded");
            }
        }

        public static BossEnemy SpawnFromRight(int screenWidth, int screenHeight, string texturePath)
        {
            Vector2 spawnPosition = new Vector2(screenWidth + 50, screenHeight / 2);
            BossEnemy boss = new BossEnemy(spawnPosition, texturePath);
            Console.WriteLine($"Boss #{boss.bossNumber} spawning at position: {spawnPosition}");
            return boss;
        }

        public static BossEnemy SpawnFromSide(int screenWidth, int screenHeight, string texturePath, int side = 0)
        {
            Vector2 spawnPosition;

            switch (side)
            {
                case 0: // Right
                    spawnPosition = new Vector2(screenWidth + 50, screenHeight / 2);
                    break;
                case 1: // Left
                    spawnPosition = new Vector2(-50, screenHeight / 2);
                    break;
                case 2: // Top
                    spawnPosition = new Vector2(screenWidth / 2, -50);
                    break;
                case 3: // Bottom
                    spawnPosition = new Vector2(screenWidth / 2, screenHeight + 50);
                    break;
                default:
                    spawnPosition = new Vector2(screenWidth + 50, screenHeight / 2);
                    break;
            }

            return new BossEnemy(spawnPosition, texturePath);
        }

        public bool IsOnScreen(int screenWidth, int screenHeight)
        {
            float radius = (bossTexture.Width * scale) / 2;
            return position.X > -radius && position.X < screenWidth + radius &&
                   position.Y > -radius && position.Y < screenHeight + radius;
        }

        // Reset static counters (useful for new game)
        public static void ResetCounters()
        {
            totalBossesSpawned = 0;
            totalBossesDefeated = 0;
        }
    }
}