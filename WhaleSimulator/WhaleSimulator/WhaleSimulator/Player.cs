using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public class Player : Creature
    {
        private const float ROTATION_RATIO = 100;

        public Player(string species, Vector3 spawnPosition, Vector3 spawnDirection, ContentManager Content) 
            : base(species, spawnPosition, spawnDirection, Content)
        {
            
        }

        public override void Update(GameTime gameTime, InputStates inputStates)
        {
            if (inputStates.NewKeyState.IsKeyDown(Keys.A))
                Rotations.Y -=  ((Speed == 0) ? 0.015f : 1f/(Speed*100));
            if (inputStates.NewKeyState.IsKeyDown(Keys.D))
                Rotations.Y += ((Speed == 0) ? 0.015f : 1f / (Speed * 100));

            if (inputStates.NewKeyState.IsKeyDown(Keys.W))
            {
                if (Rotations.Z > (-Math.PI/2) + 0.3)
                    Rotations.Z -= ((Speed == 0) ? 0.015f : 1f / (Speed * 100));
            }
            if (inputStates.NewKeyState.IsKeyDown(Keys.S))
            {
                if (Rotations.Z < (Math.PI / 2) - 0.3)
                    Rotations.Z += ((Speed == 0) ? 0.015f : 1f / (Speed * 100));
            }

            if (inputStates.NewKeyState.IsKeyDown(Keys.Space))
                Speed = 1;
            else
                Speed = 0;

            base.Update(gameTime, inputStates);
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
