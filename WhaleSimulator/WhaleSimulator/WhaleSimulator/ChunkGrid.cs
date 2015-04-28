using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

using Microsoft.Xna.Framework;

namespace WhaleSimulator
{
    /// <summary>
    /// A data structure that stores map "chunks" and allows the traversing through them, while loading and unloading as needed.
    /// </summary>
    public class ChunkGrid
    {
        private Chunk spawnChunk;
        private Chunk currentChunk;
        private Chunk rootChunk;

        private Vector3 mapSize;

        public ChunkGrid(string filepath)
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
                }

                rootChunk = chunkList[0];
                currentChunk = rootChunk;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            

            
            

        }
    }
}
