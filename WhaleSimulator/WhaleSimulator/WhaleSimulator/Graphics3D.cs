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
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Direction { get { return direction; } set { direction = value; } }
        public Vector3 LocalUp { get { return localUp; } set { localUp = value; } }

        protected Vector3 position;
        protected Vector3 direction;
        protected Vector3 localUp;
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
            Direction = new Vector3(1, 0, 0);
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
            if (Rotations != OldRotations)
            {
                float CosZ = (float)Math.Cos(Rotations.Z);
                float CosY = (float)Math.Cos(Rotations.Y);
                float SinZ = (float)Math.Sin(Rotations.Z);
                float SinY = (float)Math.Sin(Rotations.Y);

                direction.X = CosY * CosZ;
                direction.Y = SinZ;
                direction.Z = SinY * CosZ;

                if (this.GetType() == typeof(Player))
                {
                    float CosZ90 = (float)(Math.Cos(Rotations.Z + (Math.PI / 2)));
                    float SinZ90 = (float)(Math.Sin(Rotations.Z + (Math.PI / 2)));

                    localUp.X = CosZ90 * CosY;
                    localUp.Y = SinZ90;
                    localUp.Z = SinY * CosZ90;
                }
                
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

                    effect.FogEnabled = false;
                    effect.FogStart = Camera.UnderwaterFogStart;
                    effect.FogEnd = Camera.UnderwaterFogEnd;
                    effect.FogColor = Camera.FogColor;

                }

                mesh.Draw();
            }
        }

    }
}
