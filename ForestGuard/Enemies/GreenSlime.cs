using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGuard.Enemies
{
    public class GreenSlime : CollisionBody
    {
        public static Texture2D[] greenSlimeTextures;
        public static SoundEffect hitSound;

        public const float MoveSpeed = 1.2f;
        public const int SlimeWidth = 70;
        public const int SlimeHeight = 70;

        private int health = 2;
        private int frame = 0;
        private int frameCounter = 0;
        private int direction = 1;
        private int immunityTimer = 0;

        public static void NewGreenSlime(Vector2 position)
        {
            GreenSlime greenSlime = new GreenSlime();
            greenSlime.position = position;
            greenSlime.hitbox = new Rectangle((int)position.X, (int)position.Y, SlimeWidth, SlimeHeight);
            Main.entitiesList.Add(greenSlime);
        }

        public override void Update()
        {
            AnimateSlime();

            if (immunityTimer > 0)
                immunityTimer--;
            if (Player.swingingSword && immunityTimer <= 0)
            {
                DetectManualCollisions(Player.swordHitbox, true);
            }

            Vector2 velocity = Main.player.position - position;
            velocity.Normalize();
            velocity *= MoveSpeed;

            if (Main.player.position.X > position.X)
                direction = 1;
            else
                direction = -1;

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
        }

        public override void HandleManualCollisions(Rectangle collidingRect, bool friendly)
        {
            if (friendly)
            {
                health -= 1;
                immunityTimer = 30;
                hitSound.Play();
                if (health <= 0)
                {
                    DestroyInstance();
                }
            }
        }

        private void AnimateSlime()
        {
            frameCounter++;
            if (frameCounter >= 11)
            {
                frame += 1;
                frameCounter = 0;
                if (frame >= greenSlimeTextures.Length)
                {
                    frame = 0;
                }
            }
        }

        private void DestroyInstance()
        {
            Main.enemiesKilled += 1;
            Main.entitiesList.Remove(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffect = SpriteEffects.None;
            if (direction == -1)
                spriteEffect = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(greenSlimeTextures[frame], position - Main.cameraPosition, null, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
        }
    }
}
