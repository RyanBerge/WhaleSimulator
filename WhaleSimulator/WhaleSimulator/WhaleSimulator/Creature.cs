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
    public delegate void VoidDelegate();
    
    public class Creature : Graphics3D
    {
        public CreatureInfo Properties { get; set; }
        public float Speed { get; set; }

        private VoidDelegate UpdateMove;

        public Creature(string species, Vector3 spawnPosition, Vector3 spawnDirection, ContentManager Content)
        {
            Properties = new CreatureInfo(species, spawnPosition, spawnDirection, true);
            this.Position = spawnPosition;
            this.Direction = spawnDirection;
            this.Rotations = new Vector3(0, 0, 0);
            this.localUp = new Vector3(0, 1, 0);
            this.OldRotations = new Vector3(0, 0, 0);
            this.BaseModel = Content.Load<Model>("Creatures/" + species);
            Speed = 0;
            SetAI();
        }

        public Creature(CreatureInfo info, ContentManager Content)
        {
            Properties = info;
            this.Position = info.SpawnPosition;
            this.Direction = info.SpawnDirection;
            this.Rotations = new Vector3(0, 0, 0);
            this.localUp = new Vector3(0, 1, 0);
            this.OldRotations = new Vector3(0, 0, 0);
            this.BaseModel = Content.Load<Model>("Creatures/" + Properties.Species);
            Speed = 0;
            SetAI();
        }

        public virtual void Update(GameTime gameTime, InputStates inputStates)
        {
            UpdateMove();

            position.X += (Direction.X * Speed);
            position.Y += (Direction.Y * Speed);
            position.Z += (Direction.Z * Speed);

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

        private void SetAI()
        {
            switch (Properties.Species)
            {
                case "FishSchool": 
                    UpdateMove = FishAI;
                    break;
                case "Boxthingie":
                    UpdateMove = FishAI;
                    break;
                default:
                    UpdateMove = NoAI;
                    break;
            }
        }

        private void NoAI() { }

        private void FishAI()
        {
            if (Speed < 0.05f)
            {
                Speed = (float)(Map.Randomizer.NextDouble()/2);
                Rotations.X += (float)(Map.Randomizer.NextDouble() + 0.5);
                Rotations.Z += (float)(Map.Randomizer.NextDouble() + 0.5);
            }
            else
            {
                Speed -= 0.0002f;
                if (Map.Randomizer.Next(4) == 3)
                {
                    Rotations.X += (float)(Map.Randomizer.NextDouble()/5);
                    Rotations.Z += (float)(Map.Randomizer.NextDouble()/5);
                }
            }
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
        /// The Vector direction the creature will face when spawning.
        /// </summary>
        public Vector3 SpawnDirection { get; set; }
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
        public CreatureInfo(string species, Vector3 spawn, Vector3 direction, bool isAlive) : this()
        {
            Species = species;
            SpawnPosition = spawn;
            SpawnDirection = direction;
            IsAlive = isAlive;
        }
    }
}
