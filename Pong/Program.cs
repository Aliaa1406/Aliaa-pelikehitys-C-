using System.Numerics;
using Raylib_cs;
namespace Pong
{
    internal class Program
    {
        const int ScreenWidth = 800;
        const int ScreenHeight = 450;

        static void Main(string[] args)
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Pong");
            Raylib.SetTargetFPS(60);

            int paddleWidth = 30, paddleHeight = 120;
            int paddleSpeed = 5;
            int Speedball = 4;
            
            Vector2 Plr1 = new Vector2(20,ScreenHeight/2 - paddleHeight);
            Vector2 Plr2 = new Vector2(ScreenWidth -30, ScreenHeight/2 - paddleHeight/2);
            Vector2 ball = new Vector2(ScreenWidth/2, ScreenHeight/2);
            Vector2 ballVelocity = new Vector2(Speedball, Speedball);

            int Score1 = 0;
            int Score2 = 0;

            while (Raylib.WindowShouldClose() ==  false)
            {
                // player move
                if (Raylib.IsKeyDown(KeyboardKey.W) && Plr1.Y > 0) Plr1.Y -= paddleSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.E) && Plr1.Y < ScreenHeight - paddleHeight) Plr1.Y += paddleSpeed;

                if (Raylib.IsKeyDown(KeyboardKey.Up) && Plr2.Y > 0) Plr2.Y -= paddleSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.Down) && Plr2.Y < ScreenHeight - paddleHeight) Plr2.Y += paddleSpeed;
                // ball move
                ball += ballVelocity;
                //Collision with the top and bottom edges
                if (ball.Y <= 0 || ball.Y >= ScreenHeight) ballVelocity.Y *= -1;

                //Collision with players' rackets
                if ((ball.X <= Plr1.X + paddleWidth && ball.Y  <= Plr1.Y + paddleHeight) || (ball.X <= Plr2.X + paddleWidth && ball.Y <= Plr2.Y + paddleHeight ))
                {
                    ballVelocity.X *= -1;
                }

                // Counting the score and resetting the ball
                if (ball.X <= 0)
                {
                    Score2 += 1; ball = new Vector2(ScreenWidth / 2, ScreenHeight / 2); ballVelocity.X = Speedball;
                }
                if(ball.X>ScreenWidth )
                {
                    Score1+= 1; ball = new Vector2(ScreenWidth / 2, ScreenHeight / 2);ballVelocity.X = -Speedball;
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                Raylib.DrawRectangle((int)Plr1.X, (int)Plr1.Y, paddleWidth, paddleHeight, Color.Orange);
                Raylib.DrawRectangle((int)Plr2.X, (int)Plr2.Y, paddleWidth, paddleHeight, Color.Violet);
                Raylib.DrawCircle((int)ball.X, (int)ball.Y, 16, Color.White);

                Raylib.DrawText(Score1.ToString(),ScreenWidth/2-50,20,30,(Color.Red));
                Raylib.DrawText(Score2.ToString(),ScreenWidth/2-50,20,30,(Color.Red));

                Raylib.DrawLine(ScreenWidth/2,0,ScreenWidth/2,ScreenHeight,Color.White);
                Raylib.EndDrawing();



            }
            Raylib.CloseWindow();


        }
    }
}
