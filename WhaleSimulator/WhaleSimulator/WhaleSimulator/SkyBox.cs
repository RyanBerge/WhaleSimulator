using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WhaleSimulator
{
    // class that implements a skybox into the program and renders through a custom shader effect
    public class Skybox
    {
        public Model skyBox; // the model of the skybox

        private Texture2D skyBoxTexture; // the skybox texture

        private Effect skyBoxEffect; // the effect that hold information from the shader file

        // loads the model, texture, and hlsl files for the skybox
        public Skybox(ContentManager Content)
        {
            skyBox = Content.Load<Model>("Effects/cube");
            skyBoxTexture = Content.Load<Texture2D>("Effects/oversea");
            skyBoxEffect = Content.Load<Effect>("Effects/Skybox");
        }

        // draws the skybox using the skyboxEffect shader
        public void Draw()
        {   
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        // passes in the variables in the program down to the shader pipeline
                        part.Effect = skyBoxEffect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(Camera.FarClippingPlane*0.9f) * Matrix.CreateTranslation(Camera.Position.X, 950, Camera.Position.Z));
                        part.Effect.Parameters["View"].SetValue(Camera.ViewMatrix);
                        part.Effect.Parameters["Projection"].SetValue(Camera.ProjectionMatrix);
                    
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        
                    }

                    mesh.Draw();

                }
            }  
        }
    }
}
