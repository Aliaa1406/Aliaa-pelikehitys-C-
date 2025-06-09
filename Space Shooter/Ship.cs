
using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Ship
    {
        public Vector2 position;
        public Vector2 direction;
        public Vector2 velocity;
        public float angle;
        private float rotationSpeed = 4.0f;
        private float acceleration = 5.0f;
        private float maxSpeed = 300.0f;
        private float currentSpeed = 0.0f;
        private float drag = 0.98f;
        private List<Bullet> bullets;
        private float shootCooldown = 0.3f;
        private float currentCooldown = 0;
        private Texture2D shipTexture;
        private bool textureLoaded = false;

        private Sound shootSound;
        private bool soundLoaded = false;

        public Ship(Vector2 initialPosition, string texturePath)
        {
            position = initialPosition;
            direction = new Vector2(0, -1);
            velocity = new Vector2(0, 0);
            angle = 0;
            bullets = new List<Bullet>();

            // Tarkista onko tiedosto olemassa ennen kuin yritetään ladata sitä
            if (System.IO.File.Exists(texturePath))
            {
                shipTexture = Raylib.LoadTexture(texturePath);
                textureLoaded = true;
            }
            else
            {
                Console.WriteLine($"Virhe tekstuurin latauksessa: tiedostoa {texturePath} ei löydy");
                textureLoaded = false;
            }
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
                Console.WriteLine("Shoot sound loaded successfully");
            }
            else
            {
                Console.WriteLine($"Error: Shoot sound not found: {soundPath}");
                soundLoaded = false;
            }
        }

            public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            // Käsittele kierto
            if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A))
            {
                angle -= rotationSpeed;
                UpdateDirection();
            }
            if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D))
            {
                angle += rotationSpeed;
                UpdateDirection();
            }

            // Käsittele kiihdytys
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W))
            {
                currentSpeed += acceleration;
                if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
            }
            if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S))
            {
                currentSpeed -= acceleration;
                if (currentSpeed < 0.0f)
                {
                    currentSpeed = 0.0f;
                }
            }

            // Päivitä nopeus suunnan mukaan
            velocity = direction * currentSpeed * deltaTime;

            // Päivitä sijainti
            position.X += velocity.X;
            position.Y += velocity.Y;

            // Sovella kitkaa
            currentSpeed *= drag;

            // Käsittele ruudun rajojen ylitys
            WrapAroundScreen(screenWidth, screenHeight);

            // Päivitä ampumisen viilennysaika
            if (currentCooldown > 0)
            {
                currentCooldown -= deltaTime;
            }

            // Käsittele ampuminen
            if (Raylib.IsKeyPressed(KeyboardKey.Space) && currentCooldown <= 0)
            {
                currentCooldown = shootCooldown;
                bullets.Add(new Bullet(position, direction));

                if (soundLoaded)
                {
                    Raylib.PlaySound(shootSound);
                }

            }

            // Päivitä ammukset
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime, screenWidth, screenHeight);

                // Poista ammukset, joiden elinaika on loppunut
                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
                }
            }
        }


        private void UpdateDirection()
        {
            // Päivitä suuntavektori kulman perusteella
            float radians = angle * (float)Math.PI / 180.0f;
            direction.X = (float)Math.Sin(radians);
            direction.Y = -(float)Math.Cos(radians);
        }

        private void WrapAroundScreen(int screenWidth, int screenHeight)
        {
            // Käsittele ruudun rajojen ylitys
            if (position.X < 0)
                position.X = screenWidth;
            else if (position.X > screenWidth)
                position.X = 0;

            if (position.Y < 0)
                position.Y = screenHeight;
            else if (position.Y > screenHeight)
                position.Y = 0;
        }

        public void Draw()
        {
            if (textureLoaded)
            {
                // Piirrä alus käyttäen tekstuuria
                Rectangle sourceRec = new Rectangle(0, 0, shipTexture.Width, shipTexture.Height);

                // Kohdesuorakulmio
                Rectangle destRec = new Rectangle(position.X, position.Y, shipTexture.Width, shipTexture.Height);

                // Kuvan keskipiste (tarvitaan kierrättämiseen)
                Vector2 origin = new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);

                // Piirrä tekstuuri kierrettynä
                Raylib.DrawTexturePro(
                    shipTexture,
                    sourceRec,
                    destRec,
                    origin,
                    angle,
                    Color.White
                );
            }
            else
            {
                // Jos tekstuuria ei ole, piirrä kolmio varatoimenpiteenä
                float radians = angle * (float)Math.PI / 180.0f;
                Vector2 p1 = new Vector2(
                    position.X + (float)Math.Sin(radians) * 20.0f,
                    position.Y - (float)Math.Cos(radians) * 20.0f
                );
                Vector2 p2 = new Vector2(
                    position.X + (float)Math.Sin(radians + 2.5f) * 15.0f,
                    position.Y - (float)Math.Cos(radians + 2.5f) * 15.0f
                );
                Vector2 p3 = new Vector2(
                    position.X + (float)Math.Sin(radians - 2.5f) * 15.0f,
                    position.Y - (float)Math.Cos(radians - 2.5f) * 15.0f
                );

                Raylib.DrawTriangle(p1, p2, p3, Color.White);
            }

            // Piirrä ammukset
            foreach (var bullet in bullets)
            {
                bullet.Draw();
            }
        }

        public void Reset(int screenWidth, int screenHeight)
        {
            // Resetoi pelaajan sijainti keskelle ruutua
            position = new Vector2(screenWidth / 2, screenHeight / 2);

            // Pysäytä liike
            velocity = Vector2.Zero;
            currentSpeed = 0;

            // Resetoi kulma
            angle = 0;
            direction = new Vector2(0, -1);

           
        }

        public void Destroy(int screenWidth, int screenHeight)
        {
            // Resetoi pelaajan sijainti keskelle ruutua
            Reset(screenWidth, screenHeight);

            // Poista kaikki ammukset
            bullets.Clear();
        }

        // Palauttaa ammukset törmäystarkistuksia varten
        public void UnloadResources()
        {
            if (textureLoaded)
            {
                Raylib.UnloadTexture(shipTexture);
                textureLoaded = false;
            }

            if (soundLoaded)
            {
                Raylib.UnloadSound(shootSound);
                soundLoaded = false;
            }
        }

        // Palauttaa ammukset törmäystarkistuksia varten
        public List<Bullet> GetBullets()
        {
            return bullets;
        }
    }

}
