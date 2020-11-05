using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Shmup
{
    class ParticleSprite : Sprite
    {
        Random rng = new Random();
        Vector2 velocity;
        float maxLife;
        public float currentLife;
        Color colour;
        public ParticleSprite(Texture2D newTxr, Vector2 newPos) : base(newTxr, newPos) 
        {
            maxLife = (float)(rng.NextDouble() + 2);
            currentLife = maxLife;

            velocity = new Vector2((float)(rng.NextDouble() * 100 + 50), (float)(rng.NextDouble() * 100 + 50));

            if (rng.Next(2) > 0) velocity.X *= -1;
            if (rng.Next(2) > 0) velocity.Y *= -1;

            colour = new Color((float)(rng.NextDouble() / 2 + 0.5), 0.25f, (float)(rng.NextDouble() / 2 + 0.25));
        }

        public override void Update(GameTime gameTime, Point screenSize)
        {
            spritePos += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentLife -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public new void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(spriteTexture, new Rectangle((int)spritePos.X, (int)spritePos.Y, (int)(spriteTexture.Width * (currentLife / maxLife)), (int)(spriteTexture.Height * (currentLife / maxLife))), colour);
        }
    }
}
