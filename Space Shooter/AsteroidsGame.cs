using Raylib_cs;
using System.Numerics;

namespace Space_Shooter
{
    internal class AsteroidsGame
    {
        private static readonly Random random = new Random();

        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;

        // Textures
        public static Texture2D ShipTexture;
        public static Texture2D EnemyTexture;
        public static Texture2D AsteroidLargeTexture;
        public static Texture2D AsteroidMediumTexture;
        public static Texture2D AsteroidSmallTexture;
        public static Texture2D BulletTexture;
        public static Texture2D EnemyBulletTexture;

        // Sound System
        private SoundSystem soundSystem;

        // Game Timer
        private float gameTime = 0f;
        private const float UFO_SPAWN_TIME = 15f;
        private float lastUfoSpawn = 0f;

        private Ship player;
        private List<Asteroid> asteroids = new List<Asteroid>();
        private List<Enemy> enemies = new List<Enemy>();
        private List<Bullet> bullets = new List<Bullet>();
        private List<Bullet> enemyBullets = new List<Bullet>();

        private int playerLives;
        private int score;

        public AsteroidsGame()
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Asteroids Game");
            Raylib.InitAudioDevice();
            Raylib.SetTargetFPS(60);

            // Load textures
            ShipTexture = Raylib.LoadTexture("Image/playerShip3_green.png");
            EnemyTexture = Raylib.LoadTexture("Image/ufoYellow.png");
            AsteroidLargeTexture = Raylib.LoadTexture("Image/meteorBrown_big4.png");
            AsteroidMediumTexture = Raylib.LoadTexture("Image/meteorGrey_big4.png");
            AsteroidSmallTexture = Raylib.LoadTexture("Image/meteorGrey_small1.png");
            BulletTexture = Raylib.LoadTexture("Image/bullet.png");
            EnemyBulletTexture = Raylib.LoadTexture("Image/enemyBullet.png");

            // Initialize sound system
            soundSystem = new SoundSystem();

            // Load high scores
            HighScoreManager.Load();

            InitializeGame();
        }

        private void InitializeGame()
        {
            player = new Ship(new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), ShipTexture, soundSystem);
            asteroids.Clear();
            enemies.Clear();
            bullets.Clear();
            enemyBullets.Clear();

            playerLives = 3;
            score = 0;
            gameTime = 0f;
            lastUfoSpawn = 0f;

            // Create initial asteroids
            for (int i = 0; i < 5; i++)
                SpawnAsteroid(AsteroidSize.Large);
        }

        private void SpawnAsteroid(AsteroidSize size, Vector2? position = null)
        {
            Vector2 pos;
            if (position.HasValue)
            {
                pos = position.Value;
            }
            else
            {
                // Spawn away from player
                do
                {
                    pos = new Vector2(random.NextSingle() * SCREEN_WIDTH, random.NextSingle() * SCREEN_HEIGHT);
                } while (Vector2.Distance(pos, player.GetPosition()) < 100);
            }

            Vector2 vel = new Vector2(random.NextSingle() - 0.5f, random.NextSingle() - 0.5f);
            vel = Vector2.Normalize(vel) * (50 + random.NextSingle() * 50);

            float rotSpeed = (random.NextSingle() - 0.5f) * 2;

            asteroids.Add(new Asteroid(pos, vel, size, rotSpeed));
        }

        private void SpawnEnemy()
        {
            float x = random.NextSingle() * SCREEN_WIDTH;
            Vector2 pos = new(x, -50);
            enemies.Add(new Enemy(pos, EnemyTexture, player));
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();
                Update(deltaTime);
                Draw();
            }

            soundSystem.Unload();
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }

        private void Update(float deltaTime)
        {
            gameTime += deltaTime;

            player.Update(deltaTime);
            bullets = player.GetBullets();

            foreach (var a in asteroids.ToArray()) a.Update(deltaTime);
            foreach (var e in enemies.ToArray())
            {
                e.Update(deltaTime);
                enemyBullets.AddRange(e.GetBullets());
            }
            foreach (var b in bullets.ToArray()) b.Update(deltaTime);
            foreach (var eb in enemyBullets.ToArray()) eb.Update(deltaTime);

            // Spawn UFO every 15 seconds
            if (gameTime - lastUfoSpawn >= UFO_SPAWN_TIME)
            {
                SpawnEnemy();
                lastUfoSpawn = gameTime;
            }

            // Player bullets vs asteroids
            foreach (var bullet in bullets.ToArray())
            {
                foreach (var asteroid in asteroids.ToArray())
                {
                    if (bullet.IsActive && asteroid.IsActive)
                    {
                        if (Vector2.Distance(bullet.GetPosition(), asteroid.GetPosition()) < asteroid.GetRadius())
                        {
                            bullet.Destroy();
                            Vector2 asteroidPos = asteroid.GetPosition();
                            AsteroidSize currentSize = asteroid.GetSize();
                            asteroid.Destroy();
                            soundSystem.PlayExplosionSound();

                            // Add score based on asteroid size
                            switch (currentSize)
                            {
                                case AsteroidSize.Large: score += 20; break;
                                case AsteroidSize.Medium: score += 50; break;
                                case AsteroidSize.Small: score += 100; break;
                            }

                            // Split asteroid
                            if (currentSize != AsteroidSize.Small)
                            {
                                AsteroidSize nextSize = currentSize == AsteroidSize.Large ? AsteroidSize.Medium : AsteroidSize.Small;
                                for (int i = 0; i < 2; i++)
                                {
                                    SpawnAsteroid(nextSize, asteroidPos);
                                }
                            }
                        }
                    }
                }
            }

            // Player bullets vs enemies
            foreach (var bullet in bullets.ToArray())
            {
                foreach (var enemy in enemies.ToArray())
                {
                    if (bullet.IsActive && enemy.IsActive)
                    {
                        if (Vector2.Distance(bullet.GetPosition(), enemy.GetPosition()) < 25)
                        {
                            bullet.Destroy();
                            enemy.Destroy();
                            soundSystem.PlayExplosionSound();
                            score += 500;
                        }
                    }
                }
            }

            // Enemy bullets vs asteroids
            foreach (var bullet in enemyBullets.ToArray())
            {
                foreach (var asteroid in asteroids.ToArray())
                {
                    if (bullet.IsActive && asteroid.IsActive)
                    {
                        if (Vector2.Distance(bullet.GetPosition(), asteroid.GetPosition()) < asteroid.GetRadius())
                        {
                            bullet.Destroy();
                            Vector2 asteroidPos = asteroid.GetPosition();
                            AsteroidSize currentSize = asteroid.GetSize();
                            asteroid.Destroy();
                            soundSystem.PlayExplosionSound();

                            // Split asteroid
                            if (currentSize != AsteroidSize.Small)
                            {
                                AsteroidSize nextSize = currentSize == AsteroidSize.Large ? AsteroidSize.Medium : AsteroidSize.Small;
                                for (int i = 0; i < 2; i++)
                                {
                                    SpawnAsteroid(nextSize, asteroidPos);
                                }
                            }
                        }
                    }
                }
            }

            // Player vs asteroids
            foreach (var asteroid in asteroids.ToArray())
            {
                if (asteroid.IsActive && Vector2.Distance(player.GetPosition(), asteroid.GetPosition()) < asteroid.GetRadius() + 15)
                {
                    asteroid.Destroy();
                    soundSystem.PlayExplosionSound();
                    PlayerHit();
                }
            }

            // Player vs enemies
            foreach (var enemy in enemies.ToArray())
            {
                if (enemy.IsActive && Vector2.Distance(player.GetPosition(), enemy.GetPosition()) < 25)
                {
                    enemy.Destroy();
                    soundSystem.PlayExplosionSound();
                    PlayerHit();
                }
            }

            // Player vs enemy bullets
            foreach (var bullet in enemyBullets.ToArray())
            {
                if (bullet.IsActive && Vector2.Distance(player.GetPosition(), bullet.GetPosition()) < 20)
                {
                    bullet.Destroy();
                    soundSystem.PlayExplosionSound();
                    PlayerHit();
                }
            }

            // Clean up inactive objects
            asteroids.RemoveAll(a => !a.IsActive);
            enemies.RemoveAll(e => !e.IsActive);
            bullets.RemoveAll(b => !b.IsActive);
            enemyBullets.RemoveAll(b => !b.IsActive);

            // Spawn new asteroids when all are destroyed
            if (asteroids.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                    SpawnAsteroid(AsteroidSize.Large);
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            player.Draw();
            foreach (var a in asteroids) a.Draw();
            foreach (var e in enemies) e.Draw();
            foreach (var b in bullets) b.Draw();
            foreach (var eb in enemyBullets) eb.Draw();

            // UI
            Raylib.DrawText($"Score: {score}", 10, 10, 24, Color.White);
            Raylib.DrawText($"Lives: {playerLives}", 10, 40, 24, Color.White);

            float timeUntilNextUfo = UFO_SPAWN_TIME - (gameTime - lastUfoSpawn);
            if (timeUntilNextUfo > 0)
            {
              //  Raylib.DrawText($"Next UFO: {timeUntilNextUfo:F0}s", 10, 70, 20, Color.Yellow);
            }

            // Draw high scores panel
           // HighScoreManager.DrawHighScorePanel();

            Raylib.EndDrawing();
        }

        private void PlayerHit()
        {
            playerLives--;

            if (playerLives <= 0)
            {
                // Check for high score and restart immediately
                if (HighScoreManager.IsHighScore(score))
                {
                    HighScoreManager.AddScore(score);
                }

                InitializeGame();
                return;
            }

            player = new Ship(new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), ShipTexture, soundSystem);
        }
    }
}
