using Raylib_cs;
using System.Numerics;

namespace Space_Shooter
{
    internal class RenderComponent
    {
        private Texture2D texture;
        private float radius;
        private Color color;
        private bool useTexture;

        public RenderComponent(Texture2D tex, float size)
        {
            texture = tex;
            radius = size;
            useTexture = texture.Id != 0;
            color = Color.White;
        }

        public RenderComponent(float size, Color shapeColor)
        {
            radius = size;
            color = shapeColor;
            useTexture = false;
        }

        public void Draw(Vector2 pos, float rotation = 0)
        {
            if (useTexture)
            {
                DrawTexture(pos, rotation);
            }
            else
            {
                DrawShape(pos);
            }
        }

        private void DrawTexture(Vector2 pos, float rotation)
        {
            Raylib.DrawTexturePro(
                texture,
                new Rectangle(0, 0, texture.Width, texture.Height),
                new Rectangle(pos.X, pos.Y, radius * 2, radius * 2),
                new Vector2(radius, radius),
                rotation,
                color
            );
        }

        private void DrawShape(Vector2 pos)
        {
            if (radius > 30)
            {
                Raylib.DrawCircle((int)pos.X, (int)pos.Y, radius, Color.Brown);
                Raylib.DrawCircleLines((int)pos.X, (int)pos.Y, radius, Color.DarkBrown);
            }
            else if (radius > 20)
            {
                Raylib.DrawEllipse((int)pos.X, (int)pos.Y, radius, radius * 0.6f, Color.Gray);
                Raylib.DrawEllipseLines((int)pos.X, (int)pos.Y, radius, radius * 0.6f, Color.Yellow);
            }
            else
            {
                Raylib.DrawCircle((int)pos.X, (int)pos.Y, radius, color);
            }
        }

        public void SetColor(Color newColor)
        {
            color = newColor;
        }
    }
}