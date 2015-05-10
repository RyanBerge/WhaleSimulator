using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public class Graphics3D
    {

        public Model BaseModel { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get { return direction; } set { direction = value; } }

        protected Vector3 direction;
        protected Vector3 Rotations;
        protected Vector3 OldRotations;

        private Matrix worldTransformation;


        public Graphics3D()
        {

        }

        /// <summary>
        /// Constructs a new Graphics3D object.
        /// </summary>
        /// <param name="model">The Model asset to use.</param>
        public Graphics3D(Model model)
        {
            BaseModel = model;
        }


        /// <summary>
        /// Sets the currently playing animation to a new animation.
        /// </summary>
        /// <param name="animationName">The name of the animation clip to play.</param>
        public void PlayAnimation(string animationName)
        {

        }

        /// <summary>
        /// Updates the Graphics3D object, advancing the frames of currently playing animations.
        /// </summary>
        /// <param name="gameTime">The gameTime object to use as reference.</param>
        public void Update(GameTime gameTime)
        {
            //x = cos(yaw)*cos(pitch)
            //y = sin(yaw)*cos(pitch)
            //z = sin(pitch)
            if (Rotations != OldRotations)
            {
                direction.X = (float)(Math.Cos(Rotations.Z) * Math.Cos(Rotations.Y));
                direction.Y = (float)(Math.Sin(Rotations.Z));
                direction.Z = (float)(Math.Sin(Rotations.Y) * Math.Cos(Rotations.Z));
                
            }

            worldTransformation = Matrix.CreateWorld(Position, direction, Camera.CameraUp);

            OldRotations = Rotations;
        }

        /// <summary>
        /// Draws the Graphics3D object
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public virtual void Draw3D(GameTime gameTime)
        {
            foreach (ModelMesh mesh in BaseModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldTransformation;
                    effect.View = Camera.ViewMatrix;
                    effect.Projection = Camera.ProjectionMatrix;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

    }
}
