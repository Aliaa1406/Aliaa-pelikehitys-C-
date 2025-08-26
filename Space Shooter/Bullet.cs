using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Bullet
    {
        private TransformComponent transform;
        private float speed = 400.0f;
        private float lifeTime = 2.0f;
        private float currentLife;
        public bool IsActive  = true;

        public Bullet(Vector2 startPosition, Vector2 direction)
        {
            Vector2 velocity = direction * speed;
            transform = new TransformComponent(startPosition, velocity);
            currentLife = lifeTime;
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            transform.Update(deltaTime);
            transform.WrapAroundScreen(AsteroidsGame.GetScreenWidth(), AsteroidsGame.GetScreenHeight());

            currentLife -= deltaTime;
            if (currentLife <= 0)
                IsActive = false;
        }

        public void Draw()
        {
            if (!IsActive) return;
            Raylib.DrawCircleV(transform.position, 3, Color.White);
        }

        public void Destroy()
        {
            IsActive = false;
        }

        public Vector2 GetPosition() => transform.position;
    }
}
