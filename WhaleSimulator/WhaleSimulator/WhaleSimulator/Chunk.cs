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
    public class Chunk
    {
        /// <summary>
        /// Get or set the Chunk immediately to the north of this one.
        /// </summary>
        public Chunk North { get; set; }
        /// <summary>
        /// Get or set the Chunk immediately to the south of this one.
        /// </summary>
        public Chunk South { get; set; }
        /// <summary>
        /// Get or set the Chunk immediately to the east of this one.
        /// </summary>
        public Chunk East { get; set; }
        /// <summary>
        /// Get or set the Chunk immediately to the west of this one.
        /// </summary>
        public Chunk West { get; set; }
        /// <summary>
        /// Get or set the Chunk immediately above this one.
        /// </summary>
        public Chunk Up { get; set; }
        /// <summary>
        /// Get or set the Chunk immediately below this one.
        /// </summary>
        public Chunk Down { get; set; }

        /// <summary>
        /// Get the position of this Chunk in the ChunkGrid.
        /// </summary>
        public Vector3 Position { get; private set; }
        /// <summary>
        /// Get the spawning position of the player in units, relative to the Chunk position, if the player can spawn in this Chunk.
        /// </summary>
        public Vector3 SpawnPosition { get; set; }
        /// <summary>
        /// Gets or sets the direction of the player when spawning.
        /// </summary>
        public Vector3 SpawnDirection { get; set; }
        /// <summary>
        /// Whether or not this Chunk has a spawn point for the player.
        /// </summary>
        public bool HasSpawn { get; set; }
        /// <summary>
        /// The ContentManager responsible for this Chunk's assets.
        /// </summary>
        public ContentManager Content { get; set; }

        public List<CreatureInfo> CreatureList { get; private set; }
        public List<Creature> Creatures { get; private set; }

        /// <summary>
        /// Creates a new Chunk object with a given grid position.  Does not link to other Chunks in the grid.
        /// </summary>
        /// <param name="position"></param>
        public Chunk(Vector3 position)
        {
            Position = position;
        }

        /// <summary>
        /// Takes in an XML subtree and constructs a new Chunk object without spacial linking.
        /// </summary>
        /// <param name="rootElement">The root of the XML subtree to read from.</param>
        public static Chunk FromXML(XElement rootElement)
        {
            try
            {
                XElement element;
                int x = 0;
                int y = 0;
                int z = 0;
                bool isSpawn = false;
                Vector3 position;
                Vector3 spawn = new Vector3();
                Vector3 direction = new Vector3();

                element = rootElement.Element("PositionX");
                if (element != null)
                    x = int.Parse(element.Value);
                else
                    throw new Exception("X-Position not found in XML Data.");

                element = rootElement.Element("PositionY");
                if (element != null)
                    y = int.Parse(element.Value);
                else
                    throw new Exception("Y-Position not found in XML Data.");

                element = rootElement.Element("PositionZ");
                if (element != null)
                    z = int.Parse(element.Value);
                else
                    throw new Exception("Z-Position not found in XML Data.");

                position = new Vector3(x, y, z);

                element = rootElement.Element("Spawn");
                if (element != null)
                {
                    isSpawn = bool.Parse(element.Element("HasSpawn").Value);
                    x = int.Parse(element.Element("PositionX").Value);
                    y = int.Parse(element.Element("PositionY").Value);
                    z = int.Parse(element.Element("PositionZ").Value);
                    spawn = new Vector3(x, y, z);
                    x = int.Parse(element.Element("DirectionX").Value);
                    y = int.Parse(element.Element("DirectionY").Value);
                    z = int.Parse(element.Element("DirectionZ").Value);
                    direction = new Vector3(x, y, z);
                }

                element = rootElement.Element("CreatureList");
                List<CreatureInfo> creatureList = new List<CreatureInfo>();
                if (element != null)
                {
                    IEnumerable<XElement> elements = element.Elements("Creature");
                    foreach(XElement e in elements)
                    {
                        string species = e.Element("Species").Value;
                        x = int.Parse(e.Element("PositionX").Value);
                        y = int.Parse(e.Element("PositionY").Value);
                        z = int.Parse(e.Element("PositionZ").Value);
                        creatureList.Add(new CreatureInfo(species, new Vector3(x + (position.X * 1000), y + (position.Y * 1000), z + (position.Z * 1000)), true));
                    }
                }

                Chunk temp = new Chunk(position);
                temp.CreatureList = creatureList;
                temp.Creatures = new List<Creature>();
                if (isSpawn)
                {
                    temp.HasSpawn = true;
                    if (spawn != null)
                    temp.SpawnPosition = spawn;
                    temp.SpawnDirection = direction;
                }

                return temp;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        /// Update all the objects in this Chunk.
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="inputStates">The InputStates object to use when checking player input.</param>
        public void Update(GameTime gameTime, InputStates inputStates)
        {
            //Update
            if (Creatures != null)
            {
                foreach (Creature c in Creatures)
                    c.Update(gameTime, inputStates);
            }
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public void Draw3D(GameTime gameTime)
        {
            //Draw
            if (Creatures != null)
            {
                foreach (Creature c in Creatures)
                    c.Draw3D(gameTime);
            }
        }

        /// <summary>
        /// Loads the assets associated with this Chunk into memory.
        /// </summary>
        public void LoadAssets()
        {
            Content = MasterGame.GetNewContentManager();
            foreach (CreatureInfo info in CreatureList)
            {
                if (info.IsAlive)
                    Creatures.Add(new Creature(info, Content));
            }
        }

        /// <summary>
        /// Unloads this Chunk's ContentManager,  removing all assets associated with it from memory.
        /// </summary>
        public void UnloadAssets()
        {
            if (Content != null)
            {
                Content.Unload();
                Content.Dispose();
                Content = null;
            }

            if (Creatures.Count > 0)
            {
                foreach (Creature c in Creatures)
                {
                    c.Dispose();
                }
                Creatures.Clear();
                Creatures.TrimExcess();
            }
        }

    }
}
