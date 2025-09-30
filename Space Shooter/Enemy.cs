using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Enemy
    {
        private TransformComponent transform;
        private RenderComponent renderer;
        private const float SPEED = 100f;
        private Ship targetPlayer;
        private float shootTimer = 0f;
        private const float SHOOT_INTERVAL = 2f;
        private List<Bullet> bullets;

        public bool IsActive { get; private set; } = true;

        public Enemy(Vector2 startPosition, Texture2D texture, Ship player)
        {
            Vector2 velocity = new Vector2(0, SPEED);
            transform = new TransformComponent(startPosition, velocity, 0, 50f);
            targetPlayer = player;
            bullets = new List<Bullet>();

            if (texture.Id == 0)
            {
                renderer = new RenderComponent(25, Color.Gray);
            }
            else
            {
                renderer = new RenderComponent(texture, 25);
            }
        }

        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            transform.Update(deltaTime);
            shootTimer += deltaTime;

            if (shootTimer >= SHOOT_INTERVAL)
            {
                ShootAtPlayer();
                shootTimer = 0f;
            }

            foreach (var bullet in bullets.ToArray())
                bullet.Update(deltaTime);

            bullets.RemoveAll(b => !b.IsActive);

            if (transform.position.Y > AsteroidsGame.SCREEN_HEIGHT + 50)
            {
                IsActive = false;
            }
        }

        private void ShootAtPlayer()
        {
            Vector2 direction = Vector2.Normalize(targetPlayer.GetPosition() - transform.position);
            bullets.Add(new Bullet(transform.position, direction, false));
        }

        public void Draw()
        {
            if (!IsActive) return;

            renderer.Draw(transform.position, transform.rotation);

            foreach (var bullet in bullets)
                bullet.Draw();
        }

        public void Destroy() => IsActive = false;
        public Vector2 GetPosition() => transform.position;
        public List<Bullet> GetBullets() => bullets;
    }
}