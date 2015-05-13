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

        private ContentManager mapContent;

        public static Random Randomizer { get; set; }

        public Map(string name)
        {
            mapName = name;
            mapContent = MasterGame.GetNewContentManager();
            LoadMap();
            player = new Player(chunkGrid.PlayerSpecies, chunkGrid.PlayerSpawn, chunkGrid.SpawnDirection, mapContent);
            Randomizer = new Random();
        }

        private void LoadMap()
        {
            chunkGrid = new ChunkGrid("Data/MapData/" + mapName + ".xml");
            Camera.SetDefaults(chunkGrid.PlayerSpawn, chunkGrid.SpawnDirection);
            chunkGrid[1, 1, 0].LoadAssets();
        }

        public virtual void Update(GameTime gameTime, InputStates inputStates)
        {
            if (chunkGrid != null)
                chunkGrid.Update(gameTime, inputStates);
            if (player != null)
                player.Update(gameTime, inputStates);
            Camera.Update(gameTime, inputStates, player);
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public virtual void Draw3D(GameTime gameTime)
        {
            if (chunkGrid != null)
                chunkGrid.Draw3D(gameTime);
            if (player != null)
                player.Draw3D(gameTime);
        }

        ///// <summary>
        ///// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        ///// </summary>
        ///// <param name="gameTime">The GameTime object to use as reference.</param>
        ///// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        //public virtual void Draw2D(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}
