using System.Collections.Specialized;
using System.Numerics;
using Raylib_cs;
using static System.Formats.Asn1.AsnWriter;
namespace Pong
{
    internal class Program
    {
        Ball ball;
        Paddle? player;
        static void Main(string[] args)
        {
            const int ScreenWidth = 800;
            const int ScreenHeight = 600;
            
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Pong Game");
            Raylib.SetTargetFPS(60);

            float paddleWidth = 20;
            float paddleHeight = 100;
            int paddleSpeed = 5;
            float ballSpeed = 4;
            float ballRadius = 10;

            // Create the ball
            Ball ball = new Ball(ScreenWidth / 2, ScreenHeight / 2, 4, 4, 20);
            Paddle player1 = new Paddle(30, ScreenHeight / 2 - paddleHeight / 2, paddleWidth, paddleHeight, paddleSpeed, Color.Orange);
            Paddle player2 = new Paddle(ScreenWidth - 30 - paddleWidth, ScreenHeight / 2 - paddleHeight / 2, paddleWidth, paddleHeight, paddleSpeed, Color.Violet);

            int csore1 = 0;
            int Score2 = 0;

            //Vector2 cenerLineTop = new Vector2(ScreenWidth /2, 0);
            //Vector2 cenerLineButtom = new Vector2(ScreenWidth /2, ScreenHeight);
            


            while (!Raylib.WindowShouldClose())
            {
                //update
                player1.Update(KeyboardKey.W, KeyboardKey.S, ScreenHeight );
                player2.Update(KeyboardKey.Up, KeyboardKey.Down,ScreenHeight);

                ball.Update(ScreenWidth, ScreenHeight);
                //check colleistion

                if (Raylib.CheckCollisionRecs(ball.GetRect(), player1.GetRect()))
                    ball.ReflectFromPaddle(player1);
                else if (Raylib.CheckCollisionRecs(ball.GetRect(), player2.GetRect()))
                    ball.ReflectFromPaddle(player2);

                // Check scoring
                if (ball.Position.X < 0)
                {
                    Score2++;
                    ball.Reset(ScreenWidth, ScreenHeight, 4);
                }
                else if (ball.Position.X > ScreenWidth)
                {
                    csore1++;
                    ball.Reset(ScreenWidth, ScreenHeight, -4);
                }

                // Drawing
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Draw center line (dashed)
                for (int y = 0; y < ScreenHeight; y += 30)
                {
                    Raylib.DrawLine(ScreenWidth / 2, y, ScreenWidth / 2, y + 15, Color.White);
                }

                // Draw scores
                Raylib.DrawText(csore1.ToString(), ScreenWidth / 4, 20, 40, Color.White);
                Raylib.DrawText(Score2.ToString(), 3 * ScreenWidth / 4, 20, 40, Color.White);

                // Draw ball
                ball.Draw();

                // Draw paddles
                player1.Draw();
                player2.Draw();

                //paddel 



                Raylib.EndDrawing();

                
            }
            Raylib.CloseWindow();
            return ;


        }

    }
}
