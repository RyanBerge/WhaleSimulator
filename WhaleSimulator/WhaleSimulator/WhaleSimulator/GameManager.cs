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
    public enum GameState
    {
        Undefined, MainMenu, PauseMenu, Playing
    }

    public class GameManager
    {
        private GameState gameState = GameState.Undefined;
        
        private MainMenu mainMenu;
        private PauseMenu pauseMenu;
        private Map currentMap;
        private ContentManager mapContent;

        public GameManager()
        {
            mapContent = MasterGame.GetNewContentManager();
            pauseMenu = new PauseMenu(mapContent);
            pauseMenu.Resume += pauseMenuResume;
            pauseMenu.Quit += pauseMenuQuit;
        }

        void pauseMenuQuit()
        {
            LoadMainMenu();

            gameState = GameState.MainMenu;


            currentMap.Quit();
            currentMap = null;
            mapContent = MasterGame.GetNewContentManager();
            pauseMenu = new PauseMenu(mapContent);
            pauseMenu.Resume += pauseMenuResume;
            pauseMenu.Quit += pauseMenuQuit;
            
            
        }

        void pauseMenuResume()
        {
            pauseMenu.Reset();
            gameState = GameState.Playing;
        }

        public void Initialize()
        {
            LoadMainMenu();
        }

        private void LoadMainMenu()
        {
            mainMenu = new MainMenu();
            mainMenu.mapChooseEvent += LoadMap;
            gameState = GameState.MainMenu;
        }

        void LoadMap(string name)
        {
            mainMenu.Dispose();
            currentMap = new Map(name, mapContent);
            gameState = GameState.Playing;
        }

        public void Quit()
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    mainMenu.Dispose();
                    break;
                case GameState.Playing:
                    currentMap.Quit();
                    break;
                case GameState.PauseMenu:
                    //pauseMenu.Dispose();
                    currentMap.Quit();
                    break;
            }
        }

        public void Update(GameTime gameTime, InputStates inputStates)
        {
            switch (gameState)
            {
                case GameState.Undefined:
                    break;
                case GameState.MainMenu:
                    mainMenu.Update(gameTime, inputStates);
                    break;
                case GameState.PauseMenu:
                    pauseMenu.Update(gameTime, inputStates);
                    break;
                case GameState.Playing:
                    if (currentMap != null)
                    {
                        if (inputStates.WasButtonPressed(Buttons.Start) || inputStates.WasButtonPressed(Keys.Escape))
                        {
                            gameState = GameState.PauseMenu;
                        }
                        else
                            currentMap.Update(gameTime, inputStates);
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public void Draw3D(GameTime gameTime)
        {
            if (currentMap != null)
                currentMap.Draw3D(gameTime);
        }

        /// <summary>
        /// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (gameState == GameState.MainMenu)
                mainMenu.Draw2D(gameTime, spriteBatch);
            else if (gameState == GameState.Playing)
                currentMap.Draw2D(spriteBatch, gameTime);
            else if (gameState == GameState.PauseMenu)
                pauseMenu.Draw2D(gameTime, spriteBatch);

        }
    }
}