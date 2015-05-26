using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RB_GameResources.Xna.Controls
{
    public enum MouseButtons : byte
    {
        RightButton, LeftButton, MiddleButton, XButton1, XButton2
    }

    /// <summary>
    /// Encapsulates the various XNA input states into a single class.
    /// </summary>
    public class InputStates
    {
        private MouseState oldMouseState;
        private MouseState newMouseState;
        private KeyboardState oldKeyState;
        private KeyboardState newKeyState;
        private GamePadState oldGPState;
        private GamePadState newGPState;

        /// <summary>
        /// Create a new instance of InputStates using the current control states as the initialization.
        /// </summary>
        public InputStates()
        {
            oldMouseState = Mouse.GetState();
            newMouseState = Mouse.GetState();
            oldKeyState = Keyboard.GetState();
            newKeyState = Keyboard.GetState();
            oldGPState = GamePad.GetState(PlayerIndex.One);
            newGPState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Create a new instance of Input States
        /// </summary>
        /// <param name="oldMouseState">The old mouse state to initialize.</param>
        /// <param name="newMouseState">The new mouse state to initialize.</param>
        /// <param name="oldKeyState">The old keyboard state to initialize.</param>
        /// <param name="newKeyState">The new keyboard state to initialize.</param>
        /// <param name="oldGPState">The old gamepad state to initialize.</param>
        /// <param name="newGPState">The new gamepad state to initialize.</param>
        public InputStates(MouseState oldMouseState, MouseState newMouseState, KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState oldGPState, GamePadState newGPState)
        {
            this.oldMouseState = oldMouseState;
            this.newMouseState = newMouseState;
            this.oldKeyState = oldKeyState;
            this.newKeyState = newKeyState;
            this.oldGPState = oldGPState;
            this.newGPState = newGPState;
        }

        /// <summary>
        /// Create a new instance of Input States
        /// </summary>
        /// <param name="mouseState">The mouse state to initialize.</param>
        /// <param name="keyState">The keyboard state to initialize.</param>
        /// <param name="padState">The gamepad state to initialize.</param>
        public InputStates(MouseState mouseState, KeyboardState keyState, GamePadState padState)
        {
            this.oldMouseState = mouseState;
            this.newMouseState = mouseState;
            this.oldKeyState = keyState;
            this.newKeyState = keyState;
            this.oldGPState = padState;
            this.newGPState = padState;
        }

        /// <summary>
        /// Changes the "new" states to be updated with the current state of the controls.
        /// </summary>
        public void RefreshNewStates()
        {
            newMouseState = Mouse.GetState();
            newKeyState = Keyboard.GetState();
            newGPState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Manually set the "new" state for all controls.
        /// </summary>
        /// <param name="newMouseState">The new mouse state to set.</param>
        /// <param name="newKeyboardState">The new keyboard state to set.</param>
        /// <param name="newGPState">The new gamepad state to set.</param>
        public void SetNewStates(MouseState newMouseState, KeyboardState newKeyboardState, GamePadState newGPState)
        {
            this.newMouseState = newMouseState;
            this.newKeyState = newKeyboardState;
            this.newGPState = newGPState;
        }

        /// <summary>
        /// "Rotates" out the old states, causing the old new states to become old states.
        /// </summary>
        /// <param name="refreshStates">Whether or not to also refresh the new states to update the controls.</param>
        public void RotateOldStates(bool refreshStates)
        {
            oldMouseState = newMouseState;
            oldKeyState = newKeyState;
            oldGPState = newGPState;
            if (refreshStates)
                RefreshNewStates();
        }

        /// <summary>
        /// Returns true if the key was up and is now down (if they key was just pressed)
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool WasButtonPressed(Keys key)
        {
            if (oldKeyState.IsKeyUp(key) && newKeyState.IsKeyDown(key))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the key was down and is now up (if they key was just released)
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool WasButtonReleased(Keys key)
        {
            if (oldKeyState.IsKeyDown(key) && newKeyState.IsKeyUp(key))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the button was up and is now down (if they button was just pressed)
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns></returns>
        public bool WasButtonPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    if (oldMouseState.LeftButton == ButtonState.Released && newMouseState.LeftButton == ButtonState.Pressed)
                        return true;
                    else
                        return false;
                case MouseButtons.RightButton:
                    if (oldMouseState.RightButton == ButtonState.Released && newMouseState.RightButton == ButtonState.Pressed)
                        return true;
                    else
                        return false;
                case MouseButtons.MiddleButton:
                    if (oldMouseState.MiddleButton == ButtonState.Released && newMouseState.MiddleButton == ButtonState.Pressed)
                        return true;
                    else
                        return false;
                case MouseButtons.XButton1:
                    if (oldMouseState.XButton1 == ButtonState.Released && newMouseState.XButton1 == ButtonState.Pressed)
                        return true;
                    else
                        return false;
                case MouseButtons.XButton2:
                    if (oldMouseState.XButton2 == ButtonState.Released && newMouseState.XButton2 == ButtonState.Pressed)
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the button was down and is now up (if they button was just released)
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns></returns>
        public bool WasButtonReleased(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    if (oldMouseState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Released)
                        return true;
                    else
                        return false;
                case MouseButtons.RightButton:
                    if (oldMouseState.RightButton == ButtonState.Pressed && newMouseState.RightButton == ButtonState.Released)
                        return true;
                    else
                        return false;
                case MouseButtons.MiddleButton:
                    if (oldMouseState.MiddleButton == ButtonState.Pressed && newMouseState.MiddleButton == ButtonState.Released)
                        return true;
                    else
                        return false;
                case MouseButtons.XButton1:
                    if (oldMouseState.XButton1 == ButtonState.Pressed && newMouseState.XButton1 == ButtonState.Released)
                        return true;
                    else
                        return false;
                case MouseButtons.XButton2:
                    if (oldMouseState.XButton2 == ButtonState.Pressed && newMouseState.XButton2 == ButtonState.Released)
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the button was up and is now down (if they button was just pressed)
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns></returns>
        public bool WasButtonPressed(Buttons button)
        {
            if (oldGPState.IsButtonUp(button) && newGPState.IsButtonDown(button))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the button was down and is now up (if they button was just released)
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns></returns>
        public bool WasButtonReleased(Buttons button)
        {
            if (oldGPState.IsButtonDown(button) && newGPState.IsButtonUp(button))
                return true;
            else
                return false;
        }

        /// <summary>
        /// The current "old" mouse state
        /// </summary>
        public MouseState OldMouseState
        {
            get { return oldMouseState; }
            set { oldMouseState = value; }
        }

        /// <summary>
        /// The current "new" mouse state
        /// </summary>
        public MouseState NewMouseState
        {
            get { return newMouseState; }
            set { newMouseState = value; }
        }

        /// <summary>
        /// The current "old" keyboard state
        /// </summary>
        public KeyboardState OldKeyState
        {
            get { return oldKeyState; }
            set { oldKeyState = value; }
        }

        /// <summary>
        /// The current "new" keyboard state
        /// </summary>
        public KeyboardState NewKeyState
        {
            get { return newKeyState; }
            set { newKeyState = value; }
        }

        /// <summary>
        /// The current "old" gamepad state
        /// </summary>
        public GamePadState OldGPState
        {
            get { return oldGPState; }
            set { oldGPState = value; }
        }

        /// <summary>
        /// The current "new" gamepad state
        /// </summary>
        public GamePadState NewGPState
        {
            get { return newGPState; }
            set { newGPState = value; }
        }
    }
}
