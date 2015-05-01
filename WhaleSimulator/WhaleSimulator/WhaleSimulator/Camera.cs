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
            float x = playerPosition.X - (playerDirection.X * 10);
            float y = playerPosition.Y - (playerDirection.Y * 10);
            float z = playerPosition.Z - (playerDirection.Z * 10);

            Position = new Vector3(x, y, z);
            LookTarget = playerPosition;

            FieldOfView = (float)(Math.PI / 4);
            AspectRatio = MasterGame.AspectRatio;
            NearClippingPlane = 0.1f;
            FarClippingPlane = 100f;

            IsUnderwater = true;
            FogStart = 75;
            FogEnd = 100;
            UnderwaterFogStart = 10;
            UnderwaterFogEnd = 50;
            FogColor = Color.Aqua.ToVector3();

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearClippingPlane, FarClippingPlane);
        }


        public static void Update(GameTime gameTime, InputStates inputStates, Player player)
        {
            float x = player.Position.X - (player.Direction.X * 10);
            float y = player.Position.Y - (player.Direction.Y * 10);
            float z = player.Position.Z - (player.Direction.Z * 10);

            Position = new Vector3(x, y, z);
            LookTarget = player.Position;

            ViewMatrix = Matrix.CreateLookAt(Position, LookTarget, Vector3.Up);
        }


    }
}
