using System;
using System.Numerics;
using Raylib_cs;
using TANKS_;

public class Tank
{
    public Vector2 Position;
    public Vector2 Direction;
    public Vector2 PreviousPosition;
    public Color Color;
    public Bullet Bullet;
    public int Score { get; private set; } = 0;

    private Vector2 tankSize = new Vector2(40, 40);
    private Vector2 turretSize = new Vector2(16, 16);
    private float speed = 200.0f; // Käytetään korkeampaa nopeutta, koska käytämme GetFrameTime()
    private double lastShootTime = 0;
    private readonly double shootInterval = 1.0; // 1 sekunti ampumisten välillä

    public Tank(Vector2 startPosition, Color color)
    {
        Position = startPosition;
        PreviousPosition = startPosition;
        Direction = new Vector2(1, 0); // Oletussuunta oikealle
        Color = color;
    }

    public void Update(KeyboardKey up, KeyboardKey down, KeyboardKey left, KeyboardKey right, KeyboardKey shoot)
    {
        PreviousPosition = Position;

        // Liikkeen päivitys GetFrameTime()-funktion avulla nopeuden normalisoimiseksi
        float deltaTime = Raylib.GetFrameTime();
        Vector2 movement = Vector2.Zero;

        // Tarkista näppäimet yksi kerrallaan, vältä diagonaalinen liike
        if (Raylib.IsKeyDown(up))
        {
            movement.Y = -1;
            Direction = new Vector2(0, -1); // Ylös
        }
        else if (Raylib.IsKeyDown(down))
        {
            movement.Y = 1;
            Direction = new Vector2(0, 1); // Alas
        }
        else if (Raylib.IsKeyDown(left))
        {
            movement.X = -1;
            Direction = new Vector2(-1, 0); // Vasemmalle
        }
        else if (Raylib.IsKeyDown(right))
        {
            movement.X = 1;
            Direction = new Vector2(1, 0); // Oikealle
        }

        // Päivitä tankin sijainti
        if (movement.X != 0 || movement.Y != 0)
        {
            Position += Vector2.Normalize(movement) * speed * deltaTime;
        }

        // Päivitä ammus, jos sellainen on aktiivinen
        Bullet?.Update();

        // Tarkista ammuksen osuminen reunoihin
        if (Bullet != null && Bullet.Active)
        {
            if (Bullet.Position.X < 0 || Bullet.Position.X > Raylib.GetScreenWidth() ||
                Bullet.Position.Y < 0 || Bullet.Position.Y > Raylib.GetScreenHeight())
            {
                Bullet.Deactivate();
            }
        }

        // Ampuminen
        if (Raylib.IsKeyPressed(shoot))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        double currentTime = Raylib.GetTime();

        // Tarkista onko ampumisaika kulunut
        if (currentTime - lastShootTime > shootInterval)
        {
            // Luo uusi ammus vain jos aiempi ei ole aktiivinen tai se on null
            if (Bullet == null || !Bullet.Active)
            {
                // Laske ammuksen lähtöpaikka (tankin keskeltä direktion suuntaan)
                Vector2 bulletPos = Position + Direction * (tankSize.X / 2.0f);
                Bullet = new Bullet(bulletPos, Direction);
                lastShootTime = currentTime;
            }
        }
    }

    public void Draw()
    {
        // Piirrä tankki
        DrawTank(Position, tankSize, Direction, turretSize, Color);

        // Piirrä ammus, jos sellainen on
        Bullet?.Draw();
    }

    public void DrawTank(Vector2 position, Vector2 tankSize, Vector2 direction, Vector2 turretSize, Color color)
    {
        // Tankin runko
        Vector2 topLeft = position - tankSize / 2.0f;
        Raylib.DrawRectangleV(topLeft, tankSize, color);

        // Tankin tykki suunnattu suunnan mukaan
        Vector2 turretPos = position + direction * (tankSize.X / 2.0f - turretSize.X / 2.0f);
        Vector2 turretTopLeft = turretPos - turretSize / 2.0f;
        Raylib.DrawRectangleV(turretTopLeft, turretSize, Color.Black);
    }

    public Rectangle GetBounds()
    {
        Vector2 topLeft = Position - tankSize / 2.0f;
        return new Rectangle(topLeft.X, topLeft.Y, tankSize.X, tankSize.Y);
    }

    public void RevertLastMove()
    {
        Position = PreviousPosition;
    }

    public void ClampPosition(int screenWidth, int screenHeight)
    {
        float halfWidth = tankSize.X / 2.0f;
        float halfHeight = tankSize.Y / 2.0f;

        // Pidä tankki ruudun sisällä
        Position.X = Math.Clamp(Position.X, halfWidth, screenWidth - halfWidth);
        Position.Y = Math.Clamp(Position.Y, halfHeight, screenHeight - halfHeight);
    }

    public void IncrementScore()
    {
        Score++;
    }

    public void Reset(Vector2 startPosition)
    {
        Position = startPosition;
        PreviousPosition = startPosition;
        Bullet = null;
    }

    public bool CheckBulletHit(Tank otherTank)
    {
        // Tarkista osuuko tämän tankin ammus toiseen tankkiin
        if (Bullet != null && Bullet.Active)
        {
            return Bullet.CheckCollision(otherTank.GetBounds());
        }
        return false;
    }
}