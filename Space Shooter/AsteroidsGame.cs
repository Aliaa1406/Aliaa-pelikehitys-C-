
using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class AsteroidsGame
    {
        private int screenWidth = 800;
        private int screenHeight = 600;
        private Ship player;
        private List<Asteroid> asteroids;
        private Random random;
        private int score = 0;
        private Music gameMusic;
        private Sound explosionSound;
        private Sound shootSound;
        private bool soundsLoaded = false;

        public AsteroidsGame()
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Asteroids Game");
            Raylib.SetTargetFPS(60);

            random = new Random();
            player = new Ship(new Vector2(screenWidth / 2, screenHeight / 2), "playerShip3_green.png");
            asteroids = new List<Asteroid>();

            // Luo alkuasteroidit
            for (int i = 0; i < 4; i++)
            {
                CreateAsteroid(3);
            }

            // Alusta äänet
            InitializeAudio();
        }

        private void InitializeAudio()
        {
            try
            {
                Raylib.InitAudioDevice();

                // Tarkista, onko äänitiedostot olemassa ennen latausta
                if (System.IO.File.Exists("resources/game_music.mp3"))
                    gameMusic = Raylib.LoadMusicStream("resources/game_music.mp3");

                if (System.IO.File.Exists("resources/explosion.wav"))
                    explosionSound = Raylib.LoadSound("resources/explosion.wav");

                if (System.IO.File.Exists("resources/shoot.wav"))
                    shootSound = Raylib.LoadSound("resources/shoot.wav");

                soundsLoaded = true;

                if (gameMusic.FrameCount > 0)
                    Raylib.PlayMusicStream(gameMusic);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Virhe äänien latauksessa: {e.Message}");
                soundsLoaded = false;
            }
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();

                // Päivitä musiikki
                if (soundsLoaded && gameMusic.FrameCount > 0)
                    Raylib.UpdateMusicStream(gameMusic);

                // Päivitä pelaaja
                player.Update(deltaTime, screenWidth, screenHeight);

                // Käsittele ampuminen
                if (Raylib.IsKeyPressed(KeyboardKey.Space) && soundsLoaded)
                {
                    if (shootSound.FrameCount > 0)
                        Raylib.PlaySound(shootSound);
                }

                // Päivitä asteroidit
                for (int i = asteroids.Count - 1; i >= 0; i--)
                {
                    asteroids[i].Update(deltaTime, screenWidth, screenHeight);

                    // Poista ei-aktiiviset asteroidit
                    if (!asteroids[i].IsActive)
                    {
                        asteroids.RemoveAt(i);
                    }
                }

                // Tarkista ammukset ja asteroidit törmäykset
                CheckBulletAsteroidCollisions();

                // Piirrä
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Piirrä pelaaja
                player.Draw();

                // Piirrä asteroidit
                foreach (var asteroid in asteroids)
                {
                    asteroid.Draw();
                }

                // Piirrä pisteet
                Raylib.DrawText($"Score: {score}", 10, 10, 20, Color.White);

                // Piirrä ohjeet
                Raylib.DrawText("Controls: Arrows/WASD to move, Space to shoot", 10, screenHeight - 30, 20, Color.White);

                Raylib.EndDrawing();

                // Luo uusia asteroideja tarvittaessa
                if (asteroids.Count < 5)
                {
                    CreateAsteroid(3);
                }
            }

            // Siivoa resurssit
            CleanupResources();
        }

        private void CleanupResources()
        {
            // Vapauta ääniresurssit
            if (soundsLoaded)
            {
                if (gameMusic.FrameCount > 0)
                    Raylib.UnloadMusicStream(gameMusic);

                if (explosionSound.FrameCount > 0)
                    Raylib.UnloadSound(explosionSound);

                if (shootSound.FrameCount > 0)
                    Raylib.UnloadSound(shootSound);

                Raylib.CloseAudioDevice();
            }

            // Vapauta pelaajan resurssit
            player.UnloadResources();

            // Vapauta asteroidien resurssit
            foreach (var asteroid in asteroids)
            {
                asteroid.UnloadResources();
            }

            Raylib.CloseWindow();
        }

        private void CreateAsteroid(int size, string texturePath = null)
        {
            // Asteroid luodaan ruudun ulkopuolelle
            Vector2 position;
            int side = random.Next(4); // 0 = ylä, 1 = oikea, 2 = ala, 3 = vasen

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

            // Aseta nopeus ja suunta kohti ruudun keskustaa
            Vector2 direction = new Vector2(
                screenWidth / 2 - position.X,
                screenHeight / 2 - position.Y
            );

            // Normalisoi suunta
            float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            direction.X /= length;
            direction.Y /= length;

            float speed = 50 + (float)random.NextDouble() * 50;
            Vector2 velocity = new Vector2(
                direction.X * speed,
                direction.Y * speed
            );

            float radius = size == 3 ? 40 : (size == 2 ? 20 : 10);

            // Tarkista onko asteroidi-tekstuuri määritelty
            if (texturePath == null && System.IO.File.Exists("meteorBrown_big1.png"))
            {
                // Valitse sopiva tekstuuri koon mukaan
                if (size == 3)
                    texturePath = "meteorBrown_big1.png";
                else if (size == 2)
                    texturePath = "meteorBrown_med1.png";
                else
                    texturePath = "meteorBrown_small1.png";
            }

            asteroids.Add(new Asteroid(position, velocity, radius, size, texturePath));
        }

        private void SplitAsteroid(Asteroid asteroid)
        {
            if (asteroid.Size > 1)
            {
                int newSize = asteroid.Size - 1;

                // Luo kaksi uutta asteroidia
                for (int i = 0; i < 2; i++)
                {
                    float angle = (float)random.NextDouble() * 2 * (float)Math.PI;
                    Vector2 newVelocity = new Vector2(
                        (float)Math.Sin(angle) * (70 + (float)random.NextDouble() * 30),
                        (float)Math.Cos(angle) * (70 + (float)random.NextDouble() * 30)
                    );

                    // Valitse sopiva tekstuuri koon mukaan
                    string texturePath = null;
                    if (newSize == 2 && System.IO.File.Exists("meteorBrown_med1.png"))
                        texturePath = "meteorBrown_med1.png";
                    else if (newSize == 1 && System.IO.File.Exists("meteorBrown_small1.png"))
                        texturePath = "meteorBrown_small1.png";

                    asteroids.Add(new Asteroid(asteroid.position, newVelocity, newSize == 2 ? 20 : 10, newSize, texturePath));
                }
            }

            // Lisää pisteitä
            score += asteroid.Size * 100;

            // Soita räjähdysääni
            if (soundsLoaded && explosionSound.FrameCount > 0)
                Raylib.PlaySound(explosionSound);

            // Tuhoa alkuperäinen asteroidi
            asteroid.Destroy();
        }

        private void CheckBulletAsteroidCollisions()
        {
            List<Bullet> bullets = player.GetBullets();

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = asteroids.Count - 1; j >= 0; j--)
                {
                    if (asteroids[j].CheckCollision(bullets[i].position))
                    {
                        // Asteroidi ja ammus osuvat toisiinsa
                        SplitAsteroid(asteroids[j]);

                        
                        break;
                    }
                }
            }
        }
    }
}
