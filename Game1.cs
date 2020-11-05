using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Shmup
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D saucerTxr, missileTxr, backgroundTxr, particleTxr;
        Point screenSize = new Point(800, 450);
        float spawnCooldown = 2;
        float playTime = 0;

        Sprite backgroundSprite;
        PlayerSprite playerSprite;
        List<MissileSprite> missiles = new List<MissileSprite>();
        List<ParticleSprite> particles = new List<ParticleSprite>();
        SpriteFont uiFont, bigFont;
        SoundEffect shipExplodeSnd, missileExplodeSnd;

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
            particleTxr = Content.Load<Texture2D>("particle");

            uiFont = Content.Load<SpriteFont>("UIFont");
            bigFont = Content.Load<SpriteFont>("BigFont");

            shipExplodeSnd = Content.Load<SoundEffect>("shipExplode");
            missileExplodeSnd = Content.Load<SoundEffect>("missileExplode");

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
            else if (playerSprite.playerLives > 0 && missiles.Count < (Math.Min(playTime, 120f)/120f) * 30 + 5)
            {
                missiles.Add(new MissileSprite(missileTxr, new Vector2(screenSize.X, rng.Next(0, screenSize.Y - missileTxr.Height)), (Math.Min(playTime, 120f)/120f) * 50000 + 100f));
                spawnCooldown = (float)(rng.NextDouble() + 0.5);
            }

            if (playerSprite.playerLives > 0) 
            {
                playerSprite.Update(gameTime, screenSize);
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            foreach (MissileSprite missile in missiles)
            {
                missile.Update(gameTime, screenSize);


                if (playerSprite.playerLives > 0 && playerSprite.IsColliding(missile))
                {
                    for (int i = 0; i < 16; i++) particles.Add(new ParticleSprite(particleTxr, new Vector2(missile.spritePos.X + (missileTxr.Width / 2) - (particleTxr.Width / 2), (missile.spritePos.Y + (missileTxr.Height / 2) - (particleTxr.Height / 2)))));

                    missile.dead = true;
                    playerSprite.playerLives--;
                    missileExplodeSnd.Play();
                    if (playerSprite.playerLives == 0)
                    {
                        for (int i = 0; i < 32; i++) particles.Add(new ParticleSprite(particleTxr, new Vector2(playerSprite.spritePos.X + (saucerTxr.Width / 2) - (particleTxr.Width / 2), (playerSprite.spritePos.Y + (saucerTxr.Height / 2) - (particleTxr.Height / 2)))));
                        shipExplodeSnd.Play();
                    }
                }
            }

            foreach (ParticleSprite particle in particles) particle.Update(gameTime, screenSize);

            missiles.RemoveAll(missile => missile.dead);
            particles.RemoveAll(particle => particle.currentLife <= 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            backgroundSprite.Draw(_spriteBatch);
           if (playerSprite.playerLives> 0) playerSprite.Draw(_spriteBatch);
            
            foreach(MissileSprite missile in missiles) missile.Draw(_spriteBatch);
            foreach (ParticleSprite particle in particles) particle.Draw(_spriteBatch);

            _spriteBatch.DrawString(uiFont, "Lives: " + playerSprite.playerLives, new Vector2(12, 12), Color.Black);
            _spriteBatch.DrawString(uiFont, "Lives: " + playerSprite.playerLives, new Vector2(10,10), Color.White);

            _spriteBatch.DrawString(uiFont, "Time: " + Math.Round(playTime) + "s", new Vector2(12, 44), Color.Black);
            _spriteBatch.DrawString(uiFont, "Time: " + Math.Round(playTime) + "s", new Vector2(10, 42), Color.White);


            if (playerSprite.playerLives <=0)
            {
                Vector2 textSize = bigFont.MeasureString("GAME OVER");
                _spriteBatch.DrawString(bigFont, "GAME OVER", new Vector2((screenSize.X / 2) - (textSize.X / 2) + 4, (screenSize.Y / 2) - (textSize.Y / 2) + 4), Color.Black);
                _spriteBatch.DrawString(bigFont, "GAME OVER", new Vector2((screenSize.X / 2) - (textSize.X / 2), (screenSize.Y / 2) - (textSize.Y / 2)), Color.White);
                
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
