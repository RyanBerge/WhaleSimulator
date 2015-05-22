using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using RB_GameResources.Xna.Controls;


namespace WhaleSimulator
{
    /// <summary>
    /// A data structure that stores map "chunks" and allows the traversing through them, while loading and unloading as needed.
    /// </summary>
    public class ChunkGrid : IEnumerable
    {
        private Chunk spawnChunk;
        private Chunk currentChunk;
        private Chunk rootChunk;

        private ContentManager mapContent;

        private List<Graphics3D> globalTerrain;

        private Vector3 mapSize;

        private bool initialized = false;

        public Chunk SpawnChunk { get { return spawnChunk; } set { spawnChunk = value; } }
        public Vector3 PlayerSpawn { get; set; }
        public Vector3 SpawnDirection { get; set; }
        public string PlayerSpecies { get; set; }
        public int WaterLevel { get; set; }

        public static Vector3 MapCenter { get; set; }

        /// <summary>
        /// Creates a new ChunkGrid by loading data from the designated filepath.
        /// </summary>
        /// <param name="filepath">The path to the XML data file.</param>
        public ChunkGrid(string filepath, ContentManager content)
        {
            mapContent = content;
            LoadFromXML(filepath);
            MapCenter = new Vector3((mapSize.X * 1000) / 2, (mapSize.Y * 1000) / 2, (mapSize.Z * 1000) / 2);
            Map.WaterLevel = WaterLevel;
            foreach (Graphics3D g in globalTerrain)
            {
                Vector3 position = MapCenter;
                position.Y = 0;
                g.Position = position;
            }
        }

        public void LoadAssets(Vector3 centerChunk)
        {
            LoadAssets(this[(int)centerChunk.X, (int)centerChunk.Y, (int)centerChunk.Z]);
        }

        public void LoadAssets(Chunk centerChunk)
        {
            if (centerChunk != null)
            {
                centerChunk.LoadAssets();
                if (centerChunk.North != null)
                {
                    centerChunk.North.LoadAssets();
                    if (centerChunk.North.East != null)
                        centerChunk.North.East.LoadAssets();
                    if (centerChunk.North.West != null)
                        centerChunk.North.West.LoadAssets();
                }
                if (centerChunk.South != null)
                {
                    centerChunk.South.LoadAssets();
                    if (centerChunk.South.East != null)
                        centerChunk.South.East.LoadAssets();
                    if (centerChunk.South.West != null)
                        centerChunk.South.West.LoadAssets();
                }
                if (centerChunk.East != null)
                    centerChunk.East.LoadAssets();
                if (centerChunk.West != null)
                    centerChunk.West.LoadAssets();

                if (centerChunk.Up != null)
                {
                    centerChunk.Up.LoadAssets();
                    if (centerChunk.Up.North != null)
                    {
                        centerChunk.Up.North.LoadAssets();
                        if (centerChunk.Up.North.East != null)
                            centerChunk.Up.North.East.LoadAssets();
                        if (centerChunk.Up.North.West != null)
                            centerChunk.Up.North.West.LoadAssets();
                    }
                    if (centerChunk.Up.South != null)
                    {
                        centerChunk.Up.South.LoadAssets();
                        if (centerChunk.Up.South.East != null)
                            centerChunk.Up.South.East.LoadAssets();
                        if (centerChunk.Up.South.West != null)
                            centerChunk.Up.South.West.LoadAssets();
                    }
                    if (centerChunk.Up.East != null)
                        centerChunk.Up.East.LoadAssets();
                    if (centerChunk.Up.West != null)
                        centerChunk.Up.West.LoadAssets();
                }

                if (centerChunk.Down != null)
                {
                    centerChunk.Down.LoadAssets();
                    if (centerChunk.Down.North != null)
                    {
                        centerChunk.Down.North.LoadAssets();
                        if (centerChunk.Down.North.East != null)
                            centerChunk.Down.North.East.LoadAssets();
                        if (centerChunk.Down.North.West != null)
                            centerChunk.Down.North.West.LoadAssets();
                    }
                    if (centerChunk.Down.South != null)
                    {
                        centerChunk.Down.South.LoadAssets();
                        if (centerChunk.Down.South.East != null)
                            centerChunk.Down.South.East.LoadAssets();
                        if (centerChunk.Down.South.West != null)
                            centerChunk.Down.South.West.LoadAssets();
                    }
                    if (centerChunk.Down.East != null)
                        centerChunk.Down.East.LoadAssets();
                    if (centerChunk.Down.West != null)
                        centerChunk.Down.West.LoadAssets();
                }
            }
        }

