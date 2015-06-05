using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using RB_GameResources.Xna.Controls;
using RB_GameResources.Xna.Graphics;

namespace WhaleSimulator
{
    public class MainMenu : Menu
    {
        public event MapChooseEvent mapChooseEvent;

        private SoundEffect titleMusic;

        private SoundEffectInstance titleMusicInstance;

        private enum MenuState
        {
            Main, Levels
        }

        //private MenuState state;
        private List<MenuButton> MainButtonList;
        private List<MenuButton> LevelButtonList;

        private Graphic2D backgroundLeft;
        private Graphic2D backgroundRight;
        private Texture2D blackscreen;

        private const float BLACK_DELAY = 1;

        private bool sliding = false;
        private float slideSpeed = 0; //Pixels per Second
        private int slideAcceleration = 1000; //Pixels per Second
        private float slideTotal = 0;
        private bool slidingRight = false;
        private bool slidingDown = false;
        private int slideMax;
        private float opacity = 0;
        private Rectangle blackscreenRect;
        private float blackDelayTimer = 0;

        public MainMenu()
        {
            MainButtonList = new List<MenuButton>();
            LevelButtonList = new List<MenuButton>();

            Content = MasterGame.GetNewContentManager();

            LoadMenu();
        }

        private void LoadMenu()
        {

            titleMusic = Content.Load<SoundEffect>("sounds/TitleScreenSong");

            titleMusicInstance = titleMusic.CreateInstance();
            titleMusicInstance.IsLooped = true;
            
            MenuButton play = new MenuButton();
            MenuButton options = new MenuButton();
            MenuButton quit = new MenuButton();

            MenuButton orca = new MenuButton();
            MenuButton back = new MenuButton();

            Animation2D[] tempArray = new Animation2D[2];
            tempArray[0] = new Animation2D("Default", new Vector2(0,0), new Vector2(400, 100), 1, 0);
            tempArray[1] = new Animation2D("Select", new Vector2(0,101), new Vector2(400, 100), 1, 0);

            play.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/PlayButton"), tempArray, 0, new Vector2(400, 200));
            options.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/OptionsButton"), tempArray, 0, new Vector2(400, 400));
            quit.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/QuitButton"), tempArray, 0, new Vector2(400, 600));

            tempArray = new Animation2D[1];
            tempArray[0] = new Animation2D("", new Vector2(0, 0), new Vector2(1280, 1600), 1, 0);
            backgroundLeft = new Graphic2D(Content.Load<Texture2D>("Images/BackgroundLeft"), tempArray, 0, new Vector2(0, 0));
            backgroundRight = new Graphic2D(Content.Load<Texture2D>("Images/BackgroundRight"), tempArray, 0, new Vector2(0, 0));
            blackscreen = Content.Load<Texture2D>("Images/Blackscreen");
            blackscreenRect = new Rectangle(0, 0, MasterGame.Graphics.PreferredBackBufferWidth, MasterGame.Graphics.PreferredBackBufferHeight);


            Animation2D[] orcaAnimations = new Animation2D[2];
            orcaAnimations[0] = new Animation2D("Default", new Vector2(0, 0), new Vector2(428, 339), 1, 0);
            orcaAnimations[1] = new Animation2D("Select", new Vector2(0, 340), new Vector2(428, 339), 1, 0);
            orca.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/OrcaButton"), orcaAnimations, 0, new Vector2(39, 42));

            Animation2D[] backAnimations = new Animation2D[2];
            backAnimations[0] = new Animation2D("Default", new Vector2(0, 0), new Vector2(297, 100), 1, 0);
            backAnimations[1] = new Animation2D("Select", new Vector2(0, 101), new Vector2(297, 100), 1, 0);
            back.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/BackButton"), backAnimations, 0, new Vector2(949, 653));

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

            orca.Click = OrcaClick;
            orca.name = "Orca";
            orca.down = back;
            orca.right = back;

            back.Click = BackClick;
            back.name = "Back";
            back.up = orca;
            back.left = orca;
            

            MainButtonList.Add(play);
            MainButtonList.Add(options);
            MainButtonList.Add(quit);
            selectedButton = play;

            LevelButtonList.Add(orca);
            LevelButtonList.Add(back);

            Vector2 coords = new Vector2(backgroundRight.Coordinates.X, backgroundRight.Coordinates.Y);
            coords.X += MasterGame.Graphics.PreferredBackBufferWidth;
            backgroundRight.Coordinates = coords;

            coords = new Vector2(orca.Graphic.Coordinates.X, orca.Graphic.Coordinates.Y);
            coords.X += MasterGame.Graphics.PreferredBackBufferWidth;
            orca.Graphic.Coordinates = coords;

            coords = new Vector2(back.Graphic.Coordinates.X, back.Graphic.Coordinates.Y);
            coords.X += MasterGame.Graphics.PreferredBackBufferWidth;
            back.Graphic.Coordinates = coords;

            //sound stuff
            titleMusicInstance.Play();

        }

        private void PlayClick()
        {
            //state = MenuState.Levels;
            sliding = true;
            slidingRight = false;
            slideMax = MasterGame.Graphics.PreferredBackBufferWidth;
            selectedButton.Graphic.CurrentAnimationIndex = 0;
        }

