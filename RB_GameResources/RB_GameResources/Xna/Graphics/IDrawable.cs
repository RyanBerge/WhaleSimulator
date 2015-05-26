using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RB_GameResources.Xna.Graphics
{
    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool playAnimation);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime, float layerDepth, bool playAnimation);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffect, float layerDepth, bool playAnimation);
    }
}
