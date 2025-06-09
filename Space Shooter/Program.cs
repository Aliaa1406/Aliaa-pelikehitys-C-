using Raylib_cs;
using System.Numerics;

namespace Space_Shooter
{
    class Program
    {
        static void Main(string[] args)
        {
            const int screenWidth = 800;
            const int screenHeight = 600;

            // Audio initialization
            Raylib.InitAudioDevice();
            Console.WriteLine("Audio device initialized");

            // Load sound effects
            Sound shootSound = Raylib.LoadSound(@"C:\Tiedostot\Space Shooter\shooting-star-2-104073.mp3");
            Sound explosionSound = Raylib.LoadSound(@"C:\Tiedostot\Space Shooter\large-underwater-explosion-190270.mp3");

            // Load background music
            Music backgroundMusic = Raylib.LoadMusicStream(@"C:\Tiedostot\Space Shooter\space-sound-mid-109575.mp3");

            // Start playing background music
            Raylib.PlayMusicStream(backgroundMusic);

            int score = 0;
            int playerLives = 5;
            List<Asteroid> asteroids = new List<Asteroid>();
            Random random = new Random();

            // BOSS VARIABLES - Added here
            BossEnemy? boss = null;
            bool bossSpawned = false;
            float gameTimer = 0.0f;
            float bossSpawnTime = 30.0f; // 30  seconds
            int bossesDefeated = 0;
            float nextBossSpawnTime = 120.0f;

            // Game Over logic variables
            bool gameOver = false;
            float gameOverTimer = 3.0f;
            float currentGameOverTime = 0;

            // Invincibility timer
            float invincibilityTimer = 0f;
            float invincibilityDuration = 1.5f;

            Raylib.InitWindow(screenWidth, screenHeight, "Space Shooter");
            Raylib.SetTargetFPS(60);

            // Create player ship
            Ship player = new Ship(
                new Vector2(screenWidth / 2, screenHeight / 2),
                @"C:\Tiedostot\Space Shooter\playerShip3_green.png"
            );

            // Create initial asteroids
            for (int i = 0; i < 4; i++)
            {
                CreateAsteroid(asteroids, random, screenWidth, screenHeight, 3);
            }

            // Game loop
            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();

                // Update music stream each frame
                Raylib.UpdateMusicStream(backgroundMusic);

                if (!gameOver)
                {
                    // UPDATE GAME TIMER - Added for boss spawning
                    gameTimer += deltaTime;

                    // Update invincibility timer
                    if (invincibilityTimer > 0)
                    {
                        invincibilityTimer -= deltaTime;
                    }

                    // Update player
                    player.Update(deltaTime, screenWidth, screenHeight);

                    // Check for shooting and play sound
                    if (Raylib.IsKeyPressed(KeyboardKey.Space))
                    {
                        Raylib.PlaySound(shootSound);
                    }

                    // UPDATE BOSS SYSTEM - Added boss update logic
                    UpdateBoss(ref boss, ref bossSpawned, gameTimer, bossSpawnTime, ref bossesDefeated,
                              ref nextBossSpawnTime, screenWidth, screenHeight, player, deltaTime,
                              ref score, ref playerLives, ref invincibilityTimer, invincibilityDuration, explosionSound);

                    // Update asteroids
                    for (int i = asteroids.Count - 1; i >= 0; i--)
                    {
                        asteroids[i].Update(deltaTime, screenWidth, screenHeight);

                        // Remove inactive asteroids
                        if (!asteroids[i].IsActive)
                        {
                            asteroids.RemoveAt(i);
                        }
                    }

                    // Check bullet and asteroid collisions
                    CheckBulletAsteroidCollisions(player, asteroids, ref score, random, explosionSound);

                    // Check player and asteroid collisions only if not invincible
                    if (invincibilityTimer <= 0)
                    {
                        for (int i = asteroids.Count - 1; i >= 0; i--)
                        {
                            if (CheckShipAsteroidCollision(player, asteroids[i]))
                            {
                                // Asteroid and player collide
                                SplitAsteroid(asteroids, random, asteroids[i], ref score, explosionSound);

                                // Reduce life
                                playerLives--;
                                Console.WriteLine("Player lost a life! Remaining: " + playerLives);

                                // Reset player position
                                player.Reset(screenWidth, screenHeight);

                                // Set invincibility timer
                                invincibilityTimer = invincibilityDuration;

                                // If all lives lost, activate Game Over
                                if (playerLives <= 0)
                                {
                                    gameOver = true;
                                    currentGameOverTime = 0;
                                    Console.WriteLine("GAME OVER!");
                                }

                                break;
                            }
                        }
                    }

                    // Create new asteroids if needed
                    if (asteroids.Count < 5)
                    {
                        CreateAsteroid(asteroids, random, screenWidth, screenHeight, 3);
                    }
                }
                else // Game Over screen and timer
                {
                    currentGameOverTime += deltaTime;
                    if (currentGameOverTime >= gameOverTimer)
                    {
                        // Reset game after Game Over time
                        gameOver = false;
                        score = 0;
                        playerLives = 5;

                        // RESET BOSS VARIABLES - Added boss reset
                        if (boss != null)
                        {
                            boss.UnloadResources();
                            boss = null;
                        }
                        bossSpawned = false;
                        gameTimer = 0.0f;
                        bossesDefeated = 0;
                        nextBossSpawnTime = 120.0f;

                        Console.WriteLine("Game reset. Lives: " + playerLives);

                        // Clear asteroids and create new ones
                        foreach (var asteroid in asteroids)
                        {
                            asteroid.UnloadResources();
                        }
                        asteroids.Clear();
                        for (int i = 0; i < 4; i++)
                        {
                            CreateAsteroid(asteroids, random, screenWidth, screenHeight, 3);
                        }

                        // Reset player
                        player.Reset(screenWidth, screenHeight);
                    }
                }

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Draw player (flashing when invincible)
                if (!gameOver && (invincibilityTimer <= 0 || Math.Floor(invincibilityTimer * 10) % 2 == 0))
                {
                    player.Draw();
                }

                // DRAW BOSS - Added boss drawing
                if (boss != null && boss.isActive && !gameOver)
                {
                    boss.Draw();
                }

                // Draw asteroids
                foreach (var asteroid in asteroids)
                {
                    asteroid.Draw();
                }

                // DRAW BOSS UI - Added boss-related UI
                DrawBossUI(boss, bossSpawned, gameTimer, bossSpawnTime, nextBossSpawnTime,
                          bossesDefeated, screenWidth, screenHeight, player);

                // Draw score
                Raylib.DrawText($"Score: {score}", 10, 10, 20, Color.White);

                // Draw player lives
                Raylib.DrawText("Lives:", 10, 40, 20, Color.White);
                for (int i = 0; i < playerLives; i++)
                {
                    Raylib.DrawRectangle(80 + i * 30, 40, 20, 20, Color.Red);
                }

                // If Game Over, show text
                if (gameOver)
                {
                    // Center text
                    const string gameOverText = "GAME OVER";
                    int textWidth = Raylib.MeasureText(gameOverText, 40);
                    Raylib.DrawText(gameOverText, screenWidth / 2 - textWidth / 2, screenHeight / 2 - 20, 40, Color.Red);
                }
                else
                {
                    // Draw instruction text only when game is running
                    Raylib.DrawText("Arrow keys/WASD to move, Space to shoot",
                                    10, screenHeight - 30, 20, Color.White);
                }

                Raylib.EndDrawing();
            }

