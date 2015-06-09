using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public class Player : Creature
    {
        //Represents the ratio at which the Creature's Speed affects its turning radius --- higher numbers result in wider turns
        //A value of 60 
        private const float ROTATION_RATIO = 5;

        //Represents the turning speed when not moving at all, expressed in Radians Per Second
        private const float BASE_TURN_RADIUS = 0.8f;

        private const float BASE_SPEED = 150f;
        private const float MIN_SPEED = 15f; //8
        private const float MAX_SPEED = 200; //44

        //Expressed in Units Per Second
        private const float ACCELERATION = 10f;
        private const float MANUAL_DECELERATION = 6f;
        private const float BASE_DECELERATION = 0.6f;
        private const float AIR_LOSS = 1f;
        private const float ENERGY_LOSS = 1f;
        private const float AIR_GAIN = 15f;

        private const float ENERGY_FOOD_GAIN = 30f;
        private const float HUNGRY_SOUND_DELAY = 5f;

        public Vector3 Nose { get { return nose; } }
        public float Air { get; set; } // 0 - 100
        public float Energy { get; set; } // 0 - 100

        private Vector3 nose;
        private Sound heartbeat;
        private float hungrySoundTimer = 0f;

        public bool Collision;

        public Player(string species, Vector3 spawnPosition, Vector3 spawnDirection, ContentManager Content) 
            : base(species, "Player", spawnPosition, spawnDirection, true)
        {
            Air = 100;
            Energy = 100;
            // animation stuff
            //SetClip(Clips[0]);
            //player.Looping = true;
        }

        public override void Update(GameTime gameTime, InputStates inputStates)
        {
            if (isUnderwater)
            {
                if (inputStates.NewKeyState.IsKeyDown(Keys.A))
                    Rotations.Y -= ((Speed == 0) ? (float)(BASE_TURN_RADIUS * gameTime.ElapsedGameTime.TotalSeconds) : (1f / (Velocity * ROTATION_RATIO) * (float)gameTime.ElapsedGameTime.TotalSeconds));
                if (inputStates.NewKeyState.IsKeyDown(Keys.D))
                    Rotations.Y += ((Speed == 0) ? (float)(BASE_TURN_RADIUS * gameTime.ElapsedGameTime.TotalSeconds) : (1f / (Velocity * ROTATION_RATIO) * (float)gameTime.ElapsedGameTime.TotalSeconds));

                if (inputStates.NewGPState.ThumbSticks.Left.X != 0)
                {
                    Rotations.Y += (inputStates.NewGPState.ThumbSticks.Left.X * (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (inputStates.NewGPState.ThumbSticks.Left.Y != 0)
                {
                    //Rotations.Z -= (inputStates.NewGPState.ThumbSticks.Left.Y * (float)gameTime.ElapsedGameTime.TotalSeconds) / (Velocity * ROTATION_RATIO);
                    Rotations.Z -= (inputStates.NewGPState.ThumbSticks.Left.Y * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (Rotations.Z < (-Math.PI / 2) + 0.3)
                        Rotations.Z = (float)(-Math.PI / 2f) + 0.3f;
                    else if (Rotations.Z > (Math.PI / 2) - 0.3)
                        Rotations.Z = (float)(Math.PI / 2f) - 0.3f;
                }

                if (inputStates.NewKeyState.IsKeyDown(Keys.W))
                {
                    if (Rotations.Z > (-Math.PI / 2) + 0.3)
                        Rotations.Z -= ((Speed == 0) ? (float)(BASE_TURN_RADIUS * gameTime.ElapsedGameTime.TotalSeconds) : (1f / (Velocity * ROTATION_RATIO) * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }
                if (inputStates.NewKeyState.IsKeyDown(Keys.S))
                {
                    if (Rotations.Z < (Math.PI / 2) - 0.3)
                        Rotations.Z += ((Speed == 0) ? (float)(BASE_TURN_RADIUS * gameTime.ElapsedGameTime.TotalSeconds) : (1f / (Velocity * ROTATION_RATIO) * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }

                if (inputStates.NewKeyState.IsKeyDown(Keys.Space))
                    Speed = BASE_SPEED;
                else if (inputStates.NewGPState.Triggers.Right > 0)
                    Speed += inputStates.NewGPState.Triggers.Right * ACCELERATION;
                else if (inputStates.NewGPState.Triggers.Left > 0)
                    Speed -= inputStates.NewGPState.Triggers.Left * MANUAL_DECELERATION;
                else if (Speed > MIN_SPEED)
                    Speed -= BASE_DECELERATION;
            }

            if (position.Y < Map.WaterLevel - 20)
            {
                Air -= AIR_LOSS * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Air <= 20 && heartbeat == null)
                {
                    heartbeat = Map.soundEngine.GetSound("Heartbeat");
                    heartbeat.Play(true, true);
                }
            }
            else
            {
                Air += AIR_GAIN * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Air > 20 && heartbeat != null)
                {
                    heartbeat.Stop();
                    heartbeat = null;
                }
            }

            Energy -= ENERGY_LOSS * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Energy <= 20)
            {
                if (hungrySoundTimer == 0)
                {
                    Map.soundEngine.Play("HungryWhale", false, true);
                }
                hungrySoundTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (hungrySoundTimer >= HUNGRY_SOUND_DELAY)
                    hungrySoundTimer = 0;
            }
            
            
            if (Air > 100)
                Air = 100;

            if (Energy > 100)
                Energy = 100;

            

            if (Speed >= MAX_SPEED)
                Speed = MAX_SPEED;
            if (Speed < MIN_SPEED)
                Speed = MIN_SPEED;

            if (!isUnderwater)
            {
                Speed -= BASE_DECELERATION;
                if (Rotations.Z > -1)
                    Rotations.Z -= 0.005f;
            }

            float CosZ = (float)Math.Cos(Rotations.Z);
            float CosY = (float)Math.Cos(Rotations.Y);
            float SinZ = (float)Math.Sin(Rotations.Z);
            float SinY = (float)Math.Sin(Rotations.Y);

            movingDirection.X = CosY * CosZ;
            movingDirection.Y = SinZ;
            movingDirection.Z = SinY * CosZ;

            nose = position + (facingDirection * BaseModel.Meshes[0].BoundingSphere.Radius);
            
            CheckFood();

            base.Update(gameTime, inputStates);

            

            //System.Diagnostics.Debug.WriteLine(position);

            //System.Diagnostics.Debug.WriteLine(isUnderwater);
        }

        private void CheckFood()
        {
            
            IEnumerable<Creature> FoodList =
                from food in ChunkGrid.CurrentChunk.Creatures
                where (food.Properties.Family == "Seal" || food.Properties.Family == "Fish")
                select food;
            
            foreach (Creature food in FoodList)
            {
                if (food.Properties.IsAlive)
                {
                    float xd = food.Position.X - nose.X;
                    float yd = food.Position.Y - nose.Y;
                    float zd = food.Position.Z - nose.Z;
                    if (Math.Sqrt(xd * xd + yd * yd + zd * zd) < food.Sphere.Radius)
                    {
                        Eat(food);
                    }
                }
            }

        }

        private void Eat(Creature food)
        {
            food.Despawn();
            Energy += ENERGY_FOOD_GAIN;
            Map.soundEngine.Play("ChompAndGulp", false, true);
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
        public override void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw2D(gameTime, spriteBatch);
        }
    }
}
