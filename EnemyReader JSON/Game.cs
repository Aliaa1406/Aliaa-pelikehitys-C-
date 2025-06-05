
using EnemyReader;
using Newtonsoft.Json;
using Raylib_cs;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;


namespace EnemyReader_JSON
{
    internal class Game
    {
        private int screenWidth;
        private int screenHeight;

        // Game state
        private int redScore = 0;
        private int blueScore = 0;
        private float redPower = 10.0f;
        private float redAngle = 45.0f;
        private float bluePower = 10.0f;
        private float blueAngle = 45.0f;
        private int choose;

        // Game elements
        private Terrain terrain;
        private Artillery redTank;
        private Artillery blueTank;
        private List<Bomb> bombs; 
        private List<Enemy> enemies;
        private List<Explosion> explosions;
        private List<Bomb> bombTypes;

        public Game(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
            terrain = new Terrain(screenWidth, screenHeight);
            enemies = new List<Enemy>();
            bombs = new List<Bomb>(); 
            explosions = new List<Explosion>();
            bombTypes = new List<Bomb>();
        }

        private Bomb choise()
        {
          choose =  Math.Clamp(choose,0 , bombTypes.Count-1);
            return bombTypes[choose];
        }

        public void Initialize()
        {
            terrain.Generate();

            redTank = new Artillery(new Vector2(150, terrain.GetHeightAt(150) - 10), Color.Red, 20, 10, 20, 3);
            blueTank = new Artillery(new Vector2(650, terrain.GetHeightAt(650) - 10), Color.Blue, 20, 10, 20, 3);
            Bomb loadit = new Bomb();
            loadit = LoadBomb("RedBullet.txt");
            bombTypes.Add(loadit);
            loadit = LoadBomb("YellowBullet.txt");
            bombTypes.Add((loadit));
        }


        public Bomb LoadBomb( string filename)
        {
            if (File.Exists(filename) == false)
            {
                Console.WriteLine($"Could not read file {filename}");
            }
            else
            {
                string text = File.ReadAllText(filename);

                try
                {
                    return  JsonConvert.DeserializeObject<Bomb>(text);
                }
                catch (JsonReaderException exp)
                {
                    Console.WriteLine($"Error in file {filename} at {exp.LineNumber}: {exp.LinePosition}");
                    Console.WriteLine(exp.Message);
                }
            }
            return null;
        }




        private void SpawnEnemies(int count)
        {
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                int x = random.Next(200, screenWidth - 200);
                int y = terrain.GetHeightAt(x) - 15;

                Enemy enemy = new Enemy(new Vector2(x, y), 15, Color.Green);
                enemies.Add(enemy);
            }
        }

        public void Update()
        {
            HandleInput();
            UpdateBombs(); 
            UpdateExplosions();
        }