        /// <summary>
        /// Opens an XML data file from the filepath and creates the ChunkGrid full of Chunks.
        /// </summary>
        /// <param name="filepath">The path to the XML data file.</param>
        private void LoadFromXML(string filepath)
        {
            XDocument doc;
            XElement element;

            try
            {
                using (Stream stream = TitleContainer.OpenStream(filepath))
                {
                    doc = XDocument.Load(stream);
                    stream.Close();
                }

                float x = 0;
                float y = 0;
                float z = 0;

                element = doc.Root.Element("PlayerSpecies");
                if (element != null)
                    PlayerSpecies = element.Value;
                else
                    throw new Exception("Player Species not found in XML Data.");
                
                element = doc.Root.Element("SizeX");
                if (element != null)
                    x = int.Parse(element.Value);
                else
                    throw new Exception("X-Size not found in XML Data.");

                element = doc.Root.Element("SizeY");
                if (element != null)
                    y = int.Parse(element.Value);
                else
                    throw new Exception("Y-Size not found in XML Data.");

                element = doc.Root.Element("SizeZ");
                if (element != null)
                    z = int.Parse(element.Value);
                else
                    throw new Exception("Z-Size not found in XML Data.");

                mapSize = new Vector3(x, y, z);

                WaterLevel = int.Parse(doc.Root.Element("WaterLevel").Value);

                IEnumerable<XElement> elements = doc.Root.Elements("GlobalTerrain").Elements("Terrain");
                globalTerrain = new List<Graphics3D>();

                foreach (XElement e in elements)
                {
                    globalTerrain.Add(new Graphics3D(mapContent.Load<Model>("Terrain/" + e.Element("Name").Value)));
                }

                //element = doc.Root.Element("PlayerSpecies");
                //if (element != null)

                x = int.Parse(doc.Root.Element("PlayerSpawnX").Value);
                y = int.Parse(doc.Root.Element("PlayerSpawnY").Value);
                z = int.Parse(doc.Root.Element("PlayerSpawnZ").Value);
                PlayerSpawn = new Vector3(x, y, z);

                x = float.Parse(doc.Root.Element("SpawnDirectionX").Value);
                y = float.Parse(doc.Root.Element("SpawnDirectionY").Value);
                z = float.Parse(doc.Root.Element("SpawnDirectionZ").Value);
                SpawnDirection = new Vector3(x, y, z);

                x = int.Parse(doc.Root.Element("SpawnChunkX").Value);
                y = int.Parse(doc.Root.Element("SpawnChunkY").Value);
                z = int.Parse(doc.Root.Element("SpawnChunkZ").Value);

                elements = doc.Root.Elements("ChunkGrid").Elements("Chunk");

                List<Chunk> chunkList = new List<Chunk>();

                foreach (XElement e in elements)
                {
                    chunkList.Add(Chunk.FromXML(e));
                }

                if (chunkList.Count == 0)
                    throw new Exception("No chunks found in Map data.");

                foreach (Chunk chunk in chunkList)
                {
                    foreach (Chunk c in chunkList)
                    {
                        if ((c.Position.X == chunk.Position.X - 1) && (c.Position.Y == chunk.Position.Y) && (c.Position.Z == chunk.Position.Z))
                            chunk.West = c;
                        if ((c.Position.X == chunk.Position.X + 1) && (c.Position.Y == chunk.Position.Y) && (c.Position.Z == chunk.Position.Z))
                            chunk.East = c;

                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y) && (c.Position.Z == chunk.Position.Z - 1))
                            chunk.North = c;
                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y) && (c.Position.Z == chunk.Position.Z + 1))
                            chunk.South = c;

                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y - 1) && (c.Position.Z == chunk.Position.Z))
                            chunk.Down = c;
                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y + 1) && (c.Position.Z == chunk.Position.Z))
                            chunk.Up = c;
                    }
                }



                rootChunk = chunkList[0];
                SpawnChunk = this[(int)x, (int)y, (int)z];
                currentChunk = spawnChunk;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            initialized = true;
        }

        /// <summary>
        /// Returns the Chunk at the given Grid coordinates.
        /// </summary>
        /// <param name="x">The X-Coordinate of the Chunk to get.</param>
        /// <param name="y">The Y-Coordinate of the Chunk to get.</param>
        /// <param name="z">The Z-Coordinate of the Chunk to get.</param>
        /// <returns>The Chunk at the given Grid coordinates.</returns>
        public Chunk this[int x, int y, int z]
        {
            get
            {
                Chunk temp = rootChunk;

                for (int i = 0; i < x; i++)
                {
                    if (temp == null)
                        return null;
                    else
                        temp = temp.East;
                }

                for (int i = 0; i < y; i++)
                {
                        
                    if (temp == null)
                        return null;
                    else
                        temp = temp.Up;
                }

                for (int i = 0; i < z; i++)
                {
                    
                    if (temp == null)
                        return null;
                    else
                        temp = temp.South;
                }

                return temp;
            }
            //set { }
        }

        /// <summary>
        /// Updates each of the Chunks in the ChunkGrid.
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        /// <param name="inputStates">The InputStates object to use when checking player input.</param>
        public void Update(GameTime gameTime, InputStates inputStates, Player player)
        {
            if (initialized)
            {
                if (((player.Position.X < currentChunk.Position.X * 1000) || (player.Position.X > currentChunk.Position.X * 1000 + 1000)) ||
                    ((player.Position.Y < currentChunk.Position.Y * 1000) || (player.Position.Y > currentChunk.Position.Y * 1000 + 1000)) ||
                    ((player.Position.Z < currentChunk.Position.Z * 1000) || (player.Position.Z > currentChunk.Position.Z * 1000 + 1000)))
                {
                    Chunk oldChunk = currentChunk;
                    currentChunk = this[(int)Math.Floor(player.Position.X / 1000), (int)Math.Floor(player.Position.Y / 1000), (int)Math.Floor(player.Position.Z / 1000)];
                    if (currentChunk == null)
                        currentChunk = oldChunk;
                    //System.Diagnostics.Debug.WriteLine("New Chunk: " + currentChunk.Position);
                }



                foreach (Chunk chunk in this)
                {
                    chunk.Update(gameTime, inputStates);
                }

                foreach (Graphics3D g in globalTerrain)
                {
                    g.Update(gameTime);
                }
            }
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public void Draw3D(GameTime gameTime)
        {
            if (initialized)
            {
                foreach (Chunk chunk in this)
                {
                    chunk.Draw3D(gameTime);
                }

                foreach (Graphics3D g in globalTerrain)
                {
                    g.Draw3D(gameTime);
                }
            }
        }

        ///// <summary>
        ///// Draws any 2-dimensional sprites to the SpriteBatch (2D Sprites are always drawn above 3D material).
        ///// </summary>
        ///// <param name="gameTime">The GameTime object to use as reference.</param>
        ///// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        //public void Draw2D(GameTime gameTime, SpriteBatch spriteBatch)
        //{
        //}

        /// <summary>
        /// Gets an Enumerator to enumerate through the Chunks in the grid.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        /// <summary>
        /// Gets an Enumerator to enumerate through the Chunks in the grid.
        /// </summary>
        /// <returns></returns>
        public ChunkGridEnumerator GetEnumerator()
        {
            return new ChunkGridEnumerator(rootChunk);
        }
    }

    /// <summary>
    /// An Enumerator to be used with a ChunkGrid that enumerates through the grid.
    /// </summary>
    public class ChunkGridEnumerator : IEnumerator
    {
        private Chunk chunk = null;
        private Chunk root;

        private bool left = false;
        private bool up = false;

        /// <summary>
        /// Creates a new Enumerator.
        /// </summary>
        /// <param name="root">The root Chunk in the grid.</param>
        public ChunkGridEnumerator(Chunk root)
        {
            this.root = root;
        }

        /// <summary>
        /// Moves the enumerator one step forward through the grid.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (chunk == null)
            {
                chunk = root;
                return true;
            }
            else
            {
                if (!left && chunk.East != null)
                {
                    chunk = chunk.East;
                    return true;
                }
                else if (!left && chunk.East == null)
                {
                    if (!up && chunk.South != null)
                    {
                        chunk = chunk.South;
                        left = true;
                        return true;
                    }
                    else if (!up && chunk.South == null)
                    {
                        if (chunk.Up != null)
                        {
                            chunk = chunk.Up;
                            up = true;
                            left = true;
                            return true;
                        }
                        else
                            return false;
                    }
                    else if (up && chunk.North != null)
                    {
                        chunk = chunk.North;
                        left = true;
                        return true;
                    }
                    else if (up && chunk.North == null)
                    {
                        if (chunk.Up != null)
                        {
                            chunk = chunk.Up;
                            up = false;
                            left = true;
                            return true;
                        }
                        else
                            return false;
                    }

                }
                else if (left && chunk.West != null)
                {
                    chunk = chunk.West;
                    return true;
                }
                else if (left && chunk.West == null)
                {
                    if (!up && chunk.South != null)
                    {
                        chunk = chunk.South;
                        left = false;
                        return true;
                    }
                    else if (!up && chunk.South == null)
                    {
                        if (chunk.Up != null)
                        {
                            chunk = chunk.Up;
                            up = true;
                            left = false;
                            return true;
                        }
                        else
                            return false;
                    }
                    else if (up && chunk.North != null)
                    {
                        chunk = chunk.North;
                        left = false;
                        return true;
                    }
                    else if (up && chunk.North == null)
                    {
                        if (chunk.Up != null)
                        {
                            chunk = chunk.Up;
                            up = false;
                            left = false;
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Resets the enumerator to the beginning.
        /// </summary>
        public void Reset()
        {
            chunk = null;
            left = false;
            up = false;
        }

        /// <summary>
        /// Returns the Chunk being pointed to by the enumerator.
        /// </summary>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        /// Returns the Chunk being pointed to by the enumerator.
        /// </summary>
        public Chunk Current
        {
            get { return chunk; }
        }
    }
}
