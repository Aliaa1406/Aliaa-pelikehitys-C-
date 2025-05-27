
using System.Numerics;
using Raylib_cs;

namespace EnemyReader
{
    public class Enemy
    {
        public Vector2 Position;
        public float Radius;
        public Color Color;
        public int Health = 2;
        private float oscillation = 0;

        public Enemy(Vector2 position, float radius, Color color)
        {
            Position = position;
            Radius = radius;
            Color = color;
        }

        public void Update()
        {
            
            oscillation += 0.05f;
            float offset = (float)Math.Sin(oscillation) * 2;
            Position = new Vector2(Position.X, Position.Y + offset);
        }

        public bool CheckHit(Vector2 bombPos)
        {
           
            float distance = Vector2.Distance(Position, bombPos);
            return distance <= Radius;
        }

        public void TakeDamage()
        {
            Health--;

            // Change color when damaged
            if (Health == 1)
            {
                Color = new Color(150, 100, 0, 255); 
            }
        }

        public void Draw()
        {
            if (Health > 0)
            {
                Raylib.DrawCircle((int)Position.X, (int)Position.Y, Radius, Color);

                // Draw health indicator
                string healthText = Health.ToString();
                int textWidth = Raylib.MeasureText(healthText, 16);
                Raylib.DrawText(healthText, (int)(Position.X - textWidth / 2), (int)(Position.Y - 8), 16, Color.White);
            }
        }
    }
}