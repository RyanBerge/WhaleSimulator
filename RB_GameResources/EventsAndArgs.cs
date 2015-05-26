using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RB_GameResources
{
    public delegate void MouseEventHandler(object sender, MouseEventArgs args);
    public delegate void TextFieldEventHandler(object sender, TextFieldEventArgs args);

    public class MouseEventArgs : EventArgs
    {
        [FlagsAttribute]
        public enum MouseEventType : byte
        {
            None = 0,
            Press = 1,
            Release = 2,
            Enter = 4,
            Leave = 8,
            Move = 16, 
        }

        private MouseEventType eventType;
        private int x;
        private int y;

        public MouseEventArgs(MouseEventType arg, int xVal, int yVal)
        {
            eventType = arg;
            x = xVal;
            y = yVal;
        }

        public MouseEventType EventType
        {
            get { return eventType; }
        }

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
        }
    }

    public class TextFieldEventArgs : EventArgs
    {
        private char digit;
        private int index;

        public TextFieldEventArgs(char digit, int index)
        {
            this.digit = digit;
            this.index = index;
        }

        public char Digit
        {
            get { return digit; }
        }

        public int Index
        {
            get { return index; }
        }
    }
}
