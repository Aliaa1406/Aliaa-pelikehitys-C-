
using System.Numerics;
using Raylib_cs;

namespace EnemyReader_JSON
{
    internal class Bomb
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Direction;
        public float Angle;
        public bool Active;
        public float Radius = 4f;
        public Color ScreenColor;
        public bool FiredByRed;

        // choose
        public string name;
        public int damage;
        public string color;
        public int explosionSize;

        public Bomb() 
        {
            Active = false;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Direction = new Vector2(1.0f, 0.0f); // Default direction (right)
            Angle = 0.0f;
          


         
        }


        public Bomb(bool firedByRed = true)
        {
            Active = false;
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Direction = new Vector2(1.0f, 0.0f); // Default direction (right)
            Angle = 0.0f;
            FiredByRed = firedByRed;

            
            ScreenColor= firedByRed ? new Color(255, 200, 50, 255) : new Color(50, 200, 255, 255);
        }

        // Recreate direction vector using angle
        public void UpdateDirectionFromAngle(float turnAmount)
        {
            // Update angle
            Angle += turnAmount;

            // Create rotation matrix
            Matrix4x4 rotation = Matrix4x4.CreateRotationZ(Angle);

            // Base direction (right)
            Vector2 right = new Vector2(1.0f, 0.0f);

            // Transform to get new direction
            Direction = Vector2.Transform(right, rotation);
        }

        //  Rotate existing direction vector
        public void RotateDirection(float turnAmount)
        {
           
            Matrix4x4 rotation = Matrix4x4.CreateRotationZ(turnAmount);

            
            Direction = Vector2.Transform(Direction, rotation);

            // Update the angle (optional, if you need to track it)
            Angle += turnAmount;
        }

        public void Fire(Vector2 position, float power, float angle)
        {
            Position = position;

            // Set angle and calculate direction
            Angle = angle;
            Matrix4x4 rotation = Matrix4x4.CreateRotationZ(Angle);
            Direction = Vector2.Transform(new Vector2(1.0f, 0.0f), rotation);

            // Set velocity based on direction and power
            Velocity = Direction * power;

            Active = true;
        }

        // Overload that takes a predefined velocity
        public void Fire(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;

            // Calculate direction from velocity (normalized)
            if (Velocity != Vector2.Zero)
            {
                Direction = Vector2.Normalize(Velocity);

                // Calculate angle from direction (if needed)
                Angle = (float)Math.Atan2(Direction.Y, Direction.X);
            }

            Active = true;
        }

        public void Update(float gravity)
        {
            if (Active)
            {
                // Apply gravity
                Velocity = new Vector2(Velocity.X, Velocity.Y + gravity);

                // Update position
                Position = new Vector2(Position.X + Velocity.X, Position.Y + Velocity.Y);

                // Update direction from velocity (optional)
                if (Velocity != Vector2.Zero)
                {
                    Direction = Vector2.Normalize(Velocity);
                }
            }
        }

        public void Draw()
        {
            if (Active)
            {
                Raylib.DrawCircle((int)Position.X, (int)Position.Y, Radius, ScreenColor);

                // Draw 
                for (int i = 1; i <= 5; i++)
                {
                    float trailX = Position.X - (Velocity.X * 0.2f * i);
                    float trailY = Position.Y - (Velocity.Y * 0.2f * i);
                    float trailRadius = Radius * (1.0f - (i * 0.15f));

                    // Fade the trail
                    int alpha = (int)(200 * (1.0f - (i * 0.2f)));
                    Color trailScreenColor = new Color(ScreenColor.R, ScreenColor.G, ScreenColor.B, (byte)alpha);

                    Raylib.DrawCircle((int)trailX, (int)trailY, trailRadius, trailScreenColor);
                }
            }
        }
    }
}