        private void UpdateBombs()
        {
            List<Bomb> bombsToRemove = new List<Bomb>();

            foreach (Bomb bomb in bombs)
            {
                bomb.Update(0.2f);

                int bombX = (int)bomb.Position.X;
                int bombY = (int)bomb.Position.Y;

                if (bombX < 0) bombX = 0;
                if (bombX >= screenWidth) bombX = screenWidth - 1;

                if (bombY >= terrain.GetHeightAt(bombX))
                {
                    bomb.Active = false;
                    CreateExplosion(bombX, bombY);
                    bombsToRemove.Add(bomb);
                    continue;
                }

                Artillery hitTank = CheckTankHit(bomb);
                if (hitTank != null)
                {
                    CreateExplosion((int)hitTank.Position.X, (int)hitTank.Position.Y - 5);
                    if (hitTank == redTank) blueScore++;
                    else redScore++;

                    bomb.Active = false;
                    bombsToRemove.Add(bomb);
                    continue;
                }

                Enemy hitEnemy = CheckEnemyHit(bomb);
                if (hitEnemy != null)
                {
                    hitEnemy.TakeDamage();

                    if (hitEnemy.Health <= 0)
                    {
                        if (bomb.FiredByRed) redScore += 2;
                        else blueScore += 2;

                        enemies.Remove(hitEnemy);
                    }

                    bomb.Active = false;
                    bombsToRemove.Add(bomb);
                    continue;
                }

                if (bomb.Position.X < 0 || bomb.Position.X > screenWidth ||
                    bomb.Position.Y < 0 || bomb.Position.Y > screenHeight)
                {
                    bomb.Active = false;
                    bombsToRemove.Add(bomb);
                    continue;
                }

                foreach (Bomb otherBomb in bombs)
                {
                    if (bomb != otherBomb &&
                        Vector2.Distance(bomb.Position, otherBomb.Position) < (bomb.Radius + otherBomb.Radius))
                    {
                        Vector2 collisionPoint = (bomb.Position + otherBomb.Position) / 2;
                        CreateExplosion((int)collisionPoint.X, (int)collisionPoint.Y, 15);

                        if (!bombsToRemove.Contains(bomb))
                            bombsToRemove.Add(bomb);
                        if (!bombsToRemove.Contains(otherBomb))
                            bombsToRemove.Add(otherBomb);

                        bomb.Active = false;
                        otherBomb.Active = false;
                        break;
                    }
                }
            }

            foreach (Bomb bomb in bombsToRemove)
            {
                bombs.Remove(bomb);
            }
        }

        
        private void HandleInput()
        {
            if (Raylib.IsKeyDown(KeyboardKey.W)) redAngle += 1.0f;
            if (Raylib.IsKeyDown(KeyboardKey.S)) redAngle -= 1.0f;
            if (Raylib.IsKeyDown(KeyboardKey.D)) redPower += 0.2f;
            if (Raylib.IsKeyDown(KeyboardKey.A)) redPower -= 0.2f;

            if (Raylib.IsKeyPressed(KeyboardKey.Q)) choose++;
            if (Raylib.IsKeyPressed(KeyboardKey.E)) choose--;



            redAngle = Math.Clamp(redAngle, 0, 90);
            redPower = Math.Clamp(redPower, 5, 30);

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                FireBomb(redTank, redAngle, redPower, true);
            }

            if (Raylib.IsKeyDown(KeyboardKey.Up)) blueAngle += 1.0f;
            if (Raylib.IsKeyDown(KeyboardKey.Down)) blueAngle -= 1.0f;
            if (Raylib.IsKeyDown(KeyboardKey.Right)) bluePower += 0.2f;
            if (Raylib.IsKeyDown(KeyboardKey.Left)) bluePower -= 0.2f;

