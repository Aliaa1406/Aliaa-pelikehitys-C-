using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Ship
    {
        private TransformComponent transform;
        private RenderComponent renderer;
        private SoundSystem soundSystem;
        private const float ACCELERATION = 200.0f;
        private const float ROTATION_SPEED = 180.0f;
        private const float FRICTION = 0.98f;
        private List<Bullet> bullets;

        public Ship(Vector2 startPosition, Texture2D texture, SoundSystem soundSystem)
        {
            transform = new TransformComponent(startPosition);
            renderer = new RenderComponent(texture, 20);
            this.soundSystem = soundSystem;
            bullets = new List<Bullet>();
        }

        public void Update(float deltaTime)
        {
            HandleInput(deltaTime);
            transform.Update(deltaTime);

            foreach (var bullet in bullets.ToArray())
                bullet.Update(deltaTime);

            bullets.RemoveAll(b => !b.IsActive);
        }

        private void HandleInput(float deltaTime)
        {
            if (Raylib.IsKeyDown(KeyboardKey.Right))
                transform.rotation += ROTATION_SPEED * deltaTime;

            if (Raylib.IsKeyDown(KeyboardKey.Left))
                transform.rotation -= ROTATION_SPEED * deltaTime;

            if (Raylib.IsKeyDown(KeyboardKey.Up))
                transform.velocity += transform.GetDirectionVector() * ACCELERATION * deltaTime;

            transform.velocity *= FRICTION;

            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                bullets.Add(new Bullet(transform.position, transform.GetDirectionVector(), true));
                soundSystem.PlayShootSound();
            }
        }

        public void Draw()
        {
            renderer.Draw(transform.position, transform.rotation);

            foreach (var bullet in bullets)
                bullet.Draw();
        }

        public Vector2 GetPosition() => transform.position;
        public List<Bullet> GetBullets() => bullets;
    }
}

