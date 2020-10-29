using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

// simple collision detection
// player lives / death
// counter
// UI (SpriteFont, imported)

namespace Shmup
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D saucerTxr, missileTxr, backgroundTxr;
        Point screenSize = new Point(800, 450);
        float spawnCooldown = 2;

        Sprite backgroundSprite;
        PlayerSprite playerSprite;
        List<MissileSprite> missiles = new List<MissileSprite>();
        SpriteFont uiFont;
        SpriteFont bigFont;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            saucerTxr = Content.Load<Texture2D>("saucer");
            missileTxr = Content.Load<Texture2D>("missile");
            backgroundTxr = Content.Load<Texture2D>("space");
            uiFont = Content.Load<SpriteFont>("UIFont");
            bigFont = Content.Load<SpriteFont>("BigFont");

            backgroundSprite = new Sprite(backgroundTxr, new Vector2(0, 0));
            playerSprite = new PlayerSprite(saucerTxr, new Vector2(screenSize.X/6, screenSize.Y/2));
        }

        protected override void Update(GameTime gameTime)
        {
            Random rng = new Random();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (spawnCooldown > 0)
            {
                spawnCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (playerSprite.playerLives > 0 && missiles.Count < 5)
            {
                missiles.Add(new MissileSprite(missileTxr, new Vector2(screenSize.X, rng.Next(0, screenSize.Y - missileTxr.Height))));
                spawnCooldown = (float)(rng.NextDouble() + 0.5);
            }

            if (playerSprite.playerLives > 0) playerSprite.Update(gameTime, screenSize);

            foreach (MissileSprite missile in missiles)
            {
                missile.Update(gameTime, screenSize);


                if (playerSprite.playerLives > 0 && playerSprite.IsColliding(missile))
                {
                    missile.dead = true;
                    playerSprite.playerLives--;
                }
            }

            missiles.RemoveAll(missile => missile.dead);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            backgroundSprite.Draw(_spriteBatch);
           if (playerSprite.playerLives> 0) playerSprite.Draw(_spriteBatch);
            
            foreach(MissileSprite missile in missiles) missile.Draw(_spriteBatch);

            _spriteBatch.DrawString(uiFont, "Lives: " + playerSprite.playerLives, new Vector2(10,10), Color.White);
            _spriteBatch.DrawString(uiFont, "Lives: " + playerSprite.playerLives, new Vector2(12, 12), Color.Black);

            if (playerSprite.playerLives <=0)
            {
                Vector2 textSize = bigFont.MeasureString("GAME OVER");
                _spriteBatch.DrawString(bigFont, "GAME OVER", new Vector2((screenSize.X / 2) - (textSize.X / 2), (screenSize.Y / 2) - (textSize.Y / 2)), Color.White);
                _spriteBatch.DrawString(bigFont, "GAME OVER", new Vector2((screenSize.X / 2) - (textSize.X / 2) + 4, (screenSize.Y / 2) - (textSize.Y / 2) + 4), Color.Black);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
