using System.Numerics;
using Raylib_cs;

namespace Raylib_testi;

internal class Program
{
    static void Main(string[] args)
    {
        int width = 600;
        int height = 400;
        Vector2 A = new Vector2(width / 2, 0);
        Vector2 B = new Vector2(0, height / 2);
        Vector2 C = new Vector2(width, height * 3 / 4);

        Vector2 Amove = new Vector2(1, 1);
        Vector2 Bmove = new Vector2(1, -1);
        Vector2 Cmove = new Vector2(-1, 1);


        float speed = 100f;




        //Console.WriteLine("Hello, World!");
        Raylib.InitWindow(width, height, "Raylib_testi");
        while (Raylib.WindowShouldClose() == false)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            //line draw
            Raylib.DrawLineV(A, B, Color.Green);
            Raylib.DrawLineV(B, C, Color.Red);
            Raylib.DrawLineV(C, A, Color.Yellow);

            Vector2 Move = Amove * speed * Raylib.GetFrameTime();
            A = A + Move;
            Vector2 Move1 = Bmove * speed * Raylib.GetFrameTime();
            B = B + Move1;
            Vector2 Move2 = Cmove * speed * Raylib.GetFrameTime();
            C = C + Move2;

            if (A.X <= 0 || A.X >= width)
            { Amove.X = Amove.X * -1; }

            if (A.Y <= 0 || A.Y >= height)
            {
                Amove.Y = Amove.Y *= -1;
            }

            if (B.X <= 0 || B.X >= width)
            { Bmove.X= Bmove.X * -1; }
            if (B.Y <= 0 || B.Y >= height)
            { Bmove.Y = Bmove.Y *= -1; }

            if (C.X <= 0 || C.X >= width) { Cmove.X= Cmove.X *= -1; }
            if (C.Y <= 0 || C.Y >= height) { Cmove.Y =Cmove.Y *= -1; }





            // Raylib.DrawText("HELLO", 100, 100, 32, Color.White);
            Raylib.EndDrawing();
            //program the reast of the game :D

        }



        Raylib.CloseWindow();
    }
}
