using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class AsteroidsGame
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 600;
        private const int INITIAL_ASTEROID_COUNT = 4;
        private const float PLAYER_RADIUS = 20.0f;
        private const float ENEMY_SPAWN_INTERVAL = 15.0f;



        // Textures
        public static Texture2D PlayerShipTexture;
        public static Texture2D UFOShipTexture;
        public static Texture2D ASTEROID_BROWNTexture;
        public static Texture2D ASTEROID_GREYTexture;


        // Sounds
        public static Sound shootSound;
        public static Sound ExplosionSound;
        public static Music BackgroundMusic;


        public static void LoadAssets()
        {

            PlayerShipTexture = Raylib.LoadTexture("C:\\Tiedostot\\Space Shooter\\Image\\playerShip3_green.png");
            UFOShipTexture = Raylib.LoadTexture("C:\\Tiedostot\\Space Shooter\\Image\\ufoYellow.png");
            ASTEROID_BROWNTexture = Raylib.LoadTexture("C:\\Tiedostot\\Space Shooter\\Image\\meteorBrown_big4.png");
            ASTEROID_GREYTexture = Raylib.LoadTexture("C:\\Tiedostot\\Space Shooter\\Image\\meteorGrey_big4.png");



            shootSound = Raylib.LoadSound("C:\\Tiedostot\\Space Shooter\\Image\\shooting-star-2-104073.mp3");
            ExplosionSound = Raylib.LoadSound("C:\\Tiedostot\\Space Shooter\\Image\\large-underwater-explosion-190270.mp3");
            BackgroundMusic = Raylib.LoadMusicStream("C:\\Tiedostot\\Space Shooter\\Image\\space-sound-mid-109575.mp3");
        }


        public static void UnloadAssets()
        {
            // Unload textures
            if (PlayerShipTexture.Id != 0) Raylib.UnloadTexture(PlayerShipTexture);
            if (UFOShipTexture.Id != 0) Raylib.UnloadTexture(UFOShipTexture);
            if (ASTEROID_BROWNTexture.Id != 0) Raylib.UnloadTexture(ASTEROID_BROWNTexture);
            if (ASTEROID_GREYTexture.Id != 0) Raylib.UnloadTexture(ASTEROID_GREYTexture);


            // Unload sounds
            if (Raylib.IsSoundValid(shootSound)) Raylib.UnloadSound(shootSound);
            if (Raylib.IsSoundValid(ExplosionSound)) Raylib.UnloadSound(ExplosionSound);
            if (Raylib.IsMusicValid(BackgroundMusic)) Raylib.UnloadMusicStream(BackgroundMusic);
        }



        // Game objects
        private Ship player;
        private List<Asteroid> asteroids;
        private List<Enemy> enemies;
        private HighScoreManager highScoreManager;
        private Random random;

        // Game state enum
        public enum GameState
        {
            Playing,
            GameOver,
            HighScoreEntry,
            ShowingHighScores
        }

        // Game state
        private GameState currentGameState = GameState.Playing;
        private int score = 0;
        private int playerLives = 3;
        private bool gameRunning = true;
        private float gameTimer = 0f;
        private float enemySpawnTimer = 0f;
        private float gameOverTimer = 0f;
        private const float GAME_OVER_DISPLAY_TIME = 2.0f;

        public AsteroidsGame()
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Asteroids Game");
            Raylib.SetTargetFPS(60);
            Raylib.InitAudioDevice();

            random = new Random();
            highScoreManager = new HighScoreManager();

            // Load all assets using the centralized system
            LoadAssets();

            // Start background music
            if (Raylib.IsMusicValid(BackgroundMusic))
                Raylib.PlayMusicStream(BackgroundMusic);

            InitializeGame();
        }

        private void InitializeGame()
        {
            currentGameState = GameState.Playing;
            score = 0;
            playerLives = 3;
            gameTimer = 0f;
            enemySpawnTimer = 0f;
            gameOverTimer = 0f;

            // Create player
            player = new Ship(new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2));

            // Create asteroids
            asteroids = new List<Asteroid>();
            CreateInitialAsteroids();

            // Clear enemies
            enemies = new List<Enemy>();
        }

        private void CreateInitialAsteroids()
        {
            asteroids.Clear();
            for (int i = 0; i < INITIAL_ASTEROID_COUNT; i++)
            {
                CreateAsteroid(2); // Size 2 = big (becomes small when split)
            }
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose() && gameRunning)
            {
                float deltaTime = Raylib.GetFrameTime();
                Update(deltaTime);
                Draw();
            }

            CleanupResources();
        }


        // MAIN UPDATE LOOP
        private void Update(float deltaTime)
        {
            // Update background music
            if (Raylib.IsMusicValid(BackgroundMusic))
                Raylib.UpdateMusicStream(BackgroundMusic);

            // Handle different game states
            switch (currentGameState)
            {
                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;
                case GameState.GameOver:
                    UpdateGameOver(deltaTime);
                    break;
                case GameState.HighScoreEntry:
                    UpdateHighScoreEntry();
                    break;
                case GameState.ShowingHighScores:
                    UpdateShowingHighScores();
                    break;
            }
        }

        private void UpdatePlaying(float deltaTime)
        {
            // Update game timer
            gameTimer += deltaTime;

            // Update player movement and shooting
            player.Update(deltaTime);

            // Play shooting sound when player shoots
            if (Raylib.IsKeyPressed(KeyboardKey.Space) && Raylib.IsSoundValid(shootSound))
            {
                Raylib.PlaySound(shootSound);
            }

            // Update all asteroids
            UpdateAsteroids(deltaTime);

            // Update all enemies
            UpdateEnemies(deltaTime);

            // Check all collision types
            CheckBulletAsteroidCollisions();
            CheckBulletEnemyCollisions();
            CheckPlayerAsteroidCollisions();
            CheckPlayerEnemyCollisions();
            CheckEnemyBulletPlayerCollisions();

            // Spawn new asteroids if screen is clear
            if (asteroids.Count == 0)
            {
                CreateInitialAsteroids();
            }
        }

        private void UpdateGameOver(float deltaTime)
        {
            gameOverTimer += deltaTime;

            // Auto restart after 2 seconds
            if (gameOverTimer >= 2.0f)
            {
                InitializeGame();
            }
        }

        private void UpdateHighScoreEntry()
        {
            // Just restart game instead
            InitializeGame();
        }

        private void UpdateShowingHighScores()
        {
            // Just restart game instead
            InitializeGame();
        }

        private void UpdateAsteroids(float deltaTime)
        {
            for (int i = asteroids.Count - 1; i >= 0; i--)
            {
                asteroids[i].Update(deltaTime);

                if (!asteroids[i].IsActive)
                {
                    asteroids.RemoveAt(i);
                }
            }
        }

        private void CheckPlayerAsteroidCollisions()
        {
            Vector2 playerPos = player.GetPosition();

            foreach (var asteroid in asteroids)
            {
                if (Raylib.CheckCollisionCircles(playerPos, PLAYER_RADIUS, asteroid.GetPosition(), asteroid.GetRadius()))
                {
                    PlayerHit();
                    return;
                }
            }
        }

        private void GameOver()
        {
            currentGameState = GameState.GameOver;
            gameOverTimer = 0f;
            Console.WriteLine($"Game Over! Final Score: {score}");
        }

        // MAIN DRAWING LOOP

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Draw different screens based on game state
            switch (currentGameState)
            {
                case GameState.Playing:
                    DrawPlaying();
                    break;
                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            Raylib.EndDrawing();
        }

        private void DrawPlaying()
        {
            player.Draw();

            foreach (var asteroid in asteroids)
            {
                asteroid.Draw();
            }

            foreach (var enemy in enemies)
            {
                enemy.Draw();
            }

            // Draw game UI
            Raylib.DrawText($"Score: {score}", 10, 10, 20, Color.White);

            // Draw player lives as red rectangles
            Raylib.DrawText("Lives:", 10, 40, 20, Color.White);
            for (int i = 0; i < playerLives; i++)
            {
                Raylib.DrawRectangle(80 + i * 30, 40, 20, 20, Color.Red);
            }
        }

        private void DrawGameOver()
        {
            // Draw game in background (dimmed)
            DrawPlaying();

            // Semi-transparent black overlay
            Raylib.DrawRectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, new Color(0, 0, 0, 150));

            // Show "GAME OVER" text
            string gameOverText = "GAME OVER";
            int gameOverWidth = Raylib.MeasureText(gameOverText, 60);
            Raylib.DrawText(gameOverText, SCREEN_WIDTH / 2 - gameOverWidth / 2, SCREEN_HEIGHT / 2 - 30, 60, Color.Red);

            // Show final score
            string finalScoreText = $"Final Score: {score}";
            int scoreWidth = Raylib.MeasureText(finalScoreText, 30);
            Raylib.DrawText(finalScoreText, SCREEN_WIDTH / 2 - scoreWidth / 2, SCREEN_HEIGHT / 2 + 30, 30, Color.White);
        }

        private void CreateAsteroid(int size)
        {
            Vector2 position = GetRandomSpawnPosition();
            Vector2 direction = GetDirectionToCenter(position);
            float speed = 50 + (float)random.NextDouble() * 50;
            Vector2 velocity = direction * speed;
            float rotationSpeed = (float)(random.NextDouble() * 200 - 100);

            asteroids.Add(new Asteroid(position, velocity, size, rotationSpeed));
        }

        private Vector2 GetRandomSpawnPosition()
        {
            int side = random.Next(4);
            return side switch
            {
                0 => new Vector2(random.Next(SCREEN_WIDTH), -50), // Top
                1 => new Vector2(SCREEN_WIDTH + 50, random.Next(SCREEN_HEIGHT)), // Right
                2 => new Vector2(random.Next(SCREEN_WIDTH), SCREEN_HEIGHT + 50), // Bottom
                _ => new Vector2(-50, random.Next(SCREEN_HEIGHT)) // Left
            };
        }

        private Vector2 GetDirectionToCenter(Vector2 fromPosition)
        {
            Vector2 direction = new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2) - fromPosition;
            return Vector2.Normalize(direction);
        }

        private void SplitAsteroid(Asteroid asteroid)
        {
            score += asteroid.GetSize() * 100;

            // Play explosion sound
            if (Raylib.IsSoundValid(ExplosionSound))
                Raylib.PlaySound(ExplosionSound);

            // Only big asteroids (size 2) split into small asteroids (size 1)
            if (asteroid.GetSize() == 2)
            {
                // Create two small asteroids
                for (int i = 0; i < 2; i++)
                {
                    float angle = (float)random.NextDouble() * MathF.PI * 2;
                    Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    Vector2 velocity = direction * (70 + (float)random.NextDouble() * 30);
                    float rotationSpeed = (float)(random.NextDouble() * 200 - 100);

                    asteroids.Add(new Asteroid(asteroid.GetPosition(), velocity, 1, rotationSpeed)); // size 1 = small
                }
            }

            asteroid.Destroy();
        }

        private void CreateEnemy()
        {
            Vector2 position = GetRandomSpawnPosition();
            enemies.Add(new Enemy(position));
        }

        private void UpdateEnemies(float deltaTime)
        {
            // Update enemy spawn timer
            enemySpawnTimer += deltaTime;

            // Spawn enemy every 15 seconds
            if (enemySpawnTimer >= ENEMY_SPAWN_INTERVAL)
            {
                CreateEnemy();
                enemySpawnTimer = 0f; // Reset timer
                Console.WriteLine("Enemy spawned!");
            }

            // Update existing enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(deltaTime, player.GetPosition());

                if (!enemies[i].IsActive)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        private void CheckBulletEnemyCollisions()
        {
            List<Bullet> bullets = player.GetBullets();

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    if (Raylib.CheckCollisionCircles(bullets[i].GetPosition(), 3, enemies[j].GetPosition(), enemies[j].GetRadius()))
                    {
                        score += 500; // Enemy is worth more points

                        // Play explosion sound
                        if (Raylib.IsSoundValid(ExplosionSound))
                            Raylib.PlaySound(ExplosionSound);

                        enemies[j].Destroy();
                        bullets[i].Destroy();
                        break;
                    }
                }
            }
        }

        private void CheckPlayerEnemyCollisions()
        {
            Vector2 playerPos = player.GetPosition();

            foreach (var enemy in enemies)
            {
                if (Raylib.CheckCollisionCircles(playerPos, PLAYER_RADIUS, enemy.GetPosition(), enemy.GetRadius()))
                {
                    PlayerHit();
                    return;
                }
            }
        }

        private void CheckEnemyBulletPlayerCollisions()
        {
            Vector2 playerPos = player.GetPosition();

            foreach (var enemy in enemies)
            {
                var enemyBullets = enemy.GetBullets();
                for (int i = enemyBullets.Count - 1; i >= 0; i--)
                {
                    if (Raylib.CheckCollisionCircles(playerPos, PLAYER_RADIUS, enemyBullets[i].GetPosition(), 3))
                    {
                        enemyBullets[i].Destroy();
                        PlayerHit();
                        return;
                    }
                }
            }
        }

        private void PlayerHit()
        {
            playerLives--;

            // Reset player position to center
            player = new Ship(new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2));

            Console.WriteLine($"Player hit! Lives remaining: {playerLives}");

            // If no lives left, game over
            if (playerLives <= 0)
            {
                GameOver();
            }
        }

        private void CheckBulletAsteroidCollisions()
        {
            List<Bullet> bullets = player.GetBullets();

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (Raylib.CheckCollisionCircles(bullets[i].GetPosition(), 3, asteroids[j].GetPosition(), asteroids[j].GetRadius()))
                    {
                        SplitAsteroid(asteroids[j]);
                        bullets[i].Destroy();
                        break;
                    }
                }
            }
        }

        private void CleanupResources()
        {
            Console.WriteLine("Cleaning up resources...");

            // Unload all assets using centralized system
            UnloadAssets();

            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
        // PUBLIC ACCESSORS FOR OTHER CLASSES

        public static Texture2D GetPlayerTexture() => PlayerShipTexture;

        public static Texture2D GetAsteroidTexture(int size)
        {
            return size switch
            {
                2 => ASTEROID_BROWNTexture,    
                1 => ASTEROID_GREYTexture, 

            };
        }

        public static Texture2D GetUfoTexture() => UFOShipTexture;
        public static Sound GetLaserSound() => shootSound;
        public static Sound GetExplosionSound() => ExplosionSound;
        public static Music GetBackgroundMusic() => BackgroundMusic;

        public static float GetPlayerRadius() => PLAYER_RADIUS;
        public static int GetScreenWidth() => SCREEN_WIDTH;
        public static int GetScreenHeight() => SCREEN_HEIGHT;
    }
}