using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Controls;
using RB_GameResources.Xna.Graphics;

namespace WhaleSimulator
{
    public class UI
    {
        private ContentManager Content;
        private Texture2D airBarTexture;
        private Rectangle airBar;

        private Texture2D energyBarTexture;
        private Rectangle energyBar;

        private Graphic2D deathScreen;

        public UI(ContentManager content)
        {
            Content = content;
            airBarTexture = new Texture2D(MasterGame.Graphics.GraphicsDevice, 1, 1);
            energyBarTexture = new Texture2D(MasterGame.Graphics.GraphicsDevice, 1, 1);
            Color[] airColor = new Color[1];
            Color[] energyColor = new Color[1];
            airColor[0] = Color.Red;
            energyColor[0] = Color.Green;
            airBarTexture.SetData(airColor);
            energyBarTexture.SetData(energyColor);
            airBar = new Rectangle(50, 50, 200, 20);
            energyBar = new Rectangle(50, 100, 200, 20);

            Animation2D[] tempArray = new Animation2D[1];
            tempArray[0] = new Animation2D("", new Vector2(0, 0), new Vector2(1280, 800), 1, 0);
            deathScreen = new Graphic2D(Content.Load<Texture2D>("Images/DeathScreen"), tempArray, 0, new Vector2(0, 0));
        }

        public void Update(GameTime gameTime, InputStates inputStates, Player player)
        {
            if (Map.PlayerReference.Properties.IsAlive)
            {
                airBar.Width = (int)(player.Air / 100f * 200f);
                energyBar.Width = (int)(player.Energy / 100f * 200f);
            }
        }

        public void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Map.PlayerReference.Properties.IsAlive)
            {
                spriteBatch.Draw(airBarTexture, airBar, Color.White);
                spriteBatch.Draw(energyBarTexture, energyBar, Color.White);
            }
            else
            {
                deathScreen.Draw(spriteBatch, gameTime);
            }
        }

    }
}
