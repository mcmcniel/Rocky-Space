using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace MMcNielMastery2
{
    public class Game1 : Game
    {
        Random rnd = new Random();

        Texture2D backgroundTexture;
        Texture2D backgroundTexture2;
        Texture2D playerTexture;
        Texture2D enemySmall;
        Texture2D enemyMedium;
        Texture2D enemyLarge;

        Vector2 backgroundPosition;
        Vector2 backgroundPosition2;
        Vector2 playerPosition;

        int backgroundSpeed;
        int playerSpeed;
        int maxBullets;
        int currentLevel = 1;
        int numberOfRocks = 1;
        int playerScore = 0;
        int playerLives = 3;
        int winningScore = 2000;

        List<Bullet> playerBullets = new List<Bullet>();
        List<Enemy> enemyList = new List<Enemy>();

        bool canFire = true;

        string gameState = "MAIN";

        SoundEffect laser;
        SoundEffect explosion;

        private SpriteFont font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            backgroundPosition = new Vector2(0, 0);
            backgroundPosition2 = new Vector2(0, -720);

            backgroundSpeed = 100;

            Song song = Content.Load<Song>("music");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            playerPosition = new Vector2((_graphics.PreferredBackBufferWidth / 2), (_graphics.PreferredBackBufferHeight - 85));
            playerSpeed = 500;

            maxBullets = 10;

            for (int i = 0; i < maxBullets; i++)
            {
                Bullet tempBullet = new Bullet();

                tempBullet.bulletPosition = new Vector2(-2000, -2000);

                playerBullets.Add(tempBullet);
            }

            for (int i = 0; i < currentLevel; i++)
            {
                Enemy tempEnemy = new Enemy();

                enemyList.Add(tempEnemy);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("background");
            backgroundTexture2 = Content.Load<Texture2D>("background");

            playerTexture = Content.Load<Texture2D>("player");

            foreach (Bullet bullet in playerBullets)
            {
                bullet.bulletTexture = Content.Load<Texture2D>("projectile");
            }

            laser = Content.Load<SoundEffect>("laser");

            enemySmall = Content.Load<Texture2D>("enemy-small");
            enemyMedium = Content.Load<Texture2D>("enemy-medium");
            enemyLarge = Content.Load<Texture2D>("enemy-large");

            foreach (Enemy enemy in enemyList)
            {
                int tempSize = rnd.Next(3);

                if (tempSize == 0)
                {
                    enemy.enemyTexture = enemySmall;
                    enemy.enemyHealth = 1;
                    enemy.enemyPoints = 100;
                }
                else if (tempSize == 1)
                {
                    enemy.enemyTexture = enemyMedium;
                    enemy.enemyHealth = 2;
                    enemy.enemyPoints = 200;
                }
                else
                {
                    enemy.enemyTexture = enemyLarge;
                }

                Vector2 tempVector = new Vector2();

                tempVector.Y = -(_graphics.PreferredBackBufferHeight + enemy.enemyTexture.Height);
                tempVector.X = rnd.Next(0, _graphics.PreferredBackBufferWidth - enemy.enemyTexture.Width);

                enemy.enemyPosition = tempVector;
                enemy.enemySpeed = rnd.Next(enemy.enemySpeed / 2, enemy.enemySpeed);
                enemy.active = true;
            }

            font = Content.Load<SpriteFont>("font");

            explosion = Content.Load<SoundEffect>("explosion");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            backgroundPosition.Y += backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            backgroundPosition2.Y += backgroundSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (backgroundPosition.Y > backgroundTexture.Height)
            {
                backgroundPosition.Y = -backgroundTexture.Height;
                backgroundPosition2.Y = 0;
            }

            if (backgroundPosition2.Y > backgroundTexture2.Height)
            {
                backgroundPosition2.Y = -backgroundTexture2.Height;
                backgroundPosition.Y = 0;
            }

            var kstate = Keyboard.GetState();

            if (gameState == "MAIN")
            {
                if (kstate.IsKeyDown(Keys.G))
                {
                    ResetGame();
                    gameState = "GAME";
                }

                if (kstate.IsKeyDown(Keys.I))
                {
                    gameState = "INSTRUCTIONS";
                }
            }

            if (gameState == "INSTRUCTIONS")
            {
                if (kstate.IsKeyDown(Keys.G))
                {
                    ResetGame();
                    gameState = "GAME";
                }

                if (kstate.IsKeyDown(Keys.M))
                {
                    gameState = "MAIN";
                }
            }

            if (gameState == "GAME")
            {
                if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
                {
                    playerPosition.Y -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
                {
                    playerPosition.Y += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
                {
                    playerPosition.X += playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
                {
                    playerPosition.X -= playerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (playerPosition.Y > (_graphics.PreferredBackBufferHeight - playerTexture.Height))
                {
                    playerPosition.Y = (_graphics.PreferredBackBufferHeight - playerTexture.Height);
                }
                else if (playerPosition.Y < 0)
                {
                    playerPosition.Y = 0;
                }

                if (playerPosition.X > (_graphics.PreferredBackBufferWidth - playerTexture.Width))
                {
                    playerPosition.X = (_graphics.PreferredBackBufferWidth - playerTexture.Width);
                }
                else if (playerPosition.X < 0)
                {
                    playerPosition.X = 0;
                }

                if (kstate.IsKeyUp(Keys.Space) && (canFire == false))
                {
                    canFire = true;
                }

                if (kstate.IsKeyDown(Keys.Space) && canFire)
                {
                    canFire = false;

                    for (int i = 0; i < playerBullets.Count; i++)
                    {
                        if (playerBullets[i].active == false)
                        {
                            laser.Play();

                            playerBullets[i].active = true;
                            playerBullets[i].bulletPosition = playerPosition;
                            playerBullets[i].bulletPosition.X += (playerTexture.Width / 2) - (playerBullets[i].bulletTexture.Width / 2);
                            playerBullets[i].bulletPosition.Y -= (playerBullets[i].bulletTexture.Height / 2);

                            break;
                        }
                    }
                }

                foreach (Bullet bullet in playerBullets)
                {
                    bullet.bulletPosition.Y -= bullet.bulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (bullet.bulletPosition.Y < -(_graphics.PreferredBackBufferHeight))
                    {
                        bullet.active = false;

                        bullet.bulletPosition = new Vector2(-2000, -2000);
                    }
                }

                foreach (Enemy enemy in enemyList)
                {
                    if (enemy.active)
                    {
                        enemy.enemyPosition.Y += enemy.enemySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        if (enemy.enemyPosition.Y >= _graphics.PreferredBackBufferHeight + enemy.enemyTexture.Height)
                        {
                            enemy.enemySpeed = 400;

                            Vector2 tempVector = new Vector2();

                            tempVector.Y = 0 - enemy.enemyTexture.Height;
                            tempVector.X = rnd.Next(0, (_graphics.PreferredBackBufferWidth - enemy.enemyTexture.Width));

                            enemy.enemyPosition = tempVector;
                            enemy.enemySpeed = rnd.Next(enemy.enemySpeed / 2, enemy.enemySpeed);
                        }
                    }
                }

                foreach (Enemy enemy in enemyList)
                {
                    if (enemy.active)
                    {
                        Rectangle enemyRectangle = new Rectangle((int)enemy.enemyPosition.X,
                            (int)enemy.enemyPosition.Y,
                            enemy.enemyTexture.Width,
                            enemy.enemyTexture.Height);

                        foreach (Bullet bullet in playerBullets)
                        {
                            Rectangle bulletRectangle = new Rectangle((int)bullet.bulletPosition.X,
                                (int)bullet.bulletPosition.Y,
                                bullet.bulletTexture.Width,
                                bullet.bulletTexture.Height);

                            if (enemyRectangle.Intersects(bulletRectangle))
                            {
                                explosion.Play();

                                bullet.active = false;

                                bullet.bulletPosition = new Vector2(-2000, -2000);

                                enemy.enemyHealth--;

                                if (enemy.enemyHealth <= 0)
                                {
                                    playerScore += enemy.enemyPoints;

                                    if (playerScore >= winningScore)
                                    {
                                        gameState = "WIN";
                                    }

                                    enemy.active = false;
                                    enemy.enemyPosition = new Vector2(-4000, -4000);

                                    numberOfRocks--;

                                    if (numberOfRocks <= 0)
                                    {
                                        ResetLevel();
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (Enemy enemy in enemyList)
                {
                    if (enemy.active)
                    {
                        Rectangle enemyRectangle = new Rectangle((int)enemy.enemyPosition.X,
                            (int)enemy.enemyPosition.Y,
                            enemy.enemyTexture.Width,
                            enemy.enemyTexture.Height);

                        Rectangle playerRectangle = new Rectangle((int)playerPosition.X,
                            (int)playerPosition.Y,
                            playerTexture.Width,
                            playerTexture.Height);

                        if (enemyRectangle.Intersects(playerRectangle))
                        {
                            Vector2 tempVector = new Vector2();

                            tempVector.Y = 0 - enemy.enemyTexture.Height;
                            tempVector.X = rnd.Next(0, (_graphics.PreferredBackBufferWidth - enemy.enemyTexture.Width));

                            enemy.enemyPosition = tempVector;

                            explosion.Play();

                            playerLives--;

                            if (playerLives <= 0)
                            {
                                gameState = "LOSE";
                            }
                        }
                    }
                }
            }

            if (gameState == "WIN")
            {
                if (kstate.IsKeyDown(Keys.M))
                {
                    ResetGame();
                    gameState = "MAIN";
                }

                if (kstate.IsKeyDown(Keys.G))
                {
                    ResetGame();
                    gameState = "GAME";
                }

                if (kstate.IsKeyDown(Keys.I))
                {
                    gameState = "INSTRUCTIONS";
                }
            }

            if (gameState == "LOSE")
            {
                if (kstate.IsKeyDown(Keys.M))
                {
                    ResetGame();
                    gameState = "MAIN";
                }

                if (kstate.IsKeyDown(Keys.G))
                {
                    ResetGame();
                    gameState = "GAME";
                }

                if (kstate.IsKeyDown(Keys.I))
                {
                    gameState = "INSTRUCTIONS";
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            _spriteBatch.Draw(backgroundTexture2, backgroundPosition2, Color.White);

            if (gameState == "MAIN")
            {
                _spriteBatch.DrawString(font, "-- ROCKY SPACE --", new Vector2(500, 190), Color.White);
                _spriteBatch.DrawString(font, "Press 'G' to Start the Game", new Vector2(500, 250), Color.White);
                _spriteBatch.DrawString(font, "Press 'I' to View Game Instructions", new Vector2(500, 290), Color.White);
            }

            if (gameState == "INSTRUCTIONS")
            {
                _spriteBatch.DrawString(font, "-- INSTRUCTIONS --", new Vector2(500, 190), Color.White);
                _spriteBatch.DrawString(font, "Destroy asteroids to earn points!", new Vector2(500, 250), Color.White);
                _spriteBatch.DrawString(font, "Avoid getting hit by asteroids.", new Vector2(500, 290), Color.White);
                _spriteBatch.DrawString(font, "Score 2000 points to win!", new Vector2(500, 330), Color.White);
                _spriteBatch.DrawString(font, "W,A,S,D = Move Ship", new Vector2(500, 370), Color.White);
                _spriteBatch.DrawString(font, "Spacebar = Shoot Laser", new Vector2(500, 410), Color.White);

                _spriteBatch.DrawString(font, "Press 'G' to Start the Game", new Vector2(300, 500), Color.White);
                _spriteBatch.DrawString(font, "Press 'M' to Return to the Main Menu", new Vector2(300, 540), Color.White);
            }

            if (gameState == "GAME")
            {
                foreach (Bullet bullet in playerBullets)
                {
                    _spriteBatch.Draw(bullet.bulletTexture, bullet.bulletPosition, Color.White);
                }

                _spriteBatch.Draw(playerTexture, playerPosition, Color.White);

                foreach (Enemy enemy in enemyList)
                {
                    _spriteBatch.Draw(enemy.enemyTexture, enemy.enemyPosition, Color.White);
                }

                _spriteBatch.DrawString(font, "Score: " + playerScore, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font, "Lives: " + playerLives, new Vector2(1190, 10), Color.White);
                _spriteBatch.DrawString(font, "Current Level: " + currentLevel, new Vector2(540, 10), Color.White);
            }

            if (gameState == "WIN")
            {
                _spriteBatch.DrawString(font, "Congratulations! You have won the game!", new Vector2(400, 250), Color.White);
                _spriteBatch.DrawString(font, "Press 'M' to Return to the Main Menu", new Vector2(500, 300), Color.White);
                _spriteBatch.DrawString(font, "Press 'I' to View Game Instructions", new Vector2(500, 340), Color.White);
                _spriteBatch.DrawString(font, "Press 'G' to Restart the Game", new Vector2(500, 380), Color.White);
            }

            if (gameState == "LOSE")
            {
                _spriteBatch.DrawString(font, "GAME OVER! You have lost the game!", new Vector2(400, 250), Color.White);
                _spriteBatch.DrawString(font, "Press 'M' to Return to the Main Menu", new Vector2(500, 300), Color.White);
                _spriteBatch.DrawString(font, "Press 'I' to View Game Instructions", new Vector2(500, 340), Color.White);
                _spriteBatch.DrawString(font, "Press 'G' to Restart the Game", new Vector2(500, 380), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ResetLevel()
        {
            currentLevel++;

            numberOfRocks = currentLevel;

            enemyList = new List<Enemy>();

            for (int i = 0; i < currentLevel; i++)
            {
                Enemy tempEnemy = new Enemy();

                enemyList.Add(tempEnemy);
            }

            foreach (Enemy enemy in enemyList)
            {
                int tempSize = rnd.Next(3);

                if (tempSize == 0)
                {
                    enemy.enemyTexture = enemySmall;
                    enemy.enemyHealth = 1;
                    enemy.enemyPoints = 100;
                }
                else if (tempSize == 1)
                {
                    enemy.enemyTexture = enemyMedium;
                    enemy.enemyHealth = 2;
                    enemy.enemyPoints = 200;
                }
                else
                {
                    enemy.enemyTexture = enemyLarge;
                }

                Vector2 tempVector = new Vector2();

                tempVector.Y = -(_graphics.PreferredBackBufferHeight + enemy.enemyTexture.Height);
                tempVector.X = rnd.Next(0, _graphics.PreferredBackBufferWidth - enemy.enemyTexture.Width);

                enemy.enemyPosition = tempVector;
                enemy.enemySpeed = rnd.Next(enemy.enemySpeed / 2, enemy.enemySpeed);
                enemy.active = true;
            }
        }

        public void ResetGame()
        {
            playerScore = 0;
            playerLives = 3;

            currentLevel = 0;

            playerPosition = new Vector2((_graphics.PreferredBackBufferWidth / 2), (_graphics.PreferredBackBufferHeight - 85));

            foreach (Bullet bullet in playerBullets)
            {
                bullet.active = false;
                bullet.bulletPosition = new Vector2(-2000, -2000);
            }

            ResetLevel();
        }
    }

    public class Bullet
    {
        public Texture2D bulletTexture;

        public Vector2 bulletPosition;

        public float bulletSpeed = 800;

        public bool active = false;
    }

    public class Enemy
    {
        public Texture2D enemyTexture;

        public Vector2 enemyPosition;

        public int enemyHealth = 3;
        public int enemyPoints = 300;
        public int enemySpeed = 400;

        public bool active = false;
    }
}
