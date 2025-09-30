using Raylib_cs;

namespace Space_Shooter
{
    internal class SoundSystem
    {
        private Sound shootSound;
        private Sound explosionSound;
        private Sound backgroundMusic;

        public SoundSystem()
        {
            shootSound = Raylib.LoadSound("assets/shooting-star.mp3");
            explosionSound = Raylib.LoadSound("assets/explosion.mp3");
            backgroundMusic = Raylib.LoadSound("assets/space-music.mp3");
        }

        public void PlayShootSound()
        {
            Raylib.PlaySound(shootSound);
        }

        public void PlayExplosionSound()
        {
            Raylib.PlaySound(explosionSound);
        }

        public void PlayBackgroundMusic()
        {
            if (!Raylib.IsSoundPlaying(backgroundMusic))
            {
                Raylib.PlaySound(backgroundMusic);
            }
        }

        public void StopBackgroundMusic()
        {
            Raylib.StopSound(backgroundMusic);
        }

        public void Unload()
        {
            Raylib.UnloadSound(shootSound);
            Raylib.UnloadSound(explosionSound);
            Raylib.UnloadSound(backgroundMusic);
        }
    }




}
