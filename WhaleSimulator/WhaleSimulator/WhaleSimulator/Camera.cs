using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public static class Camera
    {
        public static Vector3 Position { get; set; }
        public static Vector3 LookTarget { get; set; }
        public static Vector3 CameraUp { get; set; }
        public static Matrix ViewMatrix { get; set; }
        public static Matrix ProjectionMatrix { get; set; }

        public static float FieldOfView { get; set; }
        public static float AspectRatio { get; set; }
        public static float NearClippingPlane { get; set; }
        public static float FarClippingPlane { get; set; }

        public static bool IsUnderwater { get; set; }
        public static float FogStart { get; set; }
        public static float FogEnd { get; set; }
        public static Vector3 FogColor { get; set; }

        public static float UnderwaterFogStart { get; set; }
        public static float UnderwaterFogEnd { get; set; }

        public static void SetDefaults(Vector3 playerPosition, Vector3 playerDirection)
        {
            float x = playerPosition.X - (playerDirection.X * 75);
            float y = playerPosition.Y - (playerDirection.Y * 75);
            float z = playerPosition.Z - (playerDirection.Z * 75);

            CameraUp = new Vector3(0, 1, 0);
            Position = new Vector3(x, y, z) + (new Vector3(CameraUp.X, CameraUp.Y, CameraUp.Z) * 30);
            LookTarget = new Vector3(playerPosition.X, playerPosition.Y, playerPosition.Z) + (new Vector3(playerDirection.X, playerDirection.Y, playerDirection.Z) * 30);
            

            FieldOfView = (float)(Math.PI / 4);
            AspectRatio = MasterGame.AspectRatio;
            //AspectRatio = 1;
            NearClippingPlane = 0.1f;
            FarClippingPlane = 1000f;

            IsUnderwater = true;
            FogStart = 75;
            FogEnd = 100;
            UnderwaterFogStart = 80;
            UnderwaterFogEnd = 500;
            //FogColor = Color.Blue.ToVector3();
            FogColor = new Vector3(0, (120f/255f), 1f);

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearClippingPlane, FarClippingPlane);
            ViewMatrix = Matrix.CreateLookAt(Position, LookTarget, Vector3.Up);
        }


        public static void Update(GameTime gameTime, InputStates inputStates, Player player)
        {
            float x = player.Position.X - (player.Direction.X * 75);
            float y = player.Position.Y - (player.Direction.Y * 75);
            float z = player.Position.Z - (player.Direction.Z * 75);

            //CameraUp = player.LocalUp;
            Position = new Vector3(x, y, z) + (new Vector3(player.LocalUp.X, player.LocalUp.Y, player.LocalUp.Z) * 30);
            LookTarget = new Vector3(player.Position.X, player.Position.Y, player.Position.Z) + (new Vector3(player.Direction.X, player.Direction.Y, player.Direction.Z) * 30);



            ViewMatrix = Matrix.CreateLookAt(Position, LookTarget, Vector3.Up);
        }


    }
}
