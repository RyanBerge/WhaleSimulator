using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public class Map
    {
        private string mapName;
        private Player player;
        private ChunkGrid chunkGrid;
        // stuff added
        private Skybox skybox;
        private OceanSurface oceanSurface;

        private float blackOpacity = 1;
        private Rectangle blackRect;
        private Texture2D blackscreen;

        private ContentManager mapContent;

        public static Random Randomizer { get; set; }
        public static int WaterLevel { get; set; }

        public Map(string name)
        {
            Randomizer = new Random();
            mapName = name;
            mapContent = MasterGame.GetNewContentManager();
            LoadMap();
            player = new Player(chunkGrid.PlayerSpecies, chunkGrid.PlayerSpawn, chunkGrid.SpawnDirection, mapContent);
            
            // stuff added
            skybox = new Skybox(mapContent);
            oceanSurface = new OceanSurface(mapContent);
            blackRect = new Rectangle(0, 0, MasterGame.Graphics.PreferredBackBufferWidth, MasterGame.Graphics.PreferredBackBufferHeight);
            
        }

        private void LoadMap()
        {
            blackscreen = mapContent.Load<Texture2D>("Images/Blackscreen");
            chunkGrid = new ChunkGrid("Data/MapData/" + mapName + ".xml", mapContent);
            Camera.SetDefaults(chunkGrid.PlayerSpawn, chunkGrid.SpawnDirection);
            chunkGrid.LoadAssets(chunkGrid.SpawnChunk);
        }

        public virtual void Update(GameTime gameTime, InputStates inputStates)
        {
            if (blackOpacity > 0)
                blackOpacity -= 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (chunkGrid != null)
                chunkGrid.Update(gameTime, inputStates, player);
            if (player != null)
                player.Update(gameTime, inputStates);
            Camera.Update(gameTime, inputStates, player);
            // stuff added
            oceanSurface.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(blackscreen, blackRect, Color.White * blackOpacity);
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public void Draw3D(GameTime gameTime)
        {
            if (chunkGrid != null)
                chunkGrid.Draw3D(gameTime);
            if (player != null)
                player.Draw3D(gameTime);

            // stuff added
            RasterizerState currentState = MasterGame.Graphics.GraphicsDevice.RasterizerState;

            MasterGame.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            skybox.Draw();

            BlendState currentBlend = MasterGame.Graphics.GraphicsDevice.BlendState;

            MasterGame.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            oceanSurface.Draw(Camera.ViewMatrix,
                Camera.ProjectionMatrix, Camera.Position,
                new Vector3(-1, 0, 0), MasterGame.Graphics.GraphicsDevice,
                player.Position);

            MasterGame.Graphics.GraphicsDevice.BlendState = currentBlend;

            MasterGame.Graphics.GraphicsDevice.RasterizerState = currentState;
        }

        public void Quit()
        {
            foreach (Chunk c in chunkGrid)
                c.UnloadAssets();
            mapContent.Unload();
        }
    }
}
