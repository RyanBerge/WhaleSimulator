using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using RB_GameResources.Xna.Controls;
using RB_GameResources.Xna.Graphics;


namespace WhaleSimulator
{
    public class Menu
    {
        public class MenuButton
        {
            public MenuButton left;
            public MenuButton right;
            public MenuButton up;
            public MenuButton down;

            public VoidDelegate Click;
            public Graphic2D Graphic;
            public string name;
        }

        protected ContentManager Content;
        //protected List<List<MenuButton>> stateList;
        protected MenuButton selectedButton;
        protected SoundEffectInstance menuBoopInstance;
        protected SoundEffectInstance menuBoopInstanceBackup;

        private float switchSelectionThreshold;

        public Menu()
        {
            
        }

        public virtual void Dispose() { Content.Unload(); }

        public virtual void Update(GameTime gameTime, InputStates inputStates)
        {
            switchSelectionThreshold += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((inputStates.WasButtonPressed(Keys.Enter) || inputStates.WasButtonPressed(Buttons.A)) && selectedButton.Click != null)
                selectedButton.Click();

            //Menu navigation
            if (inputStates.WasButtonPressed(Keys.A) || inputStates.WasButtonPressed(Buttons.DPadLeft) ||
                inputStates.WasButtonPressed(Keys.Left) || 
                (inputStates.NewGPState.ThumbSticks.Left.X < -0.3f && switchSelectionThreshold >= 0.3))
            {
                if (selectedButton.left != null)
                {
                    selectedButton.Graphic.CurrentAnimationIndex = 0;
                    selectedButton = selectedButton.left;
                    selectedButton.Graphic.CurrentAnimationIndex = 1;
                    switchSelectionThreshold = 0;
                    if (menuBoopInstance != null)
                        menuBoopInstance.Play();
                }
            }

            if (inputStates.WasButtonPressed(Keys.D) || inputStates.WasButtonPressed(Buttons.DPadRight) ||
                inputStates.WasButtonPressed(Keys.Right) ||
                (inputStates.NewGPState.ThumbSticks.Left.X > 0.3f && switchSelectionThreshold >= 0.3))
            {
                if (selectedButton.right != null)
                {
                    selectedButton.Graphic.CurrentAnimationIndex = 0;
                    selectedButton = selectedButton.right;
                    selectedButton.Graphic.CurrentAnimationIndex = 1;
                    switchSelectionThreshold = 0;
                    if (menuBoopInstance != null)
                        menuBoopInstance.Play();
                }
            }

            if (inputStates.WasButtonPressed(Keys.W) || inputStates.WasButtonPressed(Buttons.DPadUp) ||
                inputStates.WasButtonPressed(Keys.Up) || 
                (inputStates.NewGPState.ThumbSticks.Left.Y > 0.3f && switchSelectionThreshold >= 0.3))
            {
                if (selectedButton.up != null)
                {
                    selectedButton.Graphic.CurrentAnimationIndex = 0;
                    selectedButton = selectedButton.up;
                    selectedButton.Graphic.CurrentAnimationIndex = 1;
                    switchSelectionThreshold = 0;
                    if (menuBoopInstance != null)
                        menuBoopInstance.Play();
                }
            }

            if (inputStates.WasButtonPressed(Keys.S) || inputStates.WasButtonPressed(Buttons.DPadDown) ||
                inputStates.WasButtonPressed(Keys.Down) ||
                (inputStates.NewGPState.ThumbSticks.Left.Y < -0.3f && switchSelectionThreshold >= 0.3))
            {
                if (selectedButton.down != null)
                {
                    selectedButton.Graphic.CurrentAnimationIndex = 0;
                    selectedButton = selectedButton.down;
                    selectedButton.Graphic.CurrentAnimationIndex = 1;
                    switchSelectionThreshold = 0;
                    if (menuBoopInstance != null)
                    {
                        if (menuBoopInstance.State == SoundState.Playing)
                        {
                            menuBoopInstance.Stop();
                            menuBoopInstanceBackup.Play();
                        }
                        else
                            menuBoopInstance.Play();
                    }
                    
                }
            }
            
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public virtual void Draw3D(GameTime gameTime) { }
        /// <summary>
        /// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public virtual void Draw2D(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}