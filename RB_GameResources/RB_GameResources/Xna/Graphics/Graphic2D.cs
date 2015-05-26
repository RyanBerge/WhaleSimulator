using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RB_GameResources.Xna.Graphics
{
    public class Graphic2D : IDrawable
    {
        private const int ANIMATION_SLOWDOWN_THRESHOLD = 16;

        private Animation2D[] animations;
        private Texture2D spritesheet;
        private Vector2 coordinates;
        private Vector2 center;
        private Rectangle boundingBox;
        private int currentAnimation;

        private int previousAnimation;
        private double animationTimer;
        private bool atAnimationEnd;
        private bool animationEventRaised;
        private bool isVisible;

        /// <summary>
        /// This event is raised whenever a non-looping animation ends (at the end of the last frame).
        /// </summary>
        public event EventHandler AnimationEnded;

        public Graphic2D(Texture2D spritesheet, Animation2D[] animations, int startingAnimation, Vector2 coordinates)
        {
            this.spritesheet = spritesheet;
            Initialize(animations, startingAnimation, coordinates);
        }

        public Graphic2D(ContentManager Content, string filePath, Animation2D[] animations, int startingAnimation, Vector2 coordinates)
        {
            spritesheet = Content.Load<Texture2D>(filePath);
            Initialize(animations, startingAnimation, coordinates);
        }

        private void Initialize(Animation2D[] animations, int startingAnimation, Vector2 coordinates)
        {
            this.animations = animations;
            this.coordinates = coordinates;
            isVisible = true;
            currentAnimation = startingAnimation;
            previousAnimation = currentAnimation;
            animationTimer = 0;
            atAnimationEnd = false;
            animationEventRaised = false;
            boundingBox = new Rectangle((int)coordinates.X, (int)coordinates.Y, (int)animations[currentAnimation].Dimensions.X, (int)animations[currentAnimation].Dimensions.Y);
            center = new Vector2(boundingBox.X + (float)(boundingBox.Width / 2), boundingBox.Y + (float)(boundingBox.Height / 2));
        }

        /// <summary>
        /// Draws the Graphic2D to the SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        /// <param name="gameTime">The GameTime to use for determining the animation frame.</param>
        /// <param name="playAnimation">Whether or not to advance the animation for this object.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool playAnimation)
        {
            if (isVisible)
            {
                if (playAnimation && animations[currentAnimation].IsPlaying)
                    AdvanceFrame(gameTime);
                spriteBatch.Draw(spritesheet, coordinates, animations[currentAnimation].SourceFrame, Color.White);
            }
        }

        /// <summary>
        /// Draws the Graphic2D to the SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        /// <param name="gameTime">The GameTime to use for determining the animation frame.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch, gameTime, true);
        }

        /// <summary>
        /// Draws the Graphic2D to the SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        /// <param name="gameTime">The GameTime to use for determining the animation frame.</param>
        /// <param name="layerDepth">The depth of the layer to draw to.  0 represents the front, and 1 represents the back.</param>
        /// <param name="playAnimation">Whether or not to advance the animation for this object.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float layerDepth, bool playAnimation)
        {
            if (isVisible)
            {
                if (playAnimation && animations[currentAnimation].IsPlaying)
                    AdvanceFrame(gameTime);
                spriteBatch.Draw(spritesheet, coordinates, animations[currentAnimation].SourceFrame, Color.White, 0, new Vector2(0, 0), 0, SpriteEffects.None, layerDepth);
            }
        }

        /// <summary>
        /// Draws the Graphic2D to the SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        /// <param name="gameTime">The GameTime to use for determining the animation frame.</param>
        /// <param name="layerDepth">The depth of the layer to draw to.  0 represents the front, and 1 represents the back.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float layerDepth)
        {
            Draw(spriteBatch, gameTime, layerDepth, true);
        }

        /// <summary>
        /// Draws the Graphic2D to the SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw to.</param>
        /// <param name="gameTime">The GameTime to use for determining the animation frame.</param>
        /// <param name="rotation">The angle, in radians, to rotate the object.</param>
        /// <param name="origin">The origin of where to draw the object.</param>
        /// <param name="scale">Scale factor.</param>
        /// <param name="spriteEffect">The SpriteEffect to use.</param>
        /// <param name="layerDepth">The depth of the layer to draw to.  0 represents the front, and 1 represents the back.</param>
        /// <param name="playAnimation">Whether or not to advance the animation for this object.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffect, float layerDepth, bool playAnimation)
        {
            if (isVisible)
            {
                if (playAnimation && animations[currentAnimation].IsPlaying)
                    AdvanceFrame(gameTime);
                spriteBatch.Draw(spritesheet, coordinates, animations[currentAnimation].SourceFrame, Color.White, rotation, origin, scale, spriteEffect, layerDepth);
            }
        }

        /// <summary>
        /// Restarts the current animation.
        /// </summary>
        public void RestartAnimation()
        {
            animations[currentAnimation].CurrentFrame = 0;
            atAnimationEnd = false;
            animationEventRaised = false;
            animationTimer = 0;
        }

        /// <summary>
        /// Checks how much time has passed since the last call to determine whether the current animation's frame must be advanced and if it does, advances it.
        /// </summary>
        /// <param name="gameTime">The GameTime to use to check how much time has passed.</param>
        private void AdvanceFrame(GameTime gameTime)
        {
            if (currentAnimation != previousAnimation)
            {
                previousAnimation = currentAnimation;
                animations[currentAnimation].CurrentFrame = 0;
                animationTimer = 0;
            }
            else
            {
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (animations[currentAnimation].AnimationSpeed >= ANIMATION_SLOWDOWN_THRESHOLD)
                {
                    while (animationTimer >= animations[currentAnimation].AnimationSpeed)
                    {
                        if (!atAnimationEnd)
                            animations[currentAnimation].CurrentFrame++;
                        else if (animations[currentAnimation].IsLooping)
                            animations[currentAnimation].CurrentFrame = 0;
                        else
                            OnAnimationEnded();

                        animationTimer -= animations[currentAnimation].AnimationSpeed;
                    }
                }
                else
                {
                    if (animationTimer >= animations[currentAnimation].AnimationSpeed)
                    {
                        if (!atAnimationEnd)
                            animations[currentAnimation].CurrentFrame++;
                        else if (animations[currentAnimation].IsLooping)
                            animations[currentAnimation].CurrentFrame = 0;
                        else
                            OnAnimationEnded();

                        animationTimer -= animations[currentAnimation].AnimationSpeed;
                    }
                }
            }

            if (animations[currentAnimation].CurrentFrame == animations[currentAnimation].NumberOfFrames - 1)
                atAnimationEnd = true;
            else
                atAnimationEnd = false;

        }

        /// <summary>
        /// Event callback for when a non-looping animation ends.
        /// </summary>
        private void OnAnimationEnded()
        {
            if (AnimationEnded != null && !animationEventRaised)
            {
                animationEventRaised = true;
                AnimationEnded(this, EventArgs.Empty);
            }
        }

        /*
        /// <summary>
        /// Constructs a new Graphic2D object from the specified XML document.
        /// </summary>
        /// <param name="xdoc">The XML document to read from.</param>
        /// <param name="Content">The ContentManager to use.</param>
        /// <returns>A new Graphic2D object.</returns>
        public static Graphic2D FromXML(XDocument xdoc, ContentManager Content)
        {
            return FromXML(xdoc.Root, Content, null);
        }

        /// <summary>
        /// Constructs a new Graphic2D object from the specified XML subtree.
        /// </summary>
        /// <param name="rootElement">The root of the XML subtree to read from.</param>
        /// <param name="Content">The ContentManager to use.</param>
        /// <returns>A new Graphic2D object.</returns>
        public static Graphic2D FromXML(XElement rootElement, ContentManager Content)
        {
            return FromXML(rootElement, Content, null);
        }

        /// <summary>
        /// Constructs a new Graphic2D object from the specified XML document.
        /// </summary>
        /// <param name="xdoc">The XML document to read from.</param>
        /// <param name="spritesheet">The Texture2D to use as the new object's spritesheet.</param>
        /// <returns>A new Graphic2D object.</returns>
        public static Graphic2D FromXML(XDocument xdoc, Texture2D spritesheet)
        {
            return FromXML(xdoc.Root, null, spritesheet);
        }

        /// <summary>
        /// Constructs a new Graphic2D object from the specified XML subtree.
        /// </summary>
        /// <param name="rootElement">The root of the XML subtree to read from.</param>
        /// <param name="spritesheet">The Texture2D to use as the new object's spritesheet.</param>
        /// <returns>A new Graphic2D object.</returns>
        public static Graphic2D FromXML(XElement rootElement, Texture2D spritesheet)
        {
            return FromXML(rootElement, null, spritesheet);
        }

        /// <summary>
        /// Constructs a new Graphic2D object from the specified XML document.
        /// </summary>
        /// <param name="xdoc">The XML document to read from.</param>
        /// <param name="Content">The ContentManager to use.</param>
        /// <param name="spritesheet">The Texture2D to use as the new object's spritesheet.</param>
        /// <returns>A new Graphic2D object.</returns>
        private static Graphic2D FromXML(XDocument xdoc, ContentManager Content, Texture2D spritesheet)
        {
            return FromXML(xdoc.Root, Content, spritesheet);
        }

        /// <summary>
        /// Constructs a new Graphic2D object from the specified XML subtree.
        /// </summary>
        /// <param name="rootElement">The root of the XML subtree to read from.</param>
        /// <param name="Content">The ContentManager to use.</param>
        /// <param name="spritesheet">The Texture2D to use as the new object's spritesheet.</param>
        /// <returns>A new Graphic2D object.</returns>
        private static Graphic2D FromXML(XElement rootElement, ContentManager Content, Texture2D spritesheet)
        {
            try
            {
                XElement element;

                string filePath = null;
                Vector2 coordinates = new Vector2();
                int startingAnimation = 0;

                element = rootElement.Element("FilePath");
                if (element != null)
                    filePath = element.Value;

                if (spritesheet == null && filePath == null)
                    throw new System.IO.FileFormatException("If no spritesheet is defined, the XML data must contain member <FilePath>.");

                element = rootElement.Element("Coordinates");
                if (element != null)
                    coordinates = new Vector2(float.Parse(element.Element("X").Value), float.Parse(element.Element("Y").Value));

                element = rootElement.Element("StartingAnimation");
                if (element != null)
                    startingAnimation = int.Parse(element.Value);

                IEnumerable<XElement> elements = rootElement.Elements("AnimationList").Elements("Animation");

                using (IEnumerator<XElement> enumerator = elements.GetEnumerator())
                {
                    enumerator.MoveNext();

                    if (enumerator.Current == null)
                        throw new System.IO.FileFormatException("XML document must contain member <AnimationList> which must contain at least one member <Animation>");
                }

                List<Animation2D> animationList = new List<Animation2D>();

                foreach (XElement el in elements)
                {
                    Animation2D animation = Animation2D.FromXML(el);
                    animationList.Add(animation);
                }

                Animation2D[] animationArray = animationList.ToArray();

                if (spritesheet == null && filePath != null)
                    spritesheet = Content.Load<Texture2D>(filePath);

                return new Graphic2D(spritesheet, animationArray, startingAnimation, coordinates);
            }
            catch (System.IO.FileFormatException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ArgumentException("There was an error reading the XML file.\n" + e.Message);
            }
        }*/

        /// <summary>
        /// The array containing all the animations associated with this Graphics2D object.
        /// </summary>
        public Animation2D[] AnimationArray
        {
            get { return animations; }
        }

        /// <summary>
        /// The index of the animation array where the previous animation is located.
        /// </summary>
        public int PreviousAnimationIndex
        {
            get { return previousAnimation; }
        }

        /// <summary>
        /// The index of the animation array where the current animation is located.
        /// </summary>
        public int CurrentAnimationIndex
        {
            get { return currentAnimation; }
            set
            {
                previousAnimation = currentAnimation;
                currentAnimation = value;
                animationEventRaised = false;
                boundingBox.Width = (int)animations[currentAnimation].Dimensions.X;
                boundingBox.Height = (int)animations[currentAnimation].Dimensions.Y;
                center.X = boundingBox.X + (float)(boundingBox.Width / 2);
                center.Y = boundingBox.Y + (float)(boundingBox.Height / 2);
            }
        }

        /// <summary>
        /// The coordinates in the window where this Graphics2D object is drawn.
        /// </summary>
        public Vector2 Coordinates
        {
            get { return coordinates; }
            set
            {
                coordinates = value;
                boundingBox.X = (int)coordinates.X;
                boundingBox.Y = (int)coordinates.Y;
                center.X = boundingBox.X + (float)(boundingBox.Width / 2);
                center.Y = boundingBox.Y + (float)(boundingBox.Height / 2);
            }
        }

        /// <summary>
        /// The center of this Graphic2D object's bounding box.
        /// </summary>
        public Vector2 Center
        {
            get { return center; }
        }

        /// <summary>
        /// A rectangle representing the bounding box of this Graphic2D object.
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            { return boundingBox; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
    }
}
