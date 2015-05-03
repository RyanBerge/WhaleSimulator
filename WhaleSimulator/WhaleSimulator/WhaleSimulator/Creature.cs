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
    public class Creature : Graphics3D
    {

        public CreatureInfo Properties { get; set; }

        public Creature(string species, Vector3 spawnPosition, ContentManager Content)
        {
            Properties = new CreatureInfo(species, spawnPosition, true);
            this.Position = spawnPosition;
            this.BaseModel = Content.Load<Model>("Creatures/" + species);
        }

        public Creature(CreatureInfo info, ContentManager Content)
        {
            Properties = info;
            this.Position = info.SpawnPosition;
            this.BaseModel = Content.Load<Model>("Creatures/" + Properties.Species);
        }

        public virtual void Update(GameTime gameTime, InputStates inputStates) 
        {
            base.Update(gameTime);
        }
        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public override void Draw3D(GameTime gameTime) 
        {
            base.Draw3D(gameTime);
        }
        /// <summary>
        /// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public virtual void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }

        public void Dispose()
        {

        }
    }

    /// <summary>
    /// A simple struct encapsulating non-graphical information about a given Creature.
    /// </summary>
    public struct CreatureInfo
    {
        /// <summary>
        /// The "Species" identifier for the Creature.
        /// </summary>
        public string Species { get; set; }
        /// <summary>
        /// The coordinates relative to the Chunk where the Creature will spawn.
        /// </summary>
        public Vector3 SpawnPosition { get; set; }
        /// <summary>
        /// Whether or not the Creature is still alive.
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// Creates a new CreatureInfo object.
        /// </summary>
        /// <param name="species">The "Species" identifier for the Creature.</param>
        /// <param name="spawn">The coordinates relative to the Chunk where the Creature will spawn.</param>
        /// <param name="isAlive">Whether or not the Creature is still alive.</param>
        public CreatureInfo(string species, Vector3 spawn, bool isAlive) : this()
        {
            Species = species;
            SpawnPosition = spawn;
            IsAlive = isAlive;
        }
    }
}