            // Free resources
            Raylib.UnloadSound(shootSound);
            Raylib.UnloadSound(explosionSound);
            Raylib.UnloadMusicStream(backgroundMusic);
            Raylib.CloseAudioDevice();

            // CLEANUP BOSS - Added boss cleanup
            if (boss != null)
            {
                boss.UnloadResources();
            }

            player.UnloadResources();
            foreach (var asteroid in asteroids)
            {
                asteroid.UnloadResources();
            }

            Raylib.CloseWindow();
        }

        // NEW METHOD - Boss update logic
        private static void UpdateBoss(ref BossEnemy? boss, ref bool bossSpawned, float gameTimer,
                                     float bossSpawnTime, ref int bossesDefeated, ref float nextBossSpawnTime,
                                     int screenWidth, int screenHeight, Ship player, float deltaTime,
                                     ref int score, ref int playerLives, ref float invincibilityTimer,
                                     float invincibilityDuration, Sound explosionSound)
        {
            // Spawn first boss after 1 minute
            if (!bossSpawned && gameTimer >= bossSpawnTime)
            {
                boss = BossEnemy.SpawnFromRight(
                    screenWidth,
                    screenHeight,
                    @"C:\Tiedostot\Space Shooter\ufoYellow.png"
                );
                bossSpawned = true;
                Console.WriteLine("Boss UFO spawned after 1 minute!");
            }

            // Spawn additional bosses every 2 minutes after first boss is defeated
            if (bossSpawned && (boss == null || !boss.isActive) && gameTimer >= nextBossSpawnTime)
            {
                boss = BossEnemy.SpawnFromSide(
                    screenWidth,
                    screenHeight,
                    @"C:\Tiedostot\Space Shooter\ufoYellow.png",
                    bossesDefeated % 4 // Spawn from different sides
                );
                nextBossSpawnTime = gameTimer + 120.0f; // Next boss in 2 minutes
                Console.WriteLine($"Boss UFO #{bossesDefeated + 1} spawned!");
            }

            // Update boss if it exists and is active
            if (boss != null && boss.isActive)
            {
                boss.Update(deltaTime, screenWidth, screenHeight, player.position);

                // Check collision between player bullets and boss
                List<Bullet> playerBullets = player.GetBullets();
                for (int i = playerBullets.Count - 1; i >= 0; i--)
                {
                    float distance = Vector2.Distance(playerBullets[i].position, boss.position);
                    if (distance < 60) // UFO boss collision radius
                    {
                        boss.TakeDamage(1);
                        playerBullets[i].IsActive = false; // Remove bullet
                        score += 50; // Score for hitting boss

                        if (boss.health <= 0)
                        {
                            Console.WriteLine($"Boss UFO #{bossesDefeated + 1} defeated!");
                            bossesDefeated++;
                            score += 1000; // Bonus score for defeating boss
                            Raylib.PlaySound(explosionSound); // Play explosion sound
                        }
                    }
                }

                // Check collision between boss bullets and player (only if not invincible)
                if (invincibilityTimer <= 0)
                {
                    List<Bullet> bossBullets = boss.GetBullets();
                    for (int i = bossBullets.Count - 1; i >= 0; i--)
                    {
                        float distance = Vector2.Distance(bossBullets[i].position, player.position);
                        if (distance < 25) // Player collision radius
                        {
                            // Handle player getting hit by boss
                            playerLives--;
                            bossBullets[i].IsActive = false;
                            Console.WriteLine("Player hit by boss UFO bullet! Lives remaining: " + playerLives);

                            // Reset player position and set invincibility
                            player.Reset(screenWidth, screenHeight);
                            invincibilityTimer = invincibilityDuration;

                            break; // Only one hit per frame
                        }
                    }

                    // Check direct collision between boss and player (ramming damage)
                    float bossPlayerDistance = Vector2.Distance(boss.position, player.position);
                    if (bossPlayerDistance < 50)
                    {
                        playerLives--;
                        Console.WriteLine("Player rammed by boss UFO! Lives remaining: " + playerLives);

                        // Reset player position and set invincibility
                        player.Reset(screenWidth, screenHeight);
                        invincibilityTimer = invincibilityDuration;
                    }
                }
            }

            // Clean up defeated boss
            if (boss != null && !boss.isActive)
            {
                boss.UnloadResources();
                boss = null;
            }
        }

