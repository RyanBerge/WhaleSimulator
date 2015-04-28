using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public class Player : Creature
    {
        public enum PlayerType
        {
            fish, Orca, SpermWhale, BlueWhale
        }

        private PlayerType playerType;

        public Player(PlayerType type)
        {
            playerType = type;
        }

        public override void Update(GameTime gameTime, InputStates inputStates)
        {
            base.Update(gameTime);
        }
        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public override void Draw3D(GameTime gameTime)
        {
            base.Draw3D(gameTime);
        }
        /// <summary>
        /// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public override void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw2D(gameTime, spriteBatch);
        }
    }
}
