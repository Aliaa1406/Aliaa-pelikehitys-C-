using Raylib_cs;
using DVD1;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;


namespace DVD1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int Screenwidth = 100;
            int Screenheight = 100;

            Vector2 A = new Vector2(Screenwidth / 2, 0);
            Vector2 B = new Vector2(0, Screenheight / 2);
            

            Vector2 Amove = new Vector2(1, 0);
            Vector2 Bmove = new Vector2(0, -1);



             float Speed = 100.0f;

           
           



           // Color linecolor = Color.White;
           // List<Color>

            Raylib.InitWindow(Screenwidth, Screenheight, "Raylib_texti");
            while (Raylib.WindowShouldClose() == false) 
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

               Vector2 tex_size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), "DVD", 14, 2);
                Raylib.DrawText("DVD", (int)A.X, (int)A.Y, 14, Color.Yellow);
                Vector2 Move = Amove * Speed * Raylib.GetFrameTime();
                A = A + Move;

                if (A.X < 0 || A.X > Screenwidth)
                { 
                    Amove.X = Amove.X * -1;
                   
                }
                if (A.Y <= 0 || A.Y >= Screenheight)
                { Amove.Y = A.Y * -1; }
                if (B.X <= 0 || B.X >= Screenwidth)
                { Bmove.X = Bmove.X * -1; }
                if (B.Y <= 0 || B.Y >= Screenheight)
                { Bmove.Y = Bmove.Y * -1; }

                Raylib.EndDrawing();

            }
            Raylib.CloseWindow();
            
         
            
            
        }
    }
}
