using ForestGuard.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ForestGuard
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static List<Tile> tilesList = new List<Tile>();
        public static List<Decoration> decorationsList = new List<Decoration>();
        public static List<CollisionBody> entitiesList = new List<CollisionBody>();

        public const int MapWidth = 50;
        public const int MapHeight = 50;
        public const int ScreenWidth = 1200;
        public const int ScreenHeight = 800;

        public static Vector2 cameraPosition;
        public static Random random;
        public static GameState gameState = GameState.Title;
        public static int currentRoundNumber = 0;
        public static int enemiesKilled = 0;
        public static SpriteFont mainFont;
        public static Player player;
        public static Rectangle debugRect;

        private Texture2D titleScreenBackground;
        private Texture2D gameOverBackground;

        private bool showingRules = false;
        private int rulesPressTimer = 0;

        private Song titleScreenSong;
        private Song mainGameSong;

        public enum GameState
        {
            Title,
            Playing,
            GameOver
        }

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            random = new Random();

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");
            titleScreenBackground = Content.Load<Texture2D>("Textures/Backgrounds/Background");
            gameOverBackground = Content.Load<Texture2D>("Textures/Backgrounds/GameOverBackground");
            player = new Player();
            player.Initialize();

            Player.walkingFrontTextures = new Texture2D[4];
            for (int i = 0; i < 3; i++)
            {
                Player.walkingFrontTextures[i] = Content.Load<Texture2D>("Textures/Player_Running_Front_" + (i + 1));
            }
            Player.walkingFrontTextures[3] = Content.Load<Texture2D>("Textures/Player_Running_Front_2");

            Player.walkingBackTextures = new Texture2D[4];
            for (int i = 0; i < 3; i++)
            {
                Player.walkingBackTextures[i] = Content.Load<Texture2D>("Textures/Player_Running_Back_" + (i + 1));
            }
            Player.walkingBackTextures[3] = Content.Load<Texture2D>("Textures/Player_Running_Back_2");

            Player.walkingLeftTextures = new Texture2D[4];
            for (int i = 0; i < 3; i++)
            {
                Player.walkingLeftTextures[i] = Content.Load<Texture2D>("Textures/Player_Running_Left_" + (i + 1));
            }
            Player.walkingLeftTextures[3] = Content.Load<Texture2D>("Textures/Player_Running_Left_2");

            Player.walkingRightTextures = new Texture2D[4];
            for (int i = 0; i < 3; i++)
            {
                Player.walkingRightTextures[i] = Content.Load<Texture2D>("Textures/Player_Running_Right_" + (i + 1));
            }
            Player.walkingRightTextures[3] = Content.Load<Texture2D>("Textures/Player_Running_Right_2");

            Player.swordTextures = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                Player.swordTextures[i] = Content.Load<Texture2D>("Textures/Sword_" + (i + 1));
            }

            Player.standingTextures = new Texture2D[4];
            Player.standingTextures[(int)Player.Direction.Front] = Content.Load<Texture2D>("Textures/Player_Standing_Front");
            Player.standingTextures[(int)Player.Direction.Back] = Content.Load<Texture2D>("Textures/Player_Standing_Back");
            Player.standingTextures[(int)Player.Direction.Left] = Content.Load<Texture2D>("Textures/Player_Standing_Left");
            Player.standingTextures[(int)Player.Direction.Right] = Content.Load<Texture2D>("Textures/Player_Standing_Right");

            Tile.grassTile = Content.Load<Texture2D>("Textures/Tiles/GrassTile_1");
            Tile.grassyGrassTile = Content.Load<Texture2D>("Textures/Tiles/GrassTile_2");
            Decoration.treeTexture = Content.Load<Texture2D>("Textures/Tiles/Tree");
            Decoration.rockTexture = Content.Load<Texture2D>("Textures/Tiles/Rock");

            GreenSlime.greenSlimeTextures = new Texture2D[2];
            for (int i = 0; i < 2; i++)
            {
                GreenSlime.greenSlimeTextures[i] = Content.Load<Texture2D>("Textures/Slime_" + (i + 1));
            }

            OrangeSlime.orangeSlimeTextures = new Texture2D[2];
            for (int i = 0; i < 2; i++)
            {
                OrangeSlime.orangeSlimeTextures[i] = Content.Load<Texture2D>("Textures/OrangeSlime_" + (i + 1));
            }


            Player.dirtSteps = new SoundEffect[4];
            for (int i = 0; i < 4; i++)
            {
                Player.dirtSteps[i] = Content.Load<SoundEffect>("Sounds/Step" + (i + 1));
            }
            Player.swordSwingSound = Content.Load<SoundEffect>("Sounds/Sword_Swing");
            Player.deathlyScreamingSound = Content.Load<SoundEffect>("Sounds/PlayerHurtSound");
            OrangeSlime.hitSound = GreenSlime.hitSound = Content.Load<SoundEffect>("Sounds/Slime_Hit");
            titleScreenSong = Content.Load<Song>("Sounds/Music/TitleSong");
            mainGameSong = Content.Load<Song>("Sounds/Music/Main_Chill_Action");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (gameState == GameState.Title)
            {
                if (rulesPressTimer > 0)
                    rulesPressTimer--;

                if (Keyboard.GetState().IsKeyDown(Keys.R) && rulesPressTimer <= 0)
                {
                    rulesPressTimer = 30;
                    showingRules = !showingRules;
                }
                if (MediaPlayer.State != MediaState.Playing)        //Has to be on top of InitGame() or else the music will start again with the title
                {
                    MediaPlayer.Play(titleScreenSong);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    InitializeGame();
                }
            }
            else if (gameState == GameState.Playing)
            {
                CollisionBody[] entitiesListCopy = entitiesList.ToArray();
                foreach (CollisionBody entity in entitiesListCopy)
                {
                    entity.Update();
                }

                if (entitiesList.Count - 1 <= 0)
                {
                    StartNewRound();
                }
                if (player.health <= 0)
                {
                    gameState = GameState.GameOver;
                }
                if (MediaPlayer.State != MediaState.Playing)
                {
                    MediaPlayer.Play(mainGameSong);
                }
            }
            else if (gameState == GameState.GameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    InitializeGame();
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();


            debugRect = Rectangle.Empty;
            if (gameState == GameState.Title)
            {
                _spriteBatch.Draw(titleScreenBackground, Vector2.Zero, null, Color.White);

                _spriteBatch.DrawString(mainFont, "FOREST GUARD", new Vector2((_graphics.PreferredBackBufferWidth / 2) - 480, 40), Color.Black, 0f, Vector2.Zero, 2.6f, SpriteEffects.None, 0f);

                _spriteBatch.DrawString(mainFont, "Press SPACE to start", new Vector2((_graphics.PreferredBackBufferWidth / 2) - 220, 600), Color.Black);

                _spriteBatch.DrawString(mainFont, "Press R to view the rules", new Vector2((_graphics.PreferredBackBufferWidth / 2) - 220, 640), Color.Black);

                if (showingRules)
                {
                    int rectWidth = 1100;
                    int rectHeight = 160;
                    Texture2D rect = new Texture2D(GraphicsDevice, rectWidth, rectHeight);

                    Color[] colors = new Color[rectWidth * rectHeight];
                    for (int i = 0; i < rectWidth * rectHeight; i++)
                    {
                        colors[i] = Color.LightYellow;
                    }
                    rect.SetData(colors);

                    _spriteBatch.Draw(rect, new Vector2(50f, 295f), null, Color.White);

                    Vector2 wasdTextPos = new Vector2(70f, 315f);
                    _spriteBatch.DrawString(mainFont, "Use WASD to move", wasdTextPos, Color.Black);

                    _spriteBatch.DrawString(mainFont, "Use left-click to swing your sword", wasdTextPos + new Vector2(0f, 40f), Color.Black);

                    _spriteBatch.DrawString(mainFont, "Use left-click to swing your sword", wasdTextPos + new Vector2(0f, 80f), Color.Black);

                }
            }
            else if (gameState == GameState.Playing)
            {
                CollisionBody[] entitiesListCopy = entitiesList.ToArray();
                foreach (Tile tile in tilesList)
                {
                    tile.Draw(_spriteBatch);
                }
                foreach (CollisionBody entity in entitiesListCopy)
                {
                    entity.Draw(_spriteBatch);
                }
                foreach (Decoration decoration in decorationsList)
                {
                    decoration.Draw(_spriteBatch);
                }

                _spriteBatch.DrawString(mainFont, "Health:" + player.health, new Vector2(0f, 0f), Color.Black);
                _spriteBatch.DrawString(mainFont, "Round:" + currentRoundNumber, new Vector2(0f, 40f), Color.Black);
                _spriteBatch.DrawString(mainFont, "Enemies Left:" + (entitiesList.Count - 1), new Vector2(0f, 80f), Color.Black);
            }
            else if (gameState == GameState.GameOver)
            {
                _spriteBatch.Draw(gameOverBackground, Vector2.Zero, null, Color.White);

                _spriteBatch.DrawString(mainFont, "Amount of Rounds Passed:" + currentRoundNumber, new Vector2((ScreenWidth / 2) - 240f, 185f), Color.Black);

                _spriteBatch.DrawString(mainFont, "Amount of Enemies Killed:" + enemiesKilled, new Vector2((ScreenWidth / 2) - 236f, 225f), Color.Black);

                _spriteBatch.DrawString(mainFont, "Press SPACE to restart", new Vector2((ScreenWidth / 2) - 220f, 600f), Color.Black);

                _spriteBatch.DrawString(mainFont, "Press ESCAPE to quit", new Vector2((ScreenWidth / 2) - 212f, 640f), Color.Black);
            }

            if (debugRect != Rectangle.Empty)
            {
                if (debugRect.Width <= 0)
                    return;

                int rectWidth = debugRect.Width;
                int rectHeight = debugRect.Height;
                Texture2D rect = new Texture2D(GraphicsDevice, rectWidth, rectHeight);

                Color[] colors = new Color[rectWidth * rectHeight];
                for (int i = 0; i < rectWidth * rectHeight; i++)
                {
                    colors[i] = Color.White;
                }
                rect.SetData(colors);

                _spriteBatch.Draw(rect, new Vector2(debugRect.X, debugRect.Y) - cameraPosition, null, Color.Orange);
            }

            _spriteBatch.End();
        }

        public void InitializeGame()
        {
            tilesList.Clear();
            decorationsList.Clear();
            entitiesList.Clear();
            MediaPlayer.Stop();
            currentRoundNumber = 0;
            enemiesKilled = 0;

            player.health = 4;
            player.position.X = MapWidth * 50 / 2;
            player.position.Y = MapHeight * 50 / 2;
            entitiesList.Add(player);

            GenerateMap();
            StartNewRound();
            gameState = GameState.Playing;
        }

        public void StartNewRound()
        {
            player.health += 1;
            currentRoundNumber += 1;

            for (int i = 0; i < 3 * (int)((currentRoundNumber / 2) + 1); i++)
            {
                Vector2 spawnPos = new Vector2(random.Next(0, 50 * MapWidth), random.Next(0, 50 * MapHeight));
                if (random.Next(0, 200) > currentRoundNumber * 2)
                {
                    GreenSlime.NewGreenSlime(spawnPos);
                }
                else
                {
                    OrangeSlime.NewOrangeSlime(spawnPos);
                }
            }
        }

        public static void UpdateCamera(Vector2 position)
        {
            int halfScreenWidth = ScreenWidth / 2;
            int halfScreenHeight = ScreenHeight / 2;
            cameraPosition = position - new Vector2(halfScreenWidth, halfScreenHeight);

            if (cameraPosition.X < 0f)
            {
                cameraPosition.X = 0f;
            }
            else if (cameraPosition.X > (MapWidth * 50) - ScreenWidth)
            {
                cameraPosition.X = (MapWidth * 50) - ScreenWidth;
            }

            if (cameraPosition.Y < 0f)
            {
                cameraPosition.Y = 0f;
            }
            if (cameraPosition.Y > (MapHeight * 50) - ScreenHeight)
            {
                cameraPosition.Y = (MapHeight * 50) - ScreenHeight;
            }
        }

        public void GenerateMap()
        {
            int row = 0;
            int column = -1;
            for (int i = 0; i < MapWidth * MapHeight; i++)
            {
                Texture2D texture;
                if (random.Next(0, 101) > 2)
                    texture = Tile.grassTile;
                else
                    texture = Tile.grassyGrassTile;

                column++;
                if (column >= MapWidth)
                {
                    row += 1;
                    column = -1;
                }
                Vector2 position = new Vector2(column * 50f, row * 50f);

                Tile tile = new Tile(texture, position);
                tilesList.Add(tile);
            }

            for (int i = 0; i < random.Next(35, 62); i++)
            {
                Decoration decoration = new Decoration();
                decoration.texture = Decoration.treeTexture;
                decoration.position = new Vector2(random.Next(0, MapWidth * 50), random.Next(0, MapHeight * 50));
                decorationsList.Add(decoration);
            }
        }
    }
}
