using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WhaleSimulator
{
    public class Skybox
    {
        public Model skyBox;

        private Texture2D skyBoxTexture;

        // private Texture2D underWaterTexture;

        private Effect skyBoxEffect;

        // private float size = 1000f;

        public Skybox(ContentManager Content)
        {
            skyBox = Content.Load<Model>("Effects/cube");
            skyBoxTexture = Content.Load<Texture2D>("Effects/oversea");
            skyBoxEffect = Content.Load<Effect>("Effects/Skybox");
        }

        public void Draw()
        {   
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyBoxEffect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(Camera.FarClippingPlane * 0.9f) * 
                            Matrix.CreateTranslation(Camera.Position.X, Map.WaterLevel, Camera.Position.Z));
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
