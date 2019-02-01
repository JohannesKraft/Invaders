using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invaders.Interface
{
    public enum MouseButtons
    {
        None,
        Left,
        Right,
        Middle
    }

    public enum ContentAlignment
    {
        None,
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
    public static class Utilities
    {
        public static Vector2 GetContentAlign(Rectangle bounds, ContentAlignment align, Vector2 objSize)
        {
            float x = 0;
            float y = 0;
            string alignStr = align.ToString().ToLower();

            if (alignStr.Contains("left"))
            {
                x = bounds.X;
            }
            if (alignStr.Contains("center"))
            {
                x = bounds.Width / 2 + bounds.X - objSize.X / 2;
            }
            if (alignStr.Contains("right"))
            {
                x = bounds.X + bounds.Width - objSize.X;
            }

            if (alignStr.Contains("top"))
            {
                y = bounds.Y;
            }
            if (alignStr.Contains("middle"))
            {
                y = bounds.Height / 2 + bounds.Y - objSize.Y / 2;
            }
            if (alignStr.Contains("bottom"))
            {
                y = bounds.Y + bounds.Height - objSize.Y;
            }

            return new Vector2(x, y);
        }

        public static bool TryConvertKeyboardInput(Keys key, bool shift, out char character)
        {
            switch (key)
            {
                //Alphabet keys
                case Keys.A: if (shift) { character = 'A'; } else { character = 'a'; } return true;
                case Keys.B: if (shift) { character = 'B'; } else { character = 'b'; } return true;
                case Keys.C: if (shift) { character = 'C'; } else { character = 'c'; } return true;
                case Keys.D: if (shift) { character = 'D'; } else { character = 'd'; } return true;
                case Keys.E: if (shift) { character = 'E'; } else { character = 'e'; } return true;
                case Keys.F: if (shift) { character = 'F'; } else { character = 'f'; } return true;
                case Keys.G: if (shift) { character = 'G'; } else { character = 'g'; } return true;
                case Keys.H: if (shift) { character = 'H'; } else { character = 'h'; } return true;
                case Keys.I: if (shift) { character = 'I'; } else { character = 'i'; } return true;
                case Keys.J: if (shift) { character = 'J'; } else { character = 'j'; } return true;
                case Keys.K: if (shift) { character = 'K'; } else { character = 'k'; } return true;
                case Keys.L: if (shift) { character = 'L'; } else { character = 'l'; } return true;
                case Keys.M: if (shift) { character = 'M'; } else { character = 'm'; } return true;
                case Keys.N: if (shift) { character = 'N'; } else { character = 'n'; } return true;
                case Keys.O: if (shift) { character = 'O'; } else { character = 'o'; } return true;
                case Keys.P: if (shift) { character = 'P'; } else { character = 'p'; } return true;
                case Keys.Q: if (shift) { character = 'Q'; } else { character = 'q'; } return true;
                case Keys.R: if (shift) { character = 'R'; } else { character = 'r'; } return true;
                case Keys.S: if (shift) { character = 'S'; } else { character = 's'; } return true;
                case Keys.T: if (shift) { character = 'T'; } else { character = 't'; } return true;
                case Keys.U: if (shift) { character = 'U'; } else { character = 'u'; } return true;
                case Keys.V: if (shift) { character = 'V'; } else { character = 'v'; } return true;
                case Keys.W: if (shift) { character = 'W'; } else { character = 'w'; } return true;
                case Keys.X: if (shift) { character = 'X'; } else { character = 'x'; } return true;
                case Keys.Y: if (shift) { character = 'Y'; } else { character = 'y'; } return true;
                case Keys.Z: if (shift) { character = 'Z'; } else { character = 'z'; } return true;

                //Decimal keys
                case Keys.D0: if (shift) { character = '='; } else { character = '0'; } return true;
                case Keys.D1: if (shift) { character = '!'; } else { character = '1'; } return true;
                case Keys.D2: if (shift) { character = '"'; } else { character = '2'; } return true;
                case Keys.D3: if (shift) { character = '§'; } else { character = '3'; } return true;
                case Keys.D4: if (shift) { character = '$'; } else { character = '4'; } return true;
                case Keys.D5: if (shift) { character = '%'; } else { character = '5'; } return true;
                case Keys.D6: if (shift) { character = '&'; } else { character = '6'; } return true;
                case Keys.D7: if (shift) { character = '/'; } else { character = '7'; } return true;
                case Keys.D8: if (shift) { character = '('; } else { character = '8'; } return true;
                case Keys.D9: if (shift) { character = ')'; } else { character = '9'; } return true;

                //Decimal numpad keys
                case Keys.NumPad0: character = '0'; return true;
                case Keys.NumPad1: character = '1'; return true;
                case Keys.NumPad2: character = '2'; return true;
                case Keys.NumPad3: character = '3'; return true;
                case Keys.NumPad4: character = '4'; return true;
                case Keys.NumPad5: character = '5'; return true;
                case Keys.NumPad6: character = '6'; return true;
                case Keys.NumPad7: character = '7'; return true;
                case Keys.NumPad8: character = '8'; return true;
                case Keys.NumPad9: character = '9'; return true;

                //Special keys
                case Keys.OemTilde: if (shift) { character = '~'; } else { character = '`'; } return true;
                case Keys.OemSemicolon: if (shift) { character = ':'; } else { character = ';'; } return true;
                case Keys.OemQuotes: if (shift) { character = '"'; } else { character = '\''; } return true;
                case Keys.OemQuestion: if (shift) { character = '?'; } else { character = '/'; } return true;
                case Keys.OemPlus: if (shift) { character = '+'; } else { character = '='; } return true;
                case Keys.OemPipe: if (shift) { character = '|'; } else { character = '\\'; } return true;
                case Keys.OemPeriod: if (shift) { character = '>'; } else { character = '.'; } return true;
                case Keys.OemOpenBrackets: if (shift) { character = '{'; } else { character = '['; } return true;
                case Keys.OemCloseBrackets: if (shift) { character = '}'; } else { character = ']'; } return true;
                case Keys.OemMinus: if (shift) { character = '_'; } else { character = '-'; } return true;
                case Keys.OemComma: if (shift) { character = '<'; } else { character = ','; } return true;
                case Keys.Space: character = ' '; return true;
            }
       
            character = (char)0;
            return false;
        }
}
}
