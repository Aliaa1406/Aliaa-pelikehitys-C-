using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace TANKS_
{
    internal class Wall
    {
        public Rectangle Bounds { get; private set; }
        public Color Color { get; private set; }

        public Wall(float x, float y, float width, float height)
        {
            Bounds = new Rectangle(x, y, width, height);
            Color = Color.Violet;
        }

        public void Draw()
        {
            Raylib.DrawRectangleRec(Bounds, Color);
        }
    }
}