        private void OptionsClick()
        {
            System.Diagnostics.Debug.WriteLine("Options was clicked!");
        }

        private void ExitClick()
        {
            MasterGame.Quit();
        }

        private void OrcaClick()
        {
            sliding = true;
            slidingDown = true;
            slideMax = MasterGame.Graphics.PreferredBackBufferHeight;
            selectedButton.Graphic.CurrentAnimationIndex = 0;

            //if (mapChooseEvent != null)
            //    mapChooseEvent("OrcaLevel");
        }

        private void BackClick()
        {
            //state = MenuState.Main;
            sliding = true;
            slidingRight = true;
            selectedButton.Graphic.CurrentAnimationIndex = 0;
        }

        public override void Update(GameTime gameTime, InputStates inputStates)
        {
            if (slidingDown)
            {
                blackDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (blackDelayTimer >= BLACK_DELAY)
                    opacity += 0.6f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (opacity >= 1 && blackDelayTimer >= BLACK_DELAY*2)
                {
                    if (mapChooseEvent != null)
                        mapChooseEvent("OrcaLevel");
                }
            }

            if (!sliding)
                base.Update(gameTime, inputStates);
            else
            {
                if (slideTotal < (slideMax / 2) + 2)
                    slideSpeed += (slideAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds);
                else
                    slideSpeed -= (slideAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds);

                if (slideSpeed < 0)
                    slideSpeed = 10;

                float currentSlide = (slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                slideTotal += currentSlide;
                if (slideTotal > slideMax)
                {
                    float slideOffset = slideTotal - slideMax;
                    slideTotal -= slideOffset;
                    currentSlide -= slideOffset;
                    sliding = false;
                    if (slidingRight)
                    {
                        selectedButton = MainButtonList[0];
                        selectedButton.Graphic.CurrentAnimationIndex = 1;
                    }
                    else
                    {
                        selectedButton = LevelButtonList[0];
                        selectedButton.Graphic.CurrentAnimationIndex = 1;
                    }
                    slideTotal = 0;
                    slideSpeed = 0;
                }

                if (slidingDown)
                {
                    Vector2 coords = new Vector2(backgroundLeft.Coordinates.X, backgroundLeft.Coordinates.Y - currentSlide);
                    backgroundLeft.Coordinates = coords;

                    coords = new Vector2(backgroundRight.Coordinates.X, backgroundRight.Coordinates.Y - currentSlide);
                    backgroundRight.Coordinates = coords;

                    foreach (MenuButton button in MainButtonList)
                    {
                        coords = new Vector2(button.Graphic.Coordinates.X, button.Graphic.Coordinates.Y - currentSlide);
                        button.Graphic.Coordinates = coords;
                    }

                    foreach (MenuButton button in LevelButtonList)
                    {
                        coords = new Vector2(button.Graphic.Coordinates.X, button.Graphic.Coordinates.Y - currentSlide);
                        button.Graphic.Coordinates = coords;
                    }
                }
                else if (slidingRight)
                {
                    Vector2 coords = new Vector2(backgroundLeft.Coordinates.X + currentSlide, backgroundLeft.Coordinates.Y);
                    backgroundLeft.Coordinates = coords;

                    coords = new Vector2(backgroundRight.Coordinates.X + currentSlide, backgroundRight.Coordinates.Y);
                    backgroundRight.Coordinates = coords;

                    foreach (MenuButton button in MainButtonList)
                    {
                        coords = new Vector2(button.Graphic.Coordinates.X + currentSlide, button.Graphic.Coordinates.Y);
                        button.Graphic.Coordinates = coords;
                    }

                    foreach (MenuButton button in LevelButtonList)
                    {
                        coords = new Vector2(button.Graphic.Coordinates.X + currentSlide, button.Graphic.Coordinates.Y);
                        button.Graphic.Coordinates = coords;
                    }
                }
                else
                {
                    Vector2 coords = new Vector2(backgroundLeft.Coordinates.X - currentSlide, backgroundLeft.Coordinates.Y);
                    backgroundLeft.Coordinates = coords;

                    coords = new Vector2(backgroundRight.Coordinates.X - currentSlide, backgroundRight.Coordinates.Y);
                    backgroundRight.Coordinates = coords;

                    foreach (MenuButton button in MainButtonList)
                    {
                        coords = new Vector2(button.Graphic.Coordinates.X - currentSlide, button.Graphic.Coordinates.Y);
                        button.Graphic.Coordinates = coords;
                    }

                    foreach (MenuButton button in LevelButtonList)
                    {
                        coords = new Vector2(button.Graphic.Coordinates.X - currentSlide, button.Graphic.Coordinates.Y);
                        button.Graphic.Coordinates = coords;
                    }
                }
                
            }
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
            
            backgroundLeft.Draw(spriteBatch, gameTime);
            backgroundRight.Draw(spriteBatch, gameTime);
            

            foreach (MenuButton button in MainButtonList)
                button.Graphic.Draw(spriteBatch, gameTime);
            foreach (MenuButton button in LevelButtonList)
                button.Graphic.Draw(spriteBatch, gameTime);

            spriteBatch.Draw(blackscreen, blackscreenRect, Color.White * opacity);
            
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
