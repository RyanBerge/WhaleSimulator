using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Controls;

namespace WhaleSimulator
{
    public class Map
    {
        private string mapName;
        private Player player;
        private ChunkGrid chunkGrid;
        
        // stuff added
        private Skybox skybox;
        private OceanSurface oceanSurface;

        private float blackOpacity = 1;
        private Rectangle blackRect;
        private Texture2D blackscreen;

        private ContentManager mapContent;

        public Player Player { get { return player; } }
        public static Player PlayerReference { get; set; }
        public static Random Randomizer { get; set; }
        public static int WaterLevel { get; set; }
        public static Vector3 MapSize { get; set; }
        public static Dictionary<string, Model> Models { get; set; }
        public static SoundEngine soundEngine;

        public Map(string name, ContentManager content)
        {
            Randomizer = new Random();
            mapName = name;
            mapContent = content;
            soundEngine = new SoundEngine(mapContent);
            LoadMap();
            player = new Player(chunkGrid.PlayerSpecies, chunkGrid.PlayerSpawn, chunkGrid.SpawnDirection, mapContent);
            PlayerReference = player;
            // stuff added
            skybox = new Skybox(mapContent);
            oceanSurface = new OceanSurface(mapContent);
            blackRect = new Rectangle(0, 0, MasterGame.Graphics.PreferredBackBufferWidth, MasterGame.Graphics.PreferredBackBufferHeight);
            Map.soundEngine.Play("FadeIntoGame", false, false);
        }

        private void LoadMap()
        {
            blackscreen = mapContent.Load<Texture2D>("Images/Blackscreen");
            LoadModelList("Data/Models.xml");
            soundEngine = new SoundEngine(mapContent);
            soundEngine.LoadSounds("Data/Sounds.xml");
            chunkGrid = new ChunkGrid("Data/MapData/" + mapName + ".xml", mapContent);
            Camera.SetDefaults(chunkGrid.PlayerSpawn, chunkGrid.SpawnDirection);
            //chunkGrid.LoadAssets(chunkGrid.SpawnChunk);
            chunkGrid.LoadMap();
            MapSize = chunkGrid.MapSize;
        }

        private void LoadModelList(string filepath)
        {
            XDocument doc;

            try
            {
                using (Stream stream = TitleContainer.OpenStream(filepath))
                {
                    doc = XDocument.Load(stream);
                    stream.Close();
                }

                IEnumerable<XElement> elements = doc.Root.Elements("Model");
                Models = new Dictionary<string, Model>();

                foreach (XElement e in elements)
                {
                    Models.Add(e.Value, mapContent.Load<Model>("Creatures/FBX/" + e.Value));
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        

        public virtual void Update(GameTime gameTime, InputStates inputStates)
        {
            if (blackOpacity > 0)
                blackOpacity -= 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (chunkGrid != null)
                chunkGrid.Update(gameTime, inputStates, ref player);
            if (player != null)
                player.Update(gameTime, inputStates);
            Camera.Update(gameTime, inputStates, player);
            // stuff added
            oceanSurface.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(blackscreen, blackRect, Color.White * blackOpacity);
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public void Draw3D(GameTime gameTime)
        {
            if (chunkGrid != null)
                chunkGrid.Draw3D(gameTime);
            if (player != null)
                player.Draw3D(gameTime);

            // stuff added
            RasterizerState currentState = MasterGame.Graphics.GraphicsDevice.RasterizerState;

            MasterGame.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            skybox.Draw();

            BlendState currentBlend = MasterGame.Graphics.GraphicsDevice.BlendState;

            MasterGame.Graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (Camera.Position.Y > Map.WaterLevel + 10)
                MasterGame.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            else if (Camera.Position.Y < Map.WaterLevel - 10)
                MasterGame.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            else
                MasterGame.Graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            oceanSurface.Draw(Camera.ViewMatrix,
                Camera.ProjectionMatrix, Camera.Position,
                new Vector3(0, 0, 1), MasterGame.Graphics.GraphicsDevice,
                player.Position);

            MasterGame.Graphics.GraphicsDevice.BlendState = currentBlend;

            MasterGame.Graphics.GraphicsDevice.RasterizerState = currentState;
        }

        public void Quit()
        {
            //foreach (Chunk c in chunkGrid)
            //    c.UnloadAssets();
            mapContent.Unload();
        }
    }
}
