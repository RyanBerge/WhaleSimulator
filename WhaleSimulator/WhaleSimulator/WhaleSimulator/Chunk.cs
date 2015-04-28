using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Microsoft.Xna.Framework;

namespace WhaleSimulator
{
    public class Chunk
    {
        public Chunk North { get; set; }
        public Chunk South { get; set; }
        public Chunk East { get; set; }
        public Chunk West { get; set; }
        public Chunk Up { get; set; }
        public Chunk Down { get; set; }

        public Vector3 Position { get; private set; }

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


                return new Chunk(new Vector3(x, y, z));

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return null;
        }
    }
}
