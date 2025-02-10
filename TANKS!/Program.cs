using System;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;
using TANKS_;

public class Bullet
{
    public Vector2 Position { get; private set; }
    public Vector2 Direction { get; private set; }
    public bool Active { get; private set; } = true;
    public float Speed { get; private set; } = 5.0f;
    private Rectangle bulletRect;
    private const int bulletSize = 8;

    public Bullet(Vector2 position, Vector2 direction)
    {
        Position = position;
        Direction = direction;
        UpdateBulletRect();
    }

    public void Update()
    {
        if (!Active) return;
        Position += Direction * Speed;
        UpdateBulletRect();
    }

    private void UpdateBulletRect()
    {
        bulletRect = new Rectangle(Position.X - bulletSize / 2, Position.Y - bulletSize / 2, bulletSize, bulletSize);
    }

    public void Draw()
    {
        if (!Active) return;
        Raylib.DrawRectangleRec(bulletRect, Color.Red);
    }

    public bool CheckCollision(Rectangle other)
    {
        return Active && Raylib.CheckCollisionRecs(bulletRect, other);
    }

    public void Deactivate()
    {
        Active = false;
    }
}



class Program
{
    static void Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;

        Raylib.InitWindow(screenWidth, screenHeight, "Tanks Game");
        Raylib.SetTargetFPS(60);

        Tank player1 = new Tank(new Vector2(100, 100), Color.Yellow);
        Tank player2 = new Tank(new Vector2(600, 400), Color.Violet);

        List<Wall> walls = new List<Wall>
        {
            new Wall(300, 100, 40, 400),
            new Wall(500, 100, 40, 400)
        };

        while (!Raylib.WindowShouldClose())
        {
            // Update
            player1.Update(KeyboardKey.W, KeyboardKey.S, KeyboardKey.A, KeyboardKey.D, KeyboardKey.Space);
            player2.Update(KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right, KeyboardKey.Enter);

            // Wall collisions
            foreach (var wall in walls)
            {
                if (Raylib.CheckCollisionRecs(player1.GetBounds(), wall.Bounds))
                    player1.RevertLastMove();
                if (Raylib.CheckCollisionRecs(player2.GetBounds(), wall.Bounds))
                    player2.RevertLastMove();

                if (player1.Bullet?.CheckCollision(wall.Bounds) == true)
                    player1.Bullet.Deactivate();
                if (player2.Bullet?.CheckCollision(wall.Bounds) == true)
                    player2.Bullet.Deactivate();
            }

            // Keep tanks in bounds
            player1.ClampPosition(screenWidth, screenHeight);
            player2.ClampPosition(screenWidth, screenHeight);

            // Draw
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Green);

            foreach (var wall in walls)
                wall.Draw();

            player1.Draw();
            player2.Draw();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}