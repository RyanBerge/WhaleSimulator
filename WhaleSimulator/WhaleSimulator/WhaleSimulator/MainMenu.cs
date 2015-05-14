using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RB_GameResources.Xna.Controls;
using RB_GameResources.Xna.Graphics;

namespace WhaleSimulator
{
    public class MainMenu : Menu
    {
        public event MapChooseEvent mapChooseEvent;

        private enum MenuState
        {
            Main, Levels
        }

        private MenuState state;
        private List<MenuButton> MainButtonList;
        private List<MenuButton> LevelButtonList;

        public MainMenu()
        {
            MainButtonList = new List<MenuButton>();
            LevelButtonList = new List<MenuButton>();

            //stateList = new List<List<MenuButton>>();
            //stateList.Add(MainButtonList);
            //stateList.Add(LevelButtonList);

            Content = MasterGame.GetNewContentManager();

            LoadMenu();
        }

        private void LoadMenu()
        {
            MenuButton play = new MenuButton();
            //MenuButton load = new MenuButton();
            MenuButton options = new MenuButton();
            MenuButton quit = new MenuButton();

            Animation2D[] tempArray = new Animation2D[2];
            tempArray[0] = new Animation2D("Default", new Vector2(0,0), new Vector2(400, 100), 1, 0);
            tempArray[1] = new Animation2D("Select", new Vector2(0,100), new Vector2(400, 100), 1, 0);

            play.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/PlayButton"), tempArray, 0, new Vector2(400, 200));
            options.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/OptionsButton"), tempArray, 0, new Vector2(400, 400));
            quit.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/QuitButton"), tempArray, 0, new Vector2(400, 600));

            play.Graphic.CurrentAnimationIndex = 1;
            play.Click = PlayClick;
            play.name = "Play";
            play.up = quit;
            play.down = options;

            options.Click = OptionsClick;
            options.name = "Options";
            options.up = play;
            options.down = quit;

            quit.Click = ExitClick;
            quit.name = "Exit";
            quit.up = options;
            quit.down = play;
            

            MainButtonList.Add(play);
            MainButtonList.Add(options);
            MainButtonList.Add(quit);
            selectedButton = play;
        }

        private void PlayClick()
        {
            //System.Diagnostics.Debug.WriteLine("Play Click!");
            if (mapChooseEvent != null)
                mapChooseEvent("Tutorial");
        }

        private void OptionsClick()
        {
            System.Diagnostics.Debug.WriteLine("Options was clicked!");
        }

        private void ExitClick()
        {
            System.Diagnostics.Debug.WriteLine("Exit was clicked!");
        }

        public override void Update(GameTime gameTime, InputStates inputStates)
        {
            base.Update(gameTime, inputStates);
            //if (mapChooseEvent != null)
            //    mapChooseEvent("Tutorial");
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public override void Draw3D(GameTime gameTime) 
        {
        }

        /// <summary>
        /// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public override void Draw2D(GameTime gameTime, SpriteBatch spriteBatch) 
        {
            foreach (MenuButton button in MainButtonList)
                button.Graphic.Draw(spriteBatch, gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
