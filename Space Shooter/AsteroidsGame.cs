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
        private const float BULLET_RADIUS = 3.0f;

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
        private Random random;

        // Game state - simplified
        private int score = 0;
        private int playerLives = 3;
        private bool gameRunning = true;
        private float gameTimer = 0f;
        private float enemySpawnTimer = 0f;

        // Screen center for easy access
        private static readonly Vector2 ScreenCenter = new Vector2(SCREEN_WIDTH / 2f, SCREEN_HEIGHT / 2f);

        public AsteroidsGame()
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Asteroids Game");
            Raylib.SetTargetFPS(60);
            Raylib.InitAudioDevice();

            random = new Random();

            // Load all assets using the centralized system
            LoadAssets();

            // Start background music
            if (Raylib.IsMusicValid(BackgroundMusic))
                Raylib.PlayMusicStream(BackgroundMusic);

            InitializeGame();
        }

        private void InitializeGame()
        {
            score = 0;
            playerLives = 3;
            gameTimer = 0f;
            enemySpawnTimer = 0f;

            // Create player at screen center
            player = new Ship(ScreenCenter);

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

        // MAIN UPDATE LOOP - Simplified
        private void Update(float deltaTime)
        {
            // Update background music
            if (Raylib.IsMusicValid(BackgroundMusic))
                Raylib.UpdateMusicStream(BackgroundMusic);

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

        // MAIN DRAWING LOOP - Simplified
        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Always draw the game
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
           // Raylib.DrawText("Lives:", 10, 40, 20, Color.White);
            //for (int i = 0; i < playerLives; i++)
            //{
            //    Raylib.DrawRectangle(80 + i * 30, 40, 20, 20, Color.Red);
            //}

            Raylib.EndDrawing();
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

        // طريقة مبسطة لاختيار موقع عشوائي للظهور
        private Vector2 GetRandomSpawnPosition()
        {
            // اختيار جانب عشوائي من الشاشة (أعلى، يمين، أسفل، يسار)
            int side = random.Next(4);

            Vector2 spawnPosition;

            if (side == 0) // أعلى الشاشة
            {
                spawnPosition = new Vector2(random.Next(SCREEN_WIDTH), -50);
            }
            else if (side == 1) // يمين الشاشة
            {
                spawnPosition = new Vector2(SCREEN_WIDTH + 50, random.Next(SCREEN_HEIGHT));
            }
            else if (side == 2) // أسفل الشاشة
            {
                spawnPosition = new Vector2(random.Next(SCREEN_WIDTH), SCREEN_HEIGHT + 50);
            }
            else // يسار الشاشة
            {
                spawnPosition = new Vector2(-50, random.Next(SCREEN_HEIGHT));
            }

            return spawnPosition;
        }

        // طريقة مبسطة لحساب الاتجاه نحو وسط الشاشة
        private Vector2 GetDirectionToCenter(Vector2 fromPosition)
        {
            // حساب الفرق بين الموقع الحالي ووسط الشاشة
            Vector2 direction = ScreenCenter - fromPosition;

            // تطبيع الاتجاه (جعل طوله = 1) باستخدام دالة جاهزة
            return Vector2.Normalize(direction);
        }

        // طريقة مبسطة لتقسيم الكويكبات - بدون استخدام Sin/Cos معقد
        private void SplitAsteroid(Asteroid asteroid)
        {
            score += asteroid.GetSize() * 100;

            // Play explosion sound
            if (Raylib.IsSoundValid(ExplosionSound))
                Raylib.PlaySound(ExplosionSound);

            // فقط الكويكبات الكبيرة (حجم 2) تنقسم إلى صغيرة (حجم 1)
            if (asteroid.GetSize() == 2)
            {
                // إنشاء كويكبين صغيرين بطريقة مبسطة
                Vector2 currentPos = asteroid.GetPosition();

                // الكويكب الأول: يتحرك يميناً وأعلى
                Vector2 direction1 = new Vector2(1, -0.5f); // يمين + قليل أعلى
                direction1 = Vector2.Normalize(direction1);
                Vector2 velocity1 = direction1 * (70 + (float)random.NextDouble() * 30);

                // الكويكب الثاني: يتحرك يساراً وأسفل
                Vector2 direction2 = new Vector2(-1, 0.5f); // يسار + قليل أسفل
                direction2 = Vector2.Normalize(direction2);
                Vector2 velocity2 = direction2 * (70 + (float)random.NextDouble() * 30);

                // سرعة دوران عشوائية
                float rotationSpeed1 = (float)(random.NextDouble() * 200 - 100);
                float rotationSpeed2 = (float)(random.NextDouble() * 200 - 100);

                asteroids.Add(new Asteroid(currentPos, velocity1, 1, rotationSpeed1));
                asteroids.Add(new Asteroid(currentPos, velocity2, 1, rotationSpeed2));
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
                    // Using Raylib.CheckCollisionCircles instead of manual distance calculation
                    if (Raylib.CheckCollisionCircles(bullets[i].GetPosition(), BULLET_RADIUS,
                                                   enemies[j].GetPosition(), enemies[j].GetRadius()))
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
                    // Using consistent collision detection
                    if (Raylib.CheckCollisionCircles(playerPos, PLAYER_RADIUS,
                                                   enemyBullets[i].GetPosition(), BULLET_RADIUS))
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
            Console.WriteLine($"Player hit! Lives remaining: {playerLives}");

            // If no lives left, restart the game immediately
            if (playerLives <= 0)
            {
                Console.WriteLine($"Game Over! Final Score: {score}");
                InitializeGame(); // Restart immediately without any menu
                return;
            }

            // Only reset player position if still has lives
            player = new Ship(ScreenCenter);
        }

        private void CheckBulletAsteroidCollisions()
        {
            List<Bullet> bullets = player.GetBullets();

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    // Using Raylib.CheckCollisionCircles consistently
                    if (Raylib.CheckCollisionCircles(bullets[i].GetPosition(), BULLET_RADIUS,
                                                   asteroids[j].GetPosition(), asteroids[j].GetRadius()))
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
                _ => ASTEROID_BROWNTexture
            };
        }

        public static Texture2D GetUfoTexture() => UFOShipTexture;
        public static Sound GetLaserSound() => shootSound;
        public static Sound GetExplosionSound() => ExplosionSound;
        public static Music GetBackgroundMusic() => BackgroundMusic;

        public static float GetPlayerRadius() => PLAYER_RADIUS;
        public static int GetScreenWidth() => SCREEN_WIDTH;
        public static int GetScreenHeight() => SCREEN_HEIGHT;
        public static Vector2 GetScreenCenter() => ScreenCenter;

        // دوال مساعدة مبسطة لأي استخدام مستقبلي
        public static Vector2 CreateDirectionFromAngle(float angleDegrees)
        {
            // تحويل الزاوية من درجات إلى راديان
            float angleRadians = angleDegrees * MathF.PI / 180f;

            // إنشاء اتجاه باستخدام الدوال المثلثية البسيطة
            return new Vector2(MathF.Cos(angleRadians), MathF.Sin(angleRadians));
        }

        public static float GetAngleBetweenPoints(Vector2 from, Vector2 to)
        {
            Vector2 direction = to - from;
            float angleRadians = MathF.Atan2(direction.Y, direction.X);
            float angleDegrees = angleRadians * 180f / MathF.PI;

            // التأكد من أن الزاوية بين 0 و 360
            if (angleDegrees < 0) angleDegrees += 360;

            return angleDegrees;
        }
    }
}