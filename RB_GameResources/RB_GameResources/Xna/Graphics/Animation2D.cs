using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;



namespace RB_GameResources.Xna.Graphics
{
    public enum Orientation : byte
    {
        Vertical, Horizontal
    }

    /// <summary>
    /// Defines the structure and attributes for an Animation.
    /// </summary>
    public struct Animation2D
    {
        private int numFrames;
        private int currentFrame;
        private double animationSpeed;
        private double defaultAnimationSpeed;
        private bool isPlaying;
        private bool isLooping;
        private string name;

        private Vector2 location;
        private Vector2 dimensions;
        private Rectangle sourceFrame;
        private Orientation orientation;

        /// <summary>
        /// Creates a new Animation.
        /// </summary>
        /// <param name="location">The location to find the start of the animation on a spritesheet.</param>
        /// <param name="dimensions">The dimensions of the frames in the animation.</param>
        /// <param name="numFrames">The number of frames in the animation.</param>
        /// <param name="speed">The speed of the animation, expressed in frames-per-second.</param>
        public Animation2D(string name, Vector2 location, Vector2 dimensions, int numFrames, double speed) :
            this(name, location, dimensions, new Vector2(0, 0), numFrames, speed, true, true, Orientation.Horizontal) { }

        /// <summary>
        /// Creates a new Animation.
        /// </summary>
        /// <param name="location">The location to find the start of the animation on a spritesheet.</param>
        /// <param name="dimensions">The dimensions of the frames in the animation.</param>
        /// <param name="numFrames">The number of frames in the animation.</param>
        /// <param name="speed">The speed of the animation, expressed in frames-per-second.</param>
        /// <param name="playing">Whether the animation should be playing.</param>
        /// <param name="looping">Whether the animation should be looping.</param>
        /// <param name="orientation">The orientation of the animation on the spritesheet.</param>
        public Animation2D(string name, Vector2 location, Vector2 dimensions, Vector2 origin, int numFrames, double speed, bool playing, bool looping, Orientation orientation)
        {
            this.name = name;
            this.numFrames = numFrames;
            currentFrame = 0;
            animationSpeed = 1 / speed;
            defaultAnimationSpeed = speed;
            isPlaying = playing;
            isLooping = looping;
            this.location = location;
            this.dimensions = dimensions;
            this.orientation = orientation;

            sourceFrame = new Rectangle((int)location.X, (int)location.Y, (int)dimensions.X, (int)dimensions.Y);
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /*public static Animation2D FromXML(XDocument xdoc)
        {
            return FromXML(xdoc.Root);
        }*/

        /*public static Animation2D FromXML(XElement rootElement)
        {
            XElement element;

            Vector2 location = new Vector2(0, 0);
            Vector2 dimensions = new Vector2(0, 0);
            Vector2 origin = new Vector2(0, 0);
            string name = "Unnamed";
            int numFrames;
            double speed = 8;
            bool playing = true;
            bool looping = true;
            Orientation orientation = Orientation.Horizontal;

            element = rootElement.Element("Name");
            if (element != null)
                name = element.Value;

            element = rootElement.Element("Location");
            if (element != null)
                location = new Vector2(float.Parse(element.Element("X").Value), float.Parse(element.Element("Y").Value));

            element = rootElement.Element("Dimensions");
            if (element != null)
                dimensions = new Vector2(float.Parse(element.Element("Width").Value), float.Parse(element.Element("Height").Value));
            else
                throw new System.Exception("XML document must contain member <Dimensions> for each animation defined.");

            element = rootElement.Element("Origin");
            if (element != null)
                origin = new Vector2(float.Parse(element.Element("X").Value), float.Parse(element.Element("Y").Value));

            element = rootElement.Element("NumFrames");
            if (element != null)
                numFrames = int.Parse(element.Value);
            else
                throw new System.Exception("XML document must contain member <NumFrames> for each animation defined.");
            if (numFrames < 1)
                throw new System.Exception("XML element <NumFrames> must have a value greater than zero.");

            element = rootElement.Element("Speed");
            if (element != null)
                speed = double.Parse(element.Value);

            element = rootElement.Element("Playing");
            if (element != null)
                playing = bool.Parse(element.Value);

            element = rootElement.Element("Looping");
            if (element != null)
                looping = bool.Parse(element.Value);

            element = rootElement.Element("Orientation");
            if (element != null)
                orientation = (Orientation)Enum.Parse(typeof(Orientation), element.Value);
            

            return new Animation2D(name, location, dimensions, origin, numFrames, speed, playing, looping, orientation);
        }*/

        /// <summary>
        /// The number of frames in this animation.
        /// </summary>
        public int NumberOfFrames
        {
            get { return numFrames; }
            set { numFrames = value; }
        }

        /// <summary>
        /// The current frame of this animation.
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set 
            {
                currentFrame = value;
                if (orientation == Orientation.Horizontal)
                    sourceFrame.X = (int)(currentFrame * dimensions.X);
                else if (orientation == Orientation.Vertical)
                    sourceFrame.Y = (int)(currentFrame * dimensions.Y);
            }
        }

