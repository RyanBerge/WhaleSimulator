using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RB_GameResources.Xna.Controls;

using AnimationAux;

namespace WhaleSimulator
{
    public class Graphics3D
    {

        public Model BaseModel { get; set; }
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Direction { get { return direction; } set { direction = value; } }
        public Vector3 LocalUp { get { return localUp; } set { localUp = value; } }

        public BoundingBox Box { get { return BoundingBox.CreateFromSphere(BaseModel.Meshes[0].BoundingSphere); } }
        public BoundingSphere Sphere
        {
            get
            {
                BoundingSphere s = BaseModel.Meshes[0].BoundingSphere;
                return new BoundingSphere(new Vector3(s.Center.X + Position.X, s.Center.Y + Position.Y, s.Center.Z + Position.Z), s.Radius);
            }
        }

        protected Vector3 position;
        protected Vector3 direction;
        protected Vector3 localUp;
        protected Vector3 Rotations;
        protected Vector3 OldRotations;

        private Matrix worldTransformation;

        // purely animation stuff
        protected ModelExtra modelExtra = null; // stuff that gets the information for animation from the model
        protected List<Bone> bones = new List<Bone>(); // list of bones for transforms
        protected AnimationPlayer player = null; // the animation player
        public List<AnimationClip> Clips { get { return modelExtra.Clips; } } // the list of animation clips


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

            modelExtra = model.Tag as ModelExtra;
            ObtainBones();
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

            // Animation stuff
            if (player != null)
                player.Update(gameTime);
        }

        /// <summary>
        /// Draws the Graphics3D object
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public virtual void Draw3D(GameTime gameTime)
        {
            foreach (ModelMesh mesh in BaseModel.Meshes)
            {
                // This had to be a bit redone for the animation stuff

                if (modelExtra == null)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = worldTransformation;
                        effect.View = Camera.ViewMatrix;
                        effect.Projection = Camera.ProjectionMatrix;
                        effect.EnableDefaultLighting();

                        if (Camera.Position.Y < Map.WaterLevel)
                        {
                            effect.FogEnabled = false;
                            effect.FogStart = Camera.UnderwaterFogStart;
                            effect.FogEnd = Camera.UnderwaterFogEnd;
                            effect.FogColor = Camera.FogColor;
                        }
                        else
                        {
                            effect.FogEnabled = false;
                            effect.FogStart = Camera.FogStart;
                            effect.FogEnd = Camera.FogEnd;
                            effect.FogColor = Camera.FogColor;
                        }
                    }
                }
                else
                {
                    //
                    // Compute all of the bone absolute transforms
                    //

                    Matrix[] boneTransforms = new Matrix[bones.Count];

                    for (int i = 0; i < bones.Count; i++)
                    {
                        Bone bone = bones[i];
                        bone.ComputeAbsoluteTransform();

                        boneTransforms[i] = bone.AbsoluteTransform;
                    }

                    //
                    // Determine the skin transforms from the skeleton
                    //

                    Matrix[] skeleton = new Matrix[modelExtra.Skeleton.Count];
                    for (int s = 0; s < modelExtra.Skeleton.Count; s++)
                    {
                        Bone bone = bones[modelExtra.Skeleton[s]];
                        skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
                    }

                    // Draw the model.
                    foreach (Effect effect in mesh.Effects)
                    {
                        if (effect is BasicEffect)
                        {
                            BasicEffect beffect = effect as BasicEffect;
                            beffect.World = boneTransforms[mesh.ParentBone.Index] * worldTransformation;
                            beffect.View = Camera.ViewMatrix;
                            beffect.Projection = Camera.ProjectionMatrix;
                            beffect.EnableDefaultLighting();
                            beffect.PreferPerPixelLighting = true;

                            if (Camera.Position.Y < Map.WaterLevel)
                            {
                                beffect.FogEnabled = false;
                                beffect.FogStart = Camera.UnderwaterFogStart;
                                beffect.FogEnd = Camera.UnderwaterFogEnd;
                                beffect.FogColor = Camera.FogColor;
                            }
                            else
                            {
                                beffect.FogEnabled = false;
                                beffect.FogStart = Camera.FogStart;
                                beffect.FogEnd = Camera.FogEnd;
                                beffect.FogColor = Camera.FogColor;
                            }
                        }

                        if (effect is SkinnedEffect)
                        {
                            SkinnedEffect seffect = effect as SkinnedEffect;
                            seffect.World = boneTransforms[mesh.ParentBone.Index] * worldTransformation;
                            seffect.View = Camera.ViewMatrix;
                            seffect.Projection = Camera.ProjectionMatrix;
                            seffect.EnableDefaultLighting();
                            seffect.PreferPerPixelLighting = true;
                            seffect.SetBoneTransforms(skeleton);
                        }
                    }
                }

                mesh.Draw();
            }
        }

        // Animation stuff down here

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        protected void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone bone in BaseModel.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                bones.Add(newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (Bone bone in bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer SetClip(AnimationClip clip)
        {
            // Create a clip player and assign it to this model
            player = new AnimationPlayer(clip, this);
            return player;
        }

    }
}
