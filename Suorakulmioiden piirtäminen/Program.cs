using System.Numerics;
using Raylib_cs;

Raylib.InitWindow(600, 400, "Suorakulmio");
Raylib.SetTargetFPS(60);

Rectangle collisionTest = new Rectangle(200, 50, 50, 60);

Vector2 position = new Vector2(200, 50);
Vector2 size = new Vector2(50, 60);

// Kulmapisteet ja keskipiste
Vector2 topLeftCorner = position;
Vector2 topRightCorner = position + new Vector2(size.X, 0);
Vector2 bottomLeftCorner = position + new Vector2(0, size.Y);
Vector2 bottomRightCorner = position + size;
Vector2 center = topLeftCorner + size / 2.0f;

Vector2 pointPosition = new Vector2(220, 60); // Testipiste

while (!Raylib.WindowShouldClose()) // 🔹 Nyt oikein
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    // Piirretään suorakulmio
    Raylib.DrawRectangleRec(collisionTest, Color.Blue);

    // Piirretään kulmat ja keskipiste
    Raylib.DrawCircleV(topLeftCorner, 5, Color.DarkGreen);
    Raylib.DrawCircleV(topRightCorner, 5, Color.DarkGreen);
    Raylib.DrawCircleV(bottomLeftCorner, 5, Color.DarkGreen);
    Raylib.DrawCircleV(bottomRightCorner, 5, Color.DarkGreen);
    Raylib.DrawCircleV(center, 5, Color.Red);

    // Piirretään piste
    Raylib.DrawCircleV(pointPosition, 4.0f, Color.White);

    // **Törmäystarkistus (täsmälleen se, mitä pyysit)**
    if (Raylib.CheckCollisionPointRec(pointPosition, collisionTest))
    {
        Raylib.DrawText("Collision!", (int)(collisionTest.X + collisionTest.Width),
                        (int)collisionTest.Y, 32, Color.White);
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
