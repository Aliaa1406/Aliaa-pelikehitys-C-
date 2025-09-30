using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Asteroid
    {
        private TransformComponent transform;
        private RenderComponent renderer;
        private AsteroidSize size;
        private float radius;
        private static readonly float[] RADIUS_BY_SIZE = { 15f, 25f, 40f };

        public bool IsActive { get; private set; } = true;

        public Asteroid(Vector2 pos, Vector2 vel, AsteroidSize asteroidSize, float rotationSpeed)
        {
            size = asteroidSize;
            radius = RADIUS_BY_SIZE[(int)size];
            transform = new TransformComponent(pos, vel, 0, rotationSpeed);

            Texture2D texture = GetAsteroidTexture(size);

            if (texture.Id == 0)
            {
                renderer = new RenderComponent(radius, Color.Brown);
            }
            else
            {
                renderer = new RenderComponent(texture, radius);
            }
        }

        private static Texture2D GetAsteroidTexture(AsteroidSize size)
        {
            return size switch
            {
                AsteroidSize.Large => AsteroidsGame.AsteroidLargeTexture,
                AsteroidSize.Medium => AsteroidsGame.AsteroidMediumTexture,
                AsteroidSize.Small => AsteroidsGame.AsteroidSmallTexture,
                _ => AsteroidsGame.AsteroidLargeTexture
            };
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;
            transform.Update(deltaTime);
        }

        public void Draw()
        {
            if (!IsActive) return;
            renderer.Draw(transform.position, transform.rotation);
        }

        public void Destroy() => IsActive = false;
        public Vector2 GetPosition() => transform.position;
        public float GetRadius() => radius;
        public AsteroidSize GetSize() => size;
    }
}