using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Bullet
    {
        private TransformComponent transform;
        private RenderComponent renderer;
        private float lifetime = 3f;
        private bool isPlayerBullet;

        public bool IsActive { get; private set; } = true;

        public Bullet(Vector2 pos, Vector2 dir, bool fromPlayer)
        {
            isPlayerBullet = fromPlayer;
            transform = new TransformComponent(pos, dir * 400);

            Color bulletColor = fromPlayer ? Color.Yellow : Color.Red;
            renderer = new RenderComponent(3, bulletColor);
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            transform.Update(deltaTime);
            lifetime -= deltaTime;

            if (lifetime <= 0)
                IsActive = false;
        }

        public void Draw()
        {
            if (!IsActive) return;
            renderer.Draw(transform.position);
        }

        public void Destroy() => IsActive = false;
        public Vector2 GetPosition() => transform.position;
        public bool IsPlayerBullet() => isPlayerBullet;
    }
}