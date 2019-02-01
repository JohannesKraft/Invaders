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
    public delegate void ControlEventHandler(object sender);
    public delegate void ControlMouseEventHandler(object sender, Vector2 location, MouseButtons button, ButtonState state);
    public delegate void ControlKeyboardEventHandler(object sender, Keys button, bool shift);
    public delegate void ControlDrawEventHandler(object sender, SpriteBatch batch);

    public interface IControl
    {
        string Name { get; set; }
        Rectangle Bounds { get; set; }
        Color BackgroundColor { get; set; }
        Texture2D BackgroundTexture { get; set; }
        IControl Parent { get; set; }
        ContentAlignment Alignment { get; set; }
        IControl[] Children { get; set; }
        object Tag { get; set; }

        bool Visible { get; set; }
        bool Enabled { get; set; }
        bool Focused { get; set; }

        event ControlEventHandler Updated;
        event ControlDrawEventHandler Drawn;

        event ControlEventHandler SizeChanged;
        event ControlEventHandler LocationChanged;

        event ControlMouseEventHandler MouseClick;
        event ControlMouseEventHandler MouseEnter;
        event ControlMouseEventHandler MouseLeave;

        event ControlKeyboardEventHandler KeyPressed;
        event ControlKeyboardEventHandler KeyReleased;

        void Update(MouseState mstate, KeyboardState kstate);
        void Draw(SpriteBatch batch);
        void AddControl(IControl control);
        void ReAlign();
        void Unfocus(IControl sender);
        bool CompareTo(IControl control);
    }
}
