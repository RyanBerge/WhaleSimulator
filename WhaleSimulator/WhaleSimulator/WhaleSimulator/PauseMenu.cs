using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Graphics;
using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public delegate void DelegateVoid();

    public class PauseMenu : Menu
    {
        public event DelegateVoid Resume;
        public event DelegateVoid Quit;

        //private ContentManager Content;
        private List<MenuButton> buttons;

        private Graphic2D background;

        public PauseMenu(ContentManager content)
        {
            Content = content;
            buttons = new List<MenuButton>();

            LoadMenu();
        }

        public void LoadMenu()
        {
            MenuButton resume = new MenuButton();
            //MenuButton options = new MenuButton();
            MenuButton quit = new MenuButton();

            Animation2D[] tempArray = new Animation2D[2];
            tempArray[0] = new Animation2D("Default", new Vector2(0, 0), new Vector2(400, 100), 1, 0);
            tempArray[1] = new Animation2D("Select", new Vector2(0, 101), new Vector2(400, 100), 1, 0);

            resume.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/ResumeButton"), tempArray, 0, new Vector2(400, 200));
            //options.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/OptionsButton"), tempArray, 0, new Vector2(400, 400));
            quit.Graphic = new Graphic2D(Content.Load<Texture2D>("Images/QuitButton"), tempArray, 0, new Vector2(400, 500));

            tempArray = new Animation2D[1];
            tempArray[0] = new Animation2D("", new Vector2(0, 0), new Vector2(1280, 800), 1, 0);
            background = new Graphic2D(Content.Load<Texture2D>("Images/PauseBackground"), tempArray, 0, new Vector2(0, 0));

            resume.Click = ResumeClick;
            resume.name = "Play";
            resume.up = quit;
            resume.down = quit;

            //options.Click = OptionsClick;
            //options.name = "Options";
            //options.up = resume;
            //options.down = quit;

            quit.Click = ExitClick;
            quit.name = "Exit";
            quit.up = resume;
            quit.down = resume;

            buttons.Add(resume);
            //buttons.Add(options);
            buttons.Add(quit);

            Reset();
        }

        private void ResumeClick()
        {
            if (Resume != null)
                Resume();
        }

        private void OptionsClick()
        {
            //Options
        }

        private void ExitClick()
        {
            if (Quit != null)
                Quit();
        }

        public void Reset()
        {
            foreach (MenuButton button in buttons)
            {
                button.Graphic.CurrentAnimationIndex = 0;
            }
            selectedButton = buttons[0];
            buttons[0].Graphic.CurrentAnimationIndex = 1;
        }

        public override void Update(GameTime gameTime, InputStates inputStates)
        {
            base.Update(gameTime, inputStates);
        }

        public override void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch, gameTime);

            foreach (MenuButton button in buttons)
                button.Graphic.Draw(spriteBatch, gameTime);

            base.Draw2D(gameTime, spriteBatch);
        }
    }
}
