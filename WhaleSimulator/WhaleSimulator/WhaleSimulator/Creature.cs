﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Controls;

using AnimationAux;

namespace WhaleSimulator
{
    public delegate void VoidDelegate();
    public delegate void AIDelegate(GameTime gameTime);
    
    public class Creature : Graphics3D
    {
        public CreatureInfo Properties { get { return properties; } set { properties = value; } }

        public Vector3 MovingDirection { get { return movingDirection; } set { movingDirection = value; } }
        /// <summary>
        /// The forward speed of the Creature, expressed in Units Per Second
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// The actual forward speed of the Creature, expressed in Units Per Update
        /// </summary>
        public float Velocity { get; set; }


        protected Vector3 movingDirection;
        protected CreatureInfo properties;
        protected bool isUnderwater;
        protected const float GRAVITY = .5f;
        protected float fallingSpeed;

        private AIDelegate UpdateMove;
        //private float bounceTimer = 0;
        private float turnTimer = 10f;

        

        public Creature(string species, string family, Vector3 spawnPosition, Vector3 spawnDirection, bool swims)
        {
            Properties = new CreatureInfo(species, family, spawnPosition, spawnDirection, true, swims);
            this.Position = spawnPosition;
            this.FacingDirection = spawnDirection;
            this.FacingDirection.Normalize();
            this.MovingDirection = spawnDirection;
            this.MovingDirection.Normalize();
            SetRotations();
            this.localUp = new Vector3(0, 1, 0);
            this.OldRotations = new Vector3(0, 0, 0);
            Model outModel;
            try
            {
                bool success = Map.Models.TryGetValue(species, out outModel);
                BaseModel = outModel;
                if (!success)
                    System.Diagnostics.Debug.WriteLine("Model not found: " + species);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            Speed = 0;
            SetAI();

            // animation stuff

            InitializeGraphic();

            modelExtra = BaseModel.Tag as ModelExtra;

            ObtainBones();

            SetClip(Clips[0]);
            player.Looping = true;
        }

        public Creature(CreatureInfo info)
        {
            Properties = info;
            //System.Diagnostics.Debug.WriteLine("Ice Position: " + info.SpawnPosition);
            this.Position = info.SpawnPosition;
            this.FacingDirection = info.SpawnDirection;
            this.FacingDirection.Normalize();
            this.MovingDirection = info.SpawnDirection;
            this.MovingDirection.Normalize();
            SetRotations();
            this.localUp = new Vector3(0, 1, 0);
            this.OldRotations = new Vector3(0, 0, 0);

            Model outModel;
            try
            {
                bool success = Map.Models.TryGetValue(Properties.Species, out outModel);
                BaseModel = outModel;
                if (!success)
                    System.Diagnostics.Debug.WriteLine("Model not found: " + Properties.Species);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            Speed = 0;
            SetAI();

            InitializeGraphic();
        }

        public virtual void Update(GameTime gameTime, InputStates inputStates)
        {

            if (Properties.IsAlive)
            {
                if (Properties.Swims && !isUnderwater)
                    fallingSpeed += GRAVITY;
                else
                {
                    UpdateMove(gameTime);
                    if (fallingSpeed > 0)
                        fallingSpeed -= GRAVITY;
                    else
                        fallingSpeed = 0;
                }

                Velocity = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                position.X += (MovingDirection.X * Velocity);
                position.Y += (MovingDirection.Y * Velocity) - (fallingSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                position.Z += (MovingDirection.Z * Velocity);

                if (position.Y < 0)
                    position.Y = 0;

                if (position.Y > Map.WaterLevel)
                    isUnderwater = false;
                else
                    isUnderwater = true;

                base.Update(gameTime);
            }
            
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public override void Draw3D(GameTime gameTime) 
        {
            if (Properties.Family == "Terrain")
                base.Draw3D(gameTime);
            else if (Properties.IsAlive)
            {
                Vector3 differenceVector = Position - Camera.Position;
                differenceVector.Normalize();

                if (Vector3.Dot(Map.PlayerReference.FacingDirection, differenceVector) >= .5)
                {
                    if (Properties.Family == "Ice")
                        base.Draw3D(gameTime);
                    else
                    {
                        float xd = Position.X - Map.PlayerReference.Position.X;
                        float yd = Position.Y - Map.PlayerReference.Position.Y;
                        float zd = Position.Z - Map.PlayerReference.Position.Z;
                        if (Math.Sqrt(xd * xd + yd * yd + zd * zd) < 1000)
                        {
                            base.Draw3D(gameTime);
                        }
                    }
                }
            }
                
        }
        /// <summary>
        /// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        public virtual void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }

        public void Despawn()
        {
            properties.IsAlive = false;
        }

        public void Dispose()
        {

        }

        private void SetRotations()
        {
            Rotations.Y = (float)Math.Atan(FacingDirection.Z / FacingDirection.X);
            Rotations.Z = (float)Math.Atan(FacingDirection.Y / (Math.Sqrt(FacingDirection.X * FacingDirection.X + FacingDirection.Z * FacingDirection.Z)));
        }

        private void SetAI()
        {
            switch (Properties.Family)
            {
                case "Whale":
                    UpdateMove = LandBirdAI;
                    break;
                case "Fish": 
                    UpdateMove = FishAI;
                    break;
                case "Ice":
                    UpdateMove = IceAI;
                    //UpdateMove = NoAI;
                    break;
                case "StandingBird":
                    UpdateMove = LandBirdAI;
                    break;
                case "FlyingBird":
                    UpdateMove = FlyingAI;
                    break;
                default:
                    UpdateMove = NoAI;
                    break;
            }
        }

        private void NoAI(GameTime gameTime) 
        {
            //if (Sphere.Contains(Map.PlayerReference.Nose) == ContainmentType.Contains)
            //{
            //    Map.PlayerReference.Speed = 0;
            //}
        }

        private void LandBirdAI(GameTime gameTime)
        {
            const float PENGUIN_SPEED = 15f;

            if (turnTimer >= 10f)
            {
                Speed = PENGUIN_SPEED;
                Rotations.Y += ((float)Map.Randomizer.NextDouble() * 2 - 1);

                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinY = (float)Math.Sin(Rotations.Y);

                movingDirection.X = CosY * CosZ;
                movingDirection.Z = SinY * CosZ;
                turnTimer -= 0.1f;
            }
            else if (turnTimer < 0f)
            {
                turnTimer = 10.0f;
            }
            else
            {
                turnTimer -= 0.01f;
            }

            if (Sphere.Contains(Map.PlayerReference.Nose) == ContainmentType.Contains)
            {
                properties.IsAlive = false;
                Map.PlayerReference.Energy += 50;
            }
        }

        private void FlyingAI(GameTime gameTime)
        {
            //Units per second
            const float BIRD_MIN_SPEED = 6f;
            const float BIRD_MAX_SPEED = 16f;
            const float BIRD_SPEED_DECLINE = 0.1f;

            //Radians
            const float BIRD_NEW_TURN_MIN = -1f;
            const float BIRD_NEW_TURN_MAX = 1f;

            //Lower numbers result in faster turns
            //const int BIRD_TURN_FREQUENCY = 2;

            //Radians Per Second
            const float BIRD_ROTATION_TURN = 1;

            if (Speed < (BIRD_MIN_SPEED))
            {
                Speed = ((float)Map.Randomizer.NextDouble() * (BIRD_MAX_SPEED - BIRD_MIN_SPEED) + BIRD_MIN_SPEED);
                Rotations.Y += ((float)Map.Randomizer.NextDouble() * (BIRD_NEW_TURN_MAX - BIRD_NEW_TURN_MIN) + BIRD_NEW_TURN_MIN);
                Rotations.Z += ((float)Map.Randomizer.NextDouble() * (BIRD_NEW_TURN_MAX - BIRD_NEW_TURN_MIN) + BIRD_NEW_TURN_MIN);

                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinZ = (float)Math.Sin(Rotations.Z);
                float SinY = (float)Math.Sin(Rotations.Y);

                movingDirection.X = CosY * CosZ;
                movingDirection.Y = SinZ;
                movingDirection.Z = SinY * CosZ;
            }
            else
            {
                Speed -= BIRD_SPEED_DECLINE;

                Rotations.Z += BIRD_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Rotations.Y += BIRD_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;

                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinZ = (float)Math.Sin(Rotations.Z);
                float SinY = (float)Math.Sin(Rotations.Y);

                movingDirection.X = CosY * CosZ;
                movingDirection.Y = SinZ;
                movingDirection.Z = SinY * CosZ;
            }

            if (Sphere.Contains(Map.PlayerReference.Nose) == ContainmentType.Contains)
            {
                properties.IsAlive = false;
                Map.PlayerReference.Energy += 50;
            }

            if (position.Y < Map.WaterLevel)
                position.Y = Map.WaterLevel;
        }

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

                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinZ = (float)Math.Sin(Rotations.Z);
                float SinY = (float)Math.Sin(Rotations.Y);

                movingDirection.X = CosY * CosZ;
                movingDirection.Y = SinZ;
                movingDirection.Z = SinY * CosZ;
            }
            else
            {
                Speed -= FISH_SPEED_DECLINE;

                Rotations.Z += FISH_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Rotations.Y += FISH_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;

                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinZ = (float)Math.Sin(Rotations.Z);
                float SinY = (float)Math.Sin(Rotations.Y);

                movingDirection.X = CosY * CosZ;
                movingDirection.Y = SinZ;
                movingDirection.Z = SinY * CosZ;
            }

            if (Sphere.Contains(Map.PlayerReference.Nose) == ContainmentType.Contains)
            {
                properties.IsAlive = false;
                Map.PlayerReference.Energy += 20;
            }

            if (Sphere.Contains(Map.PlayerReference.Nose) == ContainmentType.Contains)
            {
                properties.IsAlive = false;
                Map.PlayerReference.Energy += 50;
            }

            if (position.Y > Map.WaterLevel)
                position.Y = Map.WaterLevel;
        }

        private void WhaleAI(GameTime gameTime)
        {
            //======================================================
            //Move constants to classwide scope when done fiddling
            //======================================================

            //Units per second
            const float FISH_MIN_SPEED = 15f;
            const float FISH_MAX_SPEED = 60f;
            const float FISH_SPEED_DECLINE = 0.01f;
            float breathTimer = 60f;

            //Radians
            const float FISH_NEW_TURN_MIN = -1f;
            const float FISH_NEW_TURN_MAX = 1f;

            //Lower numbers result in faster turns
            //const int FISH_TURN_FREQUENCY = 2;

            //Radians Per Second
            const float FISH_ROTATION_TURN = 1;

            breathTimer -= 1f;

            if (Speed < (FISH_MIN_SPEED))
            {
                Speed = ((float)Map.Randomizer.NextDouble() * (FISH_MAX_SPEED - FISH_MIN_SPEED) + FISH_MIN_SPEED);
                Rotations.Y += ((float)Map.Randomizer.NextDouble() * (FISH_NEW_TURN_MAX - FISH_NEW_TURN_MIN) + FISH_NEW_TURN_MIN);
                Rotations.Z += ((float)Map.Randomizer.NextDouble() * (FISH_NEW_TURN_MAX - FISH_NEW_TURN_MIN) + FISH_NEW_TURN_MIN);

                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinZ = (float)Math.Sin(Rotations.Z);
                float SinY = (float)Math.Sin(Rotations.Y);

                movingDirection.X = CosY * CosZ;
                if (breathTimer > 0)
                    movingDirection.Y = SinZ;
                else
                    movingDirection.Y = 1f;
                movingDirection.Z = SinY * CosZ;
            }
            else
            {
                Speed -= FISH_SPEED_DECLINE;

                Rotations.Z += FISH_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Rotations.Y += FISH_ROTATION_TURN * (float)gameTime.ElapsedGameTime.TotalSeconds;

                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinZ = (float)Math.Sin(Rotations.Z);
                float SinY = (float)Math.Sin(Rotations.Y);

                movingDirection.X = CosY * CosZ;
                if (breathTimer > 0)
                    movingDirection.Y = SinZ;
                else
                    movingDirection.Y = 1f;
                movingDirection.Z = SinY * CosZ;
            }

            if (Position.Y == Map.WaterLevel)
                breathTimer = 60f;

            if (Sphere.Contains(Map.PlayerReference.Nose) == ContainmentType.Contains)
            {
                properties.IsAlive = false;
                Map.PlayerReference.Energy += 50;
            }

            if (position.Y > Map.WaterLevel)
                position.Y = Map.WaterLevel;
        }
    
        private void IceAI(GameTime gameTime)
        {
            //Speed = 3;
            //
            //if (bounceTimer <= 0)
            //{
            //    foreach (Chunk chunk in ChunkGrid.LoadedChunks)
            //    {
            //        IEnumerable<Creature> IceList =
            //            from ice in chunk.Creatures
            //            where ice.Properties.Family == "Ice"
            //            select ice;
            //
            //        foreach (Creature ice in IceList)
            //        {
            //            if (ice != this)
            //            {
            //                if (((ice.Position.X - Position.X < 500) && (ice.Position.X - Position.X > -500)) &&
            //                    (((ice.Position.Z - Position.Z < 500) && (ice.Position.Z - Position.Z > -500))))
            //                {
            //                    if (ice.Sphere.Intersects(Sphere))
            //                    {
            //                        Vector2 normal = new Vector2(-MovingDirection.Z, MovingDirection.X);
            //                        Vector2 vector = new Vector2(MovingDirection.X, MovingDirection.Y);
            //                        vector -= 2 * Vector2.Dot(vector, normal) * normal;
            //                        MovingDirection = new Vector3(vector.X, MovingDirection.Y, vector.Y);
            //                        //System.Diagnostics.Debug.WriteLine("Bounce!");
            //                        bounceTimer = 0.5f;
            //                        SetRotations();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //    bounceTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //
            //
            //
            //properties.SpawnPosition = Position;
            //properties.SpawnDirection = MovingDirection;

            if (Sphere.Contains(Map.PlayerReference.Nose) == ContainmentType.Contains)
            {
                Map.PlayerReference.Speed = 0;
                Map.PlayerReference.Position -= 30 * Map.PlayerReference.facingDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
        /// Whether or not the Creature swims underwater.
        /// </summary>
        public bool Swims { get; set; }

        public string Family { get; set; }

        /// <summary>
        /// Creates a new CreatureInfo object.
        /// </summary>
        /// <param name="species">The "Species" identifier for the Creature.</param>
        /// <param name="spawn">The coordinates relative to the Chunk where the Creature will spawn.</param>
        /// <param name="isAlive">Whether or not the Creature is still alive.</param>
        public CreatureInfo(string species, string family, Vector3 spawn, Vector3 direction, bool isAlive, bool swims) : this()
        {
            Species = species;
            SpawnPosition = spawn;
            SpawnDirection = direction;
            IsAlive = isAlive;
            Swims = swims;
            Family = family;
        }
    }
}
