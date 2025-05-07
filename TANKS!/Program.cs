using System;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;
using TANKS_;

class Program
{
    static void Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;

        Raylib.InitWindow(screenWidth, screenHeight, "Tanks Game");
        Raylib.SetTargetFPS(60);

        // Alustetaan tankit aloituspaikoilleen
        Vector2 player1StartPos = new Vector2(100, 100);
        Vector2 player2StartPos = new Vector2(600, 400);

        Tank player1 = new Tank(player1StartPos, Color.Yellow);
        Tank player2 = new Tank(player2StartPos, Color.Green);

        // Luodaan seinät
        List<Wall> walls = new List<Wall>
        {
            new Wall(300, 100, 40, 400),
            new Wall(500, 100, 40, 400)
        };

        // Peli-silmukka
        while (!Raylib.WindowShouldClose())
        {
            // Päivitys
            player1.Update(KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D, KeyboardKey.Space);
            player2.Update(KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right, KeyboardKey.Enter);

            // Seinätörmäykset
            foreach (var wall in walls)
            {
                // Tarkista tankin törmäys seinään
                if (Raylib.CheckCollisionRecs(player1.GetBounds(), wall.Bounds))
                    player1.RevertLastMove();

                if (Raylib.CheckCollisionRecs(player2.GetBounds(), wall.Bounds))
                    player2.RevertLastMove();

                // Tarkista ammusten törmäys seinään
                if (player1.Bullet?.CheckCollision(wall.Bounds) == true)
                    player1.Bullet.Deactivate();

                if (player2.Bullet?.CheckCollision(wall.Bounds) == true)
                    player2.Bullet.Deactivate();
            }

            // Pidä tankit ruudun sisällä
            player1.ClampPosition(screenWidth, screenHeight);
            player2.ClampPosition(screenWidth, screenHeight);

            // Tarkista ammusten osumat tankkeihin
            if (player1.CheckBulletHit(player2))
            {
                player1.IncrementScore();
                player1.Bullet.Deactivate();

                // Palauta tankit aloituspaikkoihin
                player1.Reset(player1StartPos);
                player2.Reset(player2StartPos);
            }

            if (player2.CheckBulletHit(player1))
            {
                player2.IncrementScore();
                player2.Bullet.Deactivate();

                // Palauta tankit aloituspaikkoihin
                player1.Reset(player1StartPos);
                player2.Reset(player2StartPos);
            }

            // Piirtäminen
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkPurple);

            // Piirrä seinät
            foreach (var wall in walls)
                wall.Draw();

            // Piirrä tankit
            player1.Draw();
            player2.Draw();

            // Piirrä pisteet
            Raylib.DrawText($"Player 1: {player1.Score}", 10, 10, 20, Color.White);
            Raylib.DrawText($"Player 2: {player2.Score}", screenWidth - 150, 10, 20, Color.White);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}