        /// <summary>
        /// The speed of this animation, expressed in frames-per-second.
        /// </summary>
        public double AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }

        /// <summary>
        /// The default speed of this animation, expressed in frames-per-second.
        /// </summary>
        public double DefaultAnimationSpeed
        {
            get { return defaultAnimationSpeed; }
            set { defaultAnimationSpeed = value; }
        }

        /// <summary>
        /// Whether or not this animation is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get { return isPlaying; }
            set { isPlaying = value; }
        }

        /// <summary>
        /// Whether or not this animation loops.
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
            set { isLooping = value; }
        }

        /// <summary>
        /// The location of this animation on a spritesheet.
        /// </summary>
        public Vector2 Location
        {
            get { return location; }
            set 
            {
                location = value;
                if (orientation == Orientation.Horizontal)
                {
                    sourceFrame.X = (int)(location.X + (currentFrame * dimensions.X));
                    sourceFrame.Y = (int)(location.Y);
                }
                else if (orientation == Orientation.Vertical)
                {
                    sourceFrame.X = (int)(location.X);
                    sourceFrame.Y = (int)(location.Y + (currentFrame * dimensions.Y));
                }
            }
        }

        /// <summary>
        /// The dimensions of this animation's frames.
        /// </summary>
        public Vector2 Dimensions
        {
            get { return dimensions; }
            set
            {
                dimensions = value;
                if (orientation == Orientation.Horizontal)
                    sourceFrame = new Rectangle((int)(location.X + (currentFrame * dimensions.X)), (int)(location.Y), (int)dimensions.X, (int)dimensions.Y);
                else if (orientation == Orientation.Vertical)
                    sourceFrame = new Rectangle((int)(location.X), (int)(location.Y + (currentFrame * dimensions.Y)), (int)dimensions.X, (int)dimensions.Y);
            }
        }

        /// <summary>
        /// The orientation of this animation's frames on a spritesheet.
        /// </summary>
        public Orientation AnimationOrientation
        {
            get { return orientation; }
            set 
            {
                orientation = value;
                if (orientation == Orientation.Horizontal)
                {
                    sourceFrame.X = (int)(location.X + (currentFrame * dimensions.X));
                    sourceFrame.Y = (int)(location.Y);
                }
                else if (orientation == Orientation.Vertical)
                {
                    sourceFrame.X = (int)(location.X);
                    sourceFrame.Y = (int)(location.Y + (currentFrame * dimensions.Y));
                }
            }
        }

        /// <summary>
        /// A rectangle representing the location on a spritesheet to find the current frame of this animation.
        /// </summary>
        public Rectangle SourceFrame
        {
            get { return sourceFrame; }
        }
    }
}