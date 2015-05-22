using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WhaleSimulator
{
    class OceanSurface
    {
        public Model surfaceModel;

        private Texture2D surfaceTexture;

        private Texture2D surfaceNormal;

        private Effect oceanSurfaceEffect;

        public float TotalTime;

        public OceanSurface(// string texturePath, string normalPath, 
            ContentManager Content)
        {
            surfaceModel = Content.Load<Model>("Effects/ocean");
            surfaceTexture = Content.Load<Texture2D>(// texturePath
                "Effects/water");
            surfaceNormal = Content.Load<Texture2D>(// normalPath
                "Effects/wavesbump");
            oceanSurfaceEffect = Content.Load<Effect>("Effects/OceanSurface");
            TotalTime = 0;
        }

        public void Update(int ElaspedTimeInMilliseconds)
        {
            TotalTime += ElaspedTimeInMilliseconds / 5000.0f;
        }

        public void Draw(Matrix view, Matrix projection, 
            Vector3 cameraPosition, Vector3 lightDirection, GraphicsDevice graphicsDev, Vector3 playerPos)
        {
            ModelMesh surfaceModelMesh = surfaceModel.Meshes[0];
            ModelMeshPart meshPart = surfaceModelMesh.MeshParts[0];

            //set the vertex source to the mesh's vertex buffer
            graphicsDev.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);

            //set the current index buffer to the sample mesh's index buffer
            graphicsDev.Indices = meshPart.IndexBuffer;

            oceanSurfaceEffect.Parameters["World"].SetValue(
                Matrix.CreateRotationY((float)MathHelper.ToRadians((int)270)) *
                Matrix.CreateRotationZ((float)MathHelper.ToRadians((int)90)) *
                Matrix.CreateScale(100.0f) * Matrix.CreateTranslation(playerPos.X, Map.WaterLevel, playerPos.Z));
            oceanSurfaceEffect.Parameters["View"].SetValue(view);
            oceanSurfaceEffect.Parameters["Projection"].SetValue(projection);
            oceanSurfaceEffect.Parameters["AmbientColor"].SetValue(Color.Gray.ToVector4());
            oceanSurfaceEffect.Parameters["AmbientIntensity"].SetValue(0.4f);
            lightDirection.Normalize();
            oceanSurfaceEffect.Parameters["LightDirection"].SetValue(lightDirection);
            oceanSurfaceEffect.Parameters["DiffuseColor"].SetValue(Color.Gray.ToVector4());
            oceanSurfaceEffect.Parameters["DiffuseIntensity"].SetValue(0.2f);
            oceanSurfaceEffect.Parameters["SpecularColor"].SetValue(Color.Gray.ToVector4());
            oceanSurfaceEffect.Parameters["EyePosition"].SetValue(cameraPosition);
            oceanSurfaceEffect.Parameters["ColorMap"].SetValue(surfaceTexture);
            oceanSurfaceEffect.Parameters["NormalMap"].SetValue(surfaceNormal);
            oceanSurfaceEffect.Parameters["TotalTime"].SetValue(TotalTime);

            // Go through each pass in the effect, but we know there is only one...
            foreach (EffectPass pass in oceanSurfaceEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDev.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0,
                    meshPart.NumVertices, meshPart.StartIndex,
                    meshPart.PrimitiveCount);
            }
        }
    }
}