            blueAngle = Math.Clamp(blueAngle, 0, 90);
            bluePower = Math.Clamp(bluePower, 5, 30);

            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                FireBomb(blueTank, blueAngle, bluePower, false);
            }

            
        }

        public Color StringToColor(string color)
        {
            if(color  == "Red") return Color.Red;
            if (color == "Yallow") return Color.Yellow; 

            return Color.White;
        }

        private void FireBomb(Artillery tank, float angle, float power, bool isRedFiring)
        {
            float radians, velocityX, velocityY, barrelEndX, barrelEndY;

            if (isRedFiring)
            {
                radians = (float)(angle * Math.PI / 180.0f);
                barrelEndX = tank.Position.X + (float)Math.Cos(radians) * tank.BarrelLength;
                barrelEndY = tank.Position.Y - (float)Math.Sin(radians) * tank.BarrelLength;
                velocityX = (float)Math.Cos(radians) * power;
                velocityY = -(float)Math.Sin(radians) * power;
            }
            else
            {
                radians = (float)((180 - angle) * Math.PI / 180.0f);
                barrelEndX = tank.Position.X + (float)Math.Cos(radians) * tank.BarrelLength;
                barrelEndY = tank.Position.Y - (float)Math.Sin(radians) * tank.BarrelLength;
                velocityX = (float)Math.Cos(radians) * power;
                velocityY = -(float)Math.Sin(radians) * power;
            }

            Vector2 bombPosition = new Vector2(barrelEndX, barrelEndY);
            Vector2 bombVelocity = new Vector2(velocityX, velocityY);

            Bomb newBomb = new Bomb(isRedFiring);
            newBomb = choise();
            newBomb.FiredByRed = isRedFiring;
            newBomb.ScreenColor = StringToColor(newBomb.color);
            newBomb.Fire(bombPosition, bombVelocity);
            bombs.Add(newBomb); // Updated
        }

        private Artillery CheckTankHit(Bomb bomb)
        {
            Artillery tankToCheck = bomb.FiredByRed ? blueTank : redTank;

            Rectangle tankRect = new Rectangle(
                tankToCheck.Position.X - tankToCheck.Width / 2,
                tankToCheck.Position.Y - tankToCheck.Height,
                tankToCheck.Width,
                tankToCheck.Height
            );

            return Raylib.CheckCollisionPointRec(bomb.Position, tankRect) ? tankToCheck : null;
        }

        private Enemy CheckEnemyHit(Bomb bomb)
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy.CheckHit(bomb.Position))
                {
                    return enemy;
                }
            }
            return null;
        }

        private void CreateExplosion(int x, int y, int radius = 20)
        {
            terrain.CreateCrater(x, y, radius);
            explosions.Add(new Explosion(new Vector2(x, y), 30, 1.0f));
        }

        private void UpdateExplosions()
        {
            List<Explosion> explosionsToRemove = new List<Explosion>();

            foreach (Explosion explosion in explosions)
            {
                explosion.Update();

                if (explosion.IsComplete)
                {
                    explosionsToRemove.Add(explosion);
                }
            }

            foreach (Explosion explosion in explosionsToRemove)
            {
                explosions.Remove(explosion);
            }
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(20, 20, 40, 255));

            terrain.Draw();
            redTank.Draw(redAngle);
            blueTank.Draw(180 - blueAngle);

            foreach (Bomb bomb in bombs)
            {
                bomb.Draw();
            }

            foreach (Explosion explosion in explosions)
            {
                explosion.Draw();
            }

            DrawGameInfo();
            DrawControlsHelp();

            if (redScore >= 5 || blueScore >= 5)
            {
                string winner = redScore >= 5 ? "RED TANK WINS!" : "BLUE TANK WINS!";
                Color winColor = redScore >= 5 ? Color.Red : Color.Blue;

                Raylib.DrawRectangle(0, screenHeight / 2 - 50, screenWidth, 100, new Color(0, 0, 0, 200));
                Raylib.DrawText(winner, screenWidth / 2 - Raylib.MeasureText(winner, 40) / 2, screenHeight / 2 - 20, 40, winColor);
                Raylib.DrawText("PRESS ENTER TO RESTART", screenWidth / 2 - 180, screenHeight / 2 + 30, 20, Color.White);

                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    redScore = 0;
                    blueScore = 0;
                    redPower = 10.0f;
                    redAngle = 45.0f;
                    bluePower = 10.0f;
                    blueAngle = 45.0f;

                    bombs.Clear();
                    explosions.Clear();
                    terrain.Generate();
                    enemies.Clear();
                    SpawnEnemies(3);
                    redTank.Position = new Vector2(150, terrain.GetHeightAt(150) - 10);
                    blueTank.Position = new Vector2(650, terrain.GetHeightAt(650) - 10);
                }
            }

            Raylib.EndDrawing();
        }

        private void DrawControlsHelp()
        {
            Raylib.DrawRectangle(0, screenHeight - 50, screenWidth, 50, new Color(0, 0, 0, 150));
            
        }

        private void DrawGameInfo()
        {
            Raylib.DrawText($"Red score: {redScore}", 20, 20, 20, Color.Red);
            Raylib.DrawText($"Blue score: {blueScore}", screenWidth - 200, 20, 20, Color.Blue);
            Raylib.DrawText($"Active bombs: {bombs.Count}", screenWidth / 2 - 80, 20, 20, Color.Yellow);
            

            for(int i = 0; i <bombTypes.Count; i++) 
            { 

            Raylib.DrawText($"RedBullet : Name:{bombTypes[i].name}, {bombTypes [i].damage} , {bombTypes[i].color} , {bombTypes [i].explosionSize} " , screenWidth / 2 - 80, 50 + i * 20, 20, Color.Yellow);
            }
        }
    }
}
