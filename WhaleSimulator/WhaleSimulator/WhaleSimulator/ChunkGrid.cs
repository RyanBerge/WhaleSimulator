using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        private Vector3 mapSize;

        private bool initialized = false;

        public Vector3 PlayerSpawn { get; set; }
        public Vector3 SpawnDirection { get; set; }

        /// <summary>
        /// Creates a new ChunkGrid by loading data from the designated filepath.
        /// </summary>
        /// <param name="filepath">The path to the XML data file.</param>
        public ChunkGrid(string filepath)
        {
            LoadFromXML(filepath);
            float x = spawnChunk.SpawnPosition.X + (spawnChunk.Position.X * 1000);
            float y = spawnChunk.SpawnPosition.Y + (spawnChunk.Position.Y * 1000);
            float z = spawnChunk.SpawnPosition.Z + (spawnChunk.Position.Z * 1000);
            PlayerSpawn = new Vector3(x, y, z);
            SpawnDirection = spawnChunk.SpawnDirection;
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

                int x = 0;
                int y = 0;
                int z = 0;
                
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

                IEnumerable<XElement> elements = doc.Root.Elements("ChunkGrid").Elements("Chunk");

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

                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y - 1) && (c.Position.Z == chunk.Position.Z))
                            chunk.North = c;
                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y + 1) && (c.Position.Z == chunk.Position.Z))
                            chunk.South = c;

                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y) && (c.Position.Z == chunk.Position.Z - 1))
                            chunk.Down = c;
                        if ((c.Position.X == chunk.Position.X) && (c.Position.Y == chunk.Position.Y) && (c.Position.Z == chunk.Position.Z + 1))
                            chunk.Up = c;
                    }
                    if (chunk.HasSpawn)
                        spawnChunk = chunk;
                }

                rootChunk = chunkList[0];
                currentChunk = rootChunk;

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
                        temp = temp.South;
                }

                for (int i = 0; i < z; i++)
                {
                    
                    if (temp == null)
                        return null;
                    else
                        temp = temp.Up;
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
        public void Update(GameTime gameTime, InputStates inputStates)
        {
            if (initialized)
            {
                foreach (Chunk chunk in this)
                {
                    chunk.Update(gameTime, inputStates);
                }
            }
        }

        /// <summary>
        /// Draws any 3D objects to the screen (3D objects are always drawn behind 2D sprites).
        /// </summary>
        /// <param name="gameTime">The GameTime object to use as reference.</param>
        public void Draw3D(GameTime gameTime)
        {
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
