using System.Numerics;
using Raylib_cs;

namespace Space_Shooter
{
    internal class Ship
    {
        private TransformComponent transform;
        private List<Bullet> bullets;

        // Constants for cleaner, more maintainable code
        private const float ACCELERATION = 300.0f;
        private const float MAX_SPEED = 300.0f;
        private const float ROTATION_SPEED = 200.0f;
        private const float DRAG = 0.98f;
        private const float SHOOT_COOLDOWN = 0.3f;
        private const float TRIANGLE_NOSE_SIZE = 20.0f;
        private const float TRIANGLE_WING_SIZE = 15.0f;

        private float currentSpeed = 0.0f;
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
            UpdateCooldown(deltaTime);

            transform.WrapAroundScreen(AsteroidsGame.GetScreenWidth(), AsteroidsGame.GetScreenHeight());
        }

        private void HandleInput(float deltaTime)
        {
            HandleRotationInput(deltaTime);
            HandleThrustInput(deltaTime);
            HandleShootingInput();
        }

        private void HandleRotationInput(float deltaTime)
        {
            if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A))
                transform.rotation -= ROTATION_SPEED * deltaTime;

            if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D))
                transform.rotation += ROTATION_SPEED * deltaTime;
        }

        private void HandleThrustInput(float deltaTime)
        {
            // Forward thrust
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W))
            {
                currentSpeed = MathF.Min(currentSpeed + ACCELERATION * deltaTime, MAX_SPEED);
            }

            // Reverse thrust
            if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S))
            {
                currentSpeed = MathF.Max(currentSpeed - ACCELERATION * deltaTime, 0);
            }
        }

        private void HandleShootingInput()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Space) && currentCooldown <= 0)
            {
                Shoot();
                currentCooldown = SHOOT_COOLDOWN;
            }
        }

        private void UpdateMovement(float deltaTime)
        {
            Vector2 direction = transform.GetDirectionVector();
            transform.velocity = direction * currentSpeed;
            transform.Update(deltaTime);

            // Apply drag to slow down gradually
            currentSpeed *= DRAG;
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

        private void UpdateCooldown(float deltaTime)
        {
            if (currentCooldown > 0)
                currentCooldown -= deltaTime;
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
                DrawWithTexture(texture);
            else
                DrawAsTriangle();

            // Draw all bullets
            foreach (var bullet in bullets)
                bullet.Draw();
        }

        private void DrawWithTexture(Texture2D texture)
        {
            Rectangle sourceRec = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle destRec = new Rectangle(transform.position.X, transform.position.Y, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f); // Use float literal

            Raylib.DrawTexturePro(texture, sourceRec, destRec, origin, transform.rotation, Color.White);
        }

        private void DrawAsTriangle()
        {
            Vector2 direction = transform.GetDirectionVector();
            Vector2 right = new Vector2(-direction.Y, direction.X); // Perpendicular vector for wings

            // Calculate triangle points using constants
            Vector2 p1 = transform.position + direction * TRIANGLE_NOSE_SIZE;  // Nose
            Vector2 p2 = transform.position + (right * TRIANGLE_WING_SIZE - direction * TRIANGLE_WING_SIZE);   // Right wing
            Vector2 p3 = transform.position + (-right * TRIANGLE_WING_SIZE - direction * TRIANGLE_WING_SIZE);  // Left wing

            Raylib.DrawTriangle(p1, p2, p3, Color.White);
        }

        // Public accessors
        public Vector2 GetPosition() => transform.position;
        public List<Bullet> GetBullets() => bullets;
        public float GetCurrentSpeed() => currentSpeed; // Added for potential debugging/UI
        public bool IsOnCooldown() => currentCooldown > 0; // Added for potential UI feedback
    }
}