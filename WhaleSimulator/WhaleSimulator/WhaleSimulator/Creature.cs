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
    public delegate void AIDelegate(GameTime gameTime);
    
    public class Creature : Graphics3D
    {
        public CreatureInfo Properties { get; set; }
        
        /// <summary>
        /// The forward speed of the Creature, expressed in Units Per Second
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// The actual forward speed of the Creature, expressed in Units Per Update
        /// </summary>
        public float Velocity { get; set; }

        private AIDelegate UpdateMove;

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
            UpdateMove(gameTime);

            Velocity = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            position.X += (Direction.X * Velocity);
            position.Y += (Direction.Y * Velocity);
            position.Z += (Direction.Z * Velocity);

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

        private void NoAI(GameTime gameTime) { }

        private void FishAI(GameTime gameTime)
        {
            //======================================================
            //Move constants to classwide scope when done fiddling
            //======================================================

            //Units per second
            const float FISH_MIN_SPEED = 6f;
            const float FISH_MAX_SPEED = 16f;
            const float FISH_SPEED_DECLINE = 0.1f;

            //Radians
            const float FISH_NEW_TURN_MIN = -1f;
            const float FISH_NEW_TURN_MAX = 1f;

            //Lower numbers result in faster turns
            //const int FISH_TURN_FREQUENCY = 2;

            //Radians Per Second
            const float FISH_ROTATION_TURN = 1;

            if (Speed < (FISH_MIN_SPEED))
            {
                Speed = ((float)Map.Randomizer.NextDouble() * (FISH_MAX_SPEED - FISH_MIN_SPEED) + FISH_MIN_SPEED);
                Rotations.Y += ((float)Map.Randomizer.NextDouble() * (FISH_NEW_TURN_MAX - FISH_NEW_TURN_MIN) + FISH_NEW_TURN_MIN);
                Rotations.Z += ((float)Map.Randomizer.NextDouble() * (FISH_NEW_TURN_MAX - FISH_NEW_TURN_MIN) + FISH_NEW_TURN_MIN);
            }
            else
            {
                Speed -= FISH_SPEED_DECLINE;

                Rotations.Z += FISH_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Rotations.Y += FISH_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
