using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Ship
    {
        private TransformComponent transform;
        private List<Bullet> bullets;
        private float acceleration = 300.0f;
        private float maxSpeed = 300.0f;
        private float rotationSpeed = 200.0f;
        private float currentSpeed = 0.0f;
        private float drag = 0.98f;
        private float shootCooldown = 0.3f;
        private float currentCooldown = 0;

        public Ship(Vector2 startPosition)
        {
            transform = new TransformComponent(startPosition);
            bullets = new List<Bullet>();
        }

        public void Update(float deltaTime)
        {
            HandleInput(deltaTime);
            UpdateMovement(deltaTime);
            UpdateBullets(deltaTime);

            transform.WrapAroundScreen(AsteroidsGame.GetScreenWidth(), AsteroidsGame.GetScreenHeight());

            if (currentCooldown > 0)
                currentCooldown -= deltaTime;
        }

        private void HandleInput(float deltaTime)
        {
            // Rotation
            if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A))
                transform.rotation -= rotationSpeed * deltaTime;

            if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D))
                transform.rotation += rotationSpeed * deltaTime;

            // Thrust
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W))
            {
                currentSpeed += acceleration * deltaTime;
                if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;
            }

            if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S))
            {
                currentSpeed -= acceleration * deltaTime;
                if (currentSpeed < 0) currentSpeed = 0;
            }

            // Shooting
            if (Raylib.IsKeyPressed(KeyboardKey.Space) && currentCooldown <= 0)
            {
                Shoot();
                currentCooldown = shootCooldown;
            }
        }

        private void UpdateMovement(float deltaTime)
        {
            Vector2 direction = transform.GetDirectionVector();
            transform.velocity = direction * currentSpeed;
            transform.Update(deltaTime);
            currentSpeed *= drag;
        }

        private void UpdateBullets(float deltaTime)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(deltaTime);
                if (!bullets[i].IsActive)
                    bullets.RemoveAt(i);
            }
        }

        private void Shoot()
        {
            Vector2 direction = transform.GetDirectionVector();
            bullets.Add(new Bullet(transform.position, direction));
        }

         public void Draw()
        {
            var texture = AsteroidsGame.GetPlayerTexture();
            if (texture.Id != 0)
                DrawWithTexture();
            else
                DrawAsTriangle();

            foreach (var bullet in bullets)
                bullet.Draw();
        }


        private void DrawWithTexture()
        {
            var texture = AsteroidsGame.GetPlayerTexture();
            Rectangle sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle destRec = new Rectangle(transform.position.X, transform.position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, transform.rotation, Color.White);
        }

        private void DrawAsTriangle()
        {
            Vector2 direction = transform.GetDirectionVector();
            Vector2 right = new Vector2(-direction.Y, direction.X);

            Vector2 p1 = transform.position + direction * 20.0f;
            Vector2 p2 = transform.position + (right * 15.0f - direction * 15.0f);
            Vector2 p3 = transform.position + (-right * 15.0f - direction * 15.0f);

            Raylib.DrawTriangle(p1, p2, p3, Color.White);
        }

        public Vector2 GetPosition() => transform.position;
        public List<Bullet> GetBullets() => bullets;
    }
}