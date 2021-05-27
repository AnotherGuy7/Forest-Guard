using ForestGuard.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ForestGuard
{
    public class Player : CollisionBody
    {
        public static Texture2D[] standingTextures;
        public static Texture2D[] walkingFrontTextures;
        public static Texture2D[] walkingBackTextures;
        public static Texture2D[] walkingLeftTextures;
        public static Texture2D[] walkingRightTextures;
        public static SoundEffect[] dirtSteps;

        public static Texture2D[] swordTextures;
        public static SoundEffect swordSwingSound;
        public static SoundEffect deathlyScreamingSound;
        public static Rectangle swordHitbox;
        public static bool swingingSword = false;

        public static Texture2D[] currentAnimationArray = walkingFrontTextures;

        public const float MoveSpeed = 3.4f;
        public const int PlayerWidth = 60;
        public const int PlayerHeight = 112;

        public int health = 5;

        private int frame = 0;
        private int frameCounter = 0;
        private int swordFrame = 0;
        private int swordFrameCounter = 0;
        private int swordSwingTimer = 0;
        private float swordRotation = 0f;
        private int immunityTimer = 0;
        private bool walking = false;
        private Direction direction;

        public enum Direction
        {
            Front,
            Back,
            Left,
            Right
        }

        public void Initialize()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, PlayerWidth, PlayerHeight);
        }

        public override void Update()
        {
            if (immunityTimer > 0)
                immunityTimer -= 1;
            if (swordSwingTimer > 0)
                swordSwingTimer -= 1;
            DetectCollisions();

            KeyboardState keyboardState = Keyboard.GetState();

            walking = false;
            Vector2 velocity = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W) && position.Y > 0f)
            {
                walking = true;
                velocity.Y -= MoveSpeed;
                direction = Direction.Back;
                currentAnimationArray = walkingBackTextures;
            }
            if (keyboardState.IsKeyDown(Keys.A) && position.X > 0f)
            {
                walking = true;
                velocity.X -= MoveSpeed;
                direction = Direction.Left;
                currentAnimationArray = walkingLeftTextures;
            }
            if (keyboardState.IsKeyDown(Keys.S) && position.Y + PlayerHeight < Main.MapHeight * 50f)
            {
                walking = true;
                velocity.Y += MoveSpeed;
                direction = Direction.Front;
                currentAnimationArray = walkingFrontTextures;
            }
            if (keyboardState.IsKeyDown(Keys.D) && position.X + PlayerWidth < Main.MapWidth * 50f)
            {
                walking = true;
                velocity.X += MoveSpeed;
                direction = Direction.Right;
                currentAnimationArray = walkingRightTextures;
            }

            if (!walking)
            {
                currentAnimationArray = new Texture2D[1] { standingTextures[(int)direction] };
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && swordSwingTimer <= 0)
            {
                swingingSword = true;
                swordFrame = 0;
                swordFrameCounter = 0;
                swordSwingTimer = 23;
                swordSwingSound.Play();
                Vector2 offset = Vector2.Zero;
                if (direction == Direction.Front)
                {
                    offset.Y += 40f;
                    swordRotation = MathHelper.ToRadians(0f);
                }
                else if (direction == Direction.Back)
                {
                    offset.Y -= 60f;
                    swordRotation = MathHelper.ToRadians(180f);
                }
                else if (direction == Direction.Left)
                {
                    offset.X -= 60f;
                    swordRotation = MathHelper.ToRadians(90f);
                }
                else if (direction == Direction.Right)
                {
                    offset.X += 55f;
                    swordRotation = MathHelper.ToRadians(270f);
                }
                swordHitbox = new Rectangle(hitbox.X + (int)offset.X, hitbox.Y + (int)offset.Y, hitbox.Width, hitbox.Height);
            }
            swingingSword = swordSwingTimer > 0;

            AnimatePlayer();
            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            Main.UpdateCamera(position);
        }

        public override void HandleCollisions(CollisionBody collider)
        {
            if (immunityTimer <= 0 && (collider is GreenSlime || collider is OrangeSlime))
            {
                health -= 1;
                immunityTimer = 30;
                deathlyScreamingSound.Play();
            }
        }

        private void AnimatePlayer()
        {
            frameCounter++;
            if (frameCounter >= 11)
            {
                frame += 1;
                frameCounter = 0;
            }

            if (frame >= currentAnimationArray.Length)
            {
                frame = 0;
            }

            if (walking && (frame == 0 || frame == 2) && frameCounter <= 0)
            {
                dirtSteps[Main.random.Next(0, dirtSteps.Length)].Play();
            }

            if (swordSwingTimer > 0)
            {
                swordFrameCounter++;
                if (swordFrameCounter >= 6)
                {
                    swordFrame += 1;
                    swordFrameCounter = 0;

                    if (swordFrame >= swordTextures.Length)
                    {
                        swordFrame = 0;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (frame >= currentAnimationArray.Length)
            {
                frame = 0;
            }

            spriteBatch.Draw(currentAnimationArray[frame], position - Main.cameraPosition, Color.White);

            if (swordSwingTimer > 0)
            {
                Vector2 swordPos = new Vector2(swordHitbox.X + 32f, swordHitbox.Y + 40f);
                Vector2 swordOrigin = new Vector2(swordTextures[0].Width / 2f, swordTextures[0].Height / 2f);
                spriteBatch.Draw(swordTextures[swordFrame], swordPos - Main.cameraPosition, null, Color.White, swordRotation, swordOrigin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
