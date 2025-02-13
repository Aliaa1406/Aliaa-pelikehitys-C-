using System.Numerics;
using Raylib_cs;
namespace Liikemäärä_ja_kiihtyvyys
{
    internal class Program
    {
        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;
        private const float GRAVITY = 100.0f;
        private const float LANDING_PLATFORM_Y = 550;
        private Spacecraft spacecraft;
        private bool gameOver = false;
        private bool missionSuccess = false;

        static void Main()
        {
            Program game = new Program();
            game.Init();
            game.GameLoop();
        }

        void Init()
        {
            Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Lunar Lander");
            Raylib.SetTargetFPS(60);
            spacecraft = new Spacecraft(new Vector2(WINDOW_WIDTH / 2, 100));
        }

        void GameLoop()
        {
            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            Raylib.CloseWindow();
        }

        void Update()
        {
            if (gameOver) return;

            float deltaTime = Raylib.GetFrameTime();
            spacecraft.Velocity += new Vector2(0, GRAVITY * deltaTime);
            spacecraft.Update(deltaTime);

            if (spacecraft.Position.Y >= LANDING_PLATFORM_Y)
            {
                gameOver = true;
                missionSuccess = spacecraft.Velocity.Y < 150;
            }
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawRectangle(WINDOW_WIDTH / 4, (int)LANDING_PLATFORM_Y, WINDOW_WIDTH / 2, 20, Color.Gray);
            spacecraft.Draw();

            if (gameOver)
            {
                string message = missionSuccess ? "Successful Landing!" : "The Rockit Crashed!";
                Raylib.DrawText(message, WINDOW_WIDTH / 2 - 100, WINDOW_HEIGHT / 2, 30, missionSuccess ? Color.Green : Color.Red);
            }

            Raylib.EndDrawing();
        }
    }
}

       
    


