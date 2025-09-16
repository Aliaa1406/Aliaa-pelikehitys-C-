using RayGuiCreator;
using Raylib_cs;

namespace Valikkopeli
{
    enum GameState
    {
        MainMenu,

        GameLoop,

        PauseMenu
    }
    internal class Game
    {

        GameState currenState;

        public void Run()
        {
            Raylib.InitWindow(640, 480, "Valikkopeli");

            // the game end  prees esc
            Raylib.SetExitKey(KeyboardKey.Null);
            currenState = GameState.MainMenu;
            while (Raylib.WindowShouldClose() == false)
            {
                Update();
                Draw();
            }
        }

        void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                switch (currenState)
                {
                    case GameState.MainMenu:
                        ChangeState(GameState.GameLoop);
                        break;
                    case GameState.GameLoop:
                        ChangeState(GameState.MainMenu);
                        break;
                }
            }

            switch (currenState)
            {
                case GameState.GameLoop:
                    // esc pause menu
                    if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                    {
                        ChangeState(GameState.PauseMenu);
                    }
                    break;

                case GameState.PauseMenu:

                    if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                    {
                        ChangeState(GameState.GameLoop);
                    }
                    break;
            }

        }

        void ChangeState(GameState nextState)
        {
            //to do cheack that transition is valid 
            if (currenState == GameState.MainMenu && nextState == GameState.PauseMenu)
                switch (nextState)
                {
                    case GameState.MainMenu:
                        //raylib .playmusic stream(musicmusic);
                        break;
                    case GameState.GameLoop:
                        //start game muic
                        //raylib.playmusicsram(gamemusic);
                        // load level
                        break;

                    case GameState.PauseMenu:
                        // raylib set Music volume (game music ,0.2f);
                        break;
                }
            currenState = nextState;
        }

        //public void OnOptionsMenuBack()
        //{
        //    ChangeState(GameState.PauseMenu);
        //}

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Purple);
            Raylib.DrawText($"{currenState}", 10, 10, 32, Color.Yellow);

            switch (currenState)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.GameLoop:
                    break;

            }
            Raylib.EndDrawing();
        }

        void DrawMainMenu()
        {

            Raylib.ClearBackground(Color.Black);
            MenuCreator creator = new MenuCreator(60, 60, 16, 200);

            //game name
            creator.Label("Valikkopeli");

            creator.Label("info: Space = change ");

            if (creator.Button("Start Game"))
            {
                currenState = GameState.GameLoop;
            }

            //if (creator.Button("Exit"))
            //{

            //}

        }


    }

}
