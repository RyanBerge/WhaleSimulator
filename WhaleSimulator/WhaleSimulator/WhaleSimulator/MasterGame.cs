using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MasterGame : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        private InputStates inputStates;
        private GameManager gameManager;

        private static IServiceProvider newServices;

        public static float AspectRatio { get; set; }
        public static VoidDelegate Quit;

        public MasterGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            
        }

        public static ContentManager GetNewContentManager()
        {
            return new ContentManager(MasterGame.newServices, "Content");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            MasterGame.newServices = this.Services;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 800;
            Graphics.ApplyChanges();
            MasterGame.AspectRatio = (float)Graphics.PreferredBackBufferWidth / (float)Graphics.PreferredBackBufferHeight;

            IsFixedTimeStep = false;
            
            Quit = QuitGame;

            inputStates = new InputStates(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
            gameManager = new GameManager();
            gameManager.Initialize();
        }

        private void QuitGame()
        {
            gameManager.Quit();
            Exit();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            inputStates.RefreshNewStates();

            gameManager.Update(gameTime, inputStates);

            base.Update(gameTime);
            inputStates.RotateOldStates(false);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(36,78,155,255));

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            gameManager.Draw3D(gameTime);


            SpriteBatch.Begin();

            gameManager.Draw2D(gameTime, SpriteBatch);

            SpriteBatch.End();

                       
            base.Draw(gameTime);
        }
    }
}