        // NEW METHOD - Draw boss-related UI (radar and timers removed)
        private static void DrawBossUI(BossEnemy? boss, bool bossSpawned, float gameTimer,
                                     float bossSpawnTime, float nextBossSpawnTime, int bossesDefeated,
                                     int screenWidth, int screenHeight, Ship player)
        {
            // Draw boss warning if active
            if (boss != null && boss.isActive)
            {
                Raylib.DrawText("BOSS ENEMY ACTIVE!", 10, 70, 20, Color.Red);
            }

            // Draw boss statistics
            Raylib.DrawText($"Bosses Defeated: {bossesDefeated}", 10, screenHeight - 50, 16, Color.White);
        }

        private static bool CheckShipAsteroidCollision(Ship player, Asteroid asteroid)
        {
            // Simple distance-based collision check
            float distance = Vector2.Distance(player.position, asteroid.position);

            // Assume player collision radius is about 20 pixels
            float playerRadius = 20.0f;

            bool collision = distance < (playerRadius + asteroid.radius);
            if (collision)
            {
                Console.WriteLine($"Collision detected! Distance: {distance}, limit: {playerRadius + asteroid.radius}");
            }

            return collision;
        }

        private static void CreateAsteroid(List<Asteroid> asteroids, Random random, int screenWidth, int screenHeight, int size)
        {
            // Create asteroid outside screen
            Vector2 position;
            int side = random.Next(4); // 0 = top, 1 = right, 2 = bottom, 3 = left

            switch (side)
            {
                case 0:
                    position = new Vector2(random.Next(screenWidth), -50);
                    break;
                case 1:
                    position = new Vector2(screenWidth + 50, random.Next(screenHeight));
                    break;
                case 2:
                    position = new Vector2(random.Next(screenWidth), screenHeight + 50);
                    break;
                default:
                    position = new Vector2(-50, random.Next(screenHeight));
                    break;
            }

            // Set velocity and direction toward screen center
            Vector2 direction = new Vector2(
                screenWidth / 2 - position.X,
                screenHeight / 2 - position.Y
            );

            // Normalize direction
            float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            direction.X /= length;
            direction.Y /= length;

            float speed = 50 + (float)random.NextDouble() * 50;
            Vector2 velocity = new Vector2(
                direction.X * speed,
                direction.Y * speed
            );

            float radius = size == 3 ? 40 : (size == 2 ? 20 : 10);

            string texturePath;
            // Choose different asteroid textures in rotation
            if (size == 3)
            {
                // Choose either gray or brown big asteroid
                texturePath = random.Next(2) == 0
                    ? @"C:\Tiedostot\Space Shooter\meteorBrown_big4.png"
                    : @"C:\Tiedostot\Space Shooter\meteorGrey_big4.png";
            }
            else
            {
                texturePath = @"C:\Tiedostot\Space Shooter\meteorGrey_small1.png";
            }

            asteroids.Add(new Asteroid(position, velocity, radius, size, texturePath));
        }

        private static void SplitAsteroid(List<Asteroid> asteroids, Random random, Asteroid asteroid, ref int score, Sound explosionSound)
        {
            // Play explosion sound
            Raylib.PlaySound(explosionSound);

            if (asteroid.Size > 1)
            {
                int newSize = asteroid.Size - 1;

                // Create two new asteroids
                for (int i = 0; i < 2; i++)
                {
                    float angle = (float)random.NextDouble() * 2 * (float)Math.PI;
                    Vector2 newVelocity = new Vector2(
                        (float)Math.Sin(angle) * (70 + (float)random.NextDouble() * 30),
                        (float)Math.Cos(angle) * (70 + (float)random.NextDouble() * 30)
                    );

                    string texturePath = @"C:\Tiedostot\Space Shooter\meteorGrey_small1.png";

                    asteroids.Add(new Asteroid(asteroid.position, newVelocity, newSize == 2 ? 20 : 10, newSize, texturePath));
                }
            }

            // Add points
            score += asteroid.Size * 100;

            // Destroy original asteroid
            asteroid.Destroy();
        }

        private static void CheckBulletAsteroidCollisions(Ship player, List<Asteroid> asteroids, ref int score, Random random, Sound explosionSound)
        {
            List<Bullet> bullets = player.GetBullets();

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (asteroids[j].CheckCollision(bullets[i].position))
                    {
                        // Asteroid and bullet hit each other
                        SplitAsteroid(asteroids, random, asteroids[j], ref score, explosionSound);

                        // Remove bullet (mark as inactive)
                        bullets[i].IsActive = false;
                        break;
                    }
                }
            }
        }
    }
}