﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public class Map
    {
        private string mapName;
        private Player player;
        private ChunkGrid chunkGrid;

        public Map(string name)
        {
            mapName = name;
            player = new Player(GetPlayerType());
            LoadMap();
        }

        public Player.PlayerType GetPlayerType()
        {
            switch (mapName)
            {
                case "Tutorial":
                    return Player.PlayerType.Orca;
                default:
                    return Player.PlayerType.fish;
            }
        }

        private void LoadMap()
        {
            chunkGrid = new ChunkGrid("Data/MapData/" + mapName + ".xml");
        }

        public virtual void Update(GameTime gameTime, InputStates inputStates)
        {
            chunkGrid.Update(gameTime, inputStates);
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public virtual void Draw3D(GameTime gameTime)
        {
            chunkGrid.Draw3D(gameTime);
        }

        ///// <summary>
        ///// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        ///// </summary>
        ///// <param name="gameTime">The GameTime object to use as reference.</param>
        ///// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        //public virtual void Draw2D(GameTime gameTime, SpriteBatch spriteBatch) { }
    }
}
