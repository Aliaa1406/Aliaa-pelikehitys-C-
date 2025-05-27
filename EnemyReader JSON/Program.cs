using Raylib_cs;

namespace EnemyReader_JSON
{
    public class Program
    {
        const int screenWidth = 800;
        const int screenHeight = 600;
        const string gameTitle = "Artillery Game";

        static void Main()
        {
            // Initialize window
            Raylib.InitWindow(screenWidth, screenHeight, gameTitle);
            Raylib.SetTargetFPS(60);

            // Create and initialize the game
            Game game = new Game(screenWidth, screenHeight);
            game.Initialize();

            // Main game loop
            while (!Raylib.WindowShouldClose())
            {
                game.Update();
                game.Draw();
            }

            Raylib.CloseWindow();
        }
    }
}


    