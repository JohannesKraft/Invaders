using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Invaders.Misc;

namespace Invaders.Interface
{
    public class Control : IControl
    {
        public string Name { get; set; }
        public Rectangle Bounds { get => bounds; set => boundsChanged(value, bounds); }
        public Texture2D BackgroundTexture { get; set; }
        public Color BackgroundColor { get; set; }
        public IControl Parent { get => parent; set => parentChanged(value); }
        public ContentAlignment Alignment { get => alignment; set => alignmentChanged(value); }
        public IControl[] Children { get => children.ToArray(); set => children = new List<IControl>(value); }
        public object Tag { get; set; }

        public bool Visible { get; set; }
        public bool Enabled { get; set; }
        public bool Focused { get => focused; set => focusChanged(value); }

        public event ControlMouseEventHandler MouseClick;
        public event ControlMouseEventHandler MouseEnter;
        public event ControlMouseEventHandler MouseLeave;

        public event ControlKeyboardEventHandler KeyPressed;
        public event ControlKeyboardEventHandler KeyReleased;

        public event ControlEventHandler Updated;
        public event ControlDrawEventHandler Drawn;

        public event ControlEventHandler SizeChanged;
        public event ControlEventHandler LocationChanged;

        private Rectangle bounds;
        private ContentAlignment alignment;
        private IControl parent;
        private List<IControl> children;
        private bool focused;

        protected Texture2D whiteTexture;
        private GraphicsDevice device;
        private bool mouseOver;
        private ButtonState[] mouseButtonStates;
        private Keys[] keyStates;

        public Control(GraphicsDevice device)
        {
            this.Visible = true;
            this.Enabled = true;
            this.BackgroundColor = Color.White;

            this.bounds = new Rectangle(0, 0, 0, 0);
            this.alignment = ContentAlignment.None;
            this.children = new List<IControl>();
            this.focused = false;

            this.device = device;
            this.mouseButtonStates = new ButtonState[3];
            this.keyStates = new Keys[0];
            this.mouseOver = false;

            for (int i = 0; i < mouseButtonStates.Length; i++)
            {
                mouseButtonStates[i] = ButtonState.Released;
            }

            whiteTexture = new Texture2D(device, 1, 1);
            whiteTexture.SetData<Color>(new Color[1] { Color.White });
        }

        public void Draw(SpriteBatch batch)
        {
            if (!Visible)
            {
                return;
            }

            batch.Draw(whiteTexture, Bounds, BackgroundColor);
            if (BackgroundTexture != null)
            {
                batch.Draw(BackgroundTexture, Bounds, Color.White);
            }

            foreach (var control in Children)
            {
                control.Draw(batch);
            }

            Drawn?.Invoke(this, batch);
        }

        public void Update(MouseState mstate, KeyboardState kstate)
        {
            if (!Enabled)
            {
                return;
            }

            updateMouse(mstate);
            updateKeyboard(kstate);

            foreach (var control in Children)
            {
                control.Update(mstate, kstate);
            }

            Updated?.Invoke(this);
        }

        public void AddControl(IControl control)
        {
            children.Add(control);
            control.Parent = this;
        }

        public void ReAlign()
        {
            alignmentChanged(this.alignment);
        }

        public void Unfocus(IControl sender)
        {
            this.focused = false;
            if (Parent != null)
            {
                if (sender.CompareTo(Parent))
                {
                    foreach (var child in Children)
                    {
                        child.Unfocus(this);
                    }
                }
            }
            bool isChild = false;
            foreach (var child in Children)
            {
                if (sender.CompareTo(child))
                {
                    isChild = true;
                    break;
                }
            }
            if (isChild && Parent != null)
            {
                Parent.Unfocus(this);
            }

        }

        public bool CompareTo(IControl control)
        {
            return this.Bounds == control.Bounds && this.Name == control.Name && this.Alignment == control.Alignment && this.Visible == control.Visible && this.Enabled == control.Enabled;
        }

        private void updateMouse(MouseState mstate)
        {
            Vector2 location = Config.ScreenLocationToPoint(mstate.Position.ToVector2());
            if (Bounds.Contains(location.ToPoint()))
            {
                if (!mouseOver)
                {
                    mouseOver = true;
                    MouseEnter?.Invoke(this, location, MouseButtons.None, ButtonState.Released);
                }

                // Left Mouse button
                if (mstate.LeftButton == ButtonState.Pressed && mouseButtonStates[0] != ButtonState.Pressed)
                {
                    mouseButtonStates[0] = ButtonState.Pressed;
                    MouseClick?.Invoke(this, location, MouseButtons.Left, ButtonState.Pressed);
                }
                else if (mstate.LeftButton == ButtonState.Released && mouseButtonStates[0] != ButtonState.Released)
                {
                    mouseButtonStates[0] = ButtonState.Released;
                    MouseClick?.Invoke(this, location, MouseButtons.Left, ButtonState.Released);

                    Focused = true;
                }

                // Right Mouse Button
                if (mstate.RightButton == ButtonState.Pressed && mouseButtonStates[1] != ButtonState.Pressed)
                {
                    mouseButtonStates[1] = ButtonState.Pressed;
                    MouseClick?.Invoke(this, location, MouseButtons.Right, ButtonState.Pressed);
                }
                else if (mstate.RightButton == ButtonState.Released && mouseButtonStates[1] != ButtonState.Released)
                {
                    mouseButtonStates[1] = ButtonState.Released;
                    MouseClick?.Invoke(this, location, MouseButtons.Right, ButtonState.Released);
                }

                // Middle Mouse Button
                if (mstate.MiddleButton == ButtonState.Pressed && mouseButtonStates[2] != ButtonState.Pressed)
                {
                    mouseButtonStates[2] = ButtonState.Pressed;
                    MouseClick?.Invoke(this, location, MouseButtons.Middle, ButtonState.Pressed);
                }
                else if (mstate.MiddleButton == ButtonState.Released && mouseButtonStates[2] != ButtonState.Released)
                {
                    mouseButtonStates[2] = ButtonState.Released;
                    MouseClick?.Invoke(this, location, MouseButtons.Middle, ButtonState.Released);
                }
            }
            else if (mouseOver)
            {
                mouseOver = false;
                for (int i = 0; i < mouseButtonStates.Length; i++)
                {
                    mouseButtonStates[i] = ButtonState.Released;
                }

                MouseLeave?.Invoke(this, location, MouseButtons.None, ButtonState.Released);
            }
        }

        private void updateKeyboard(KeyboardState kstate)
        {
            Keys[] pressed = kstate.GetPressedKeys();
            bool shift = kstate.IsKeyDown(Keys.LeftShift) || kstate.IsKeyDown(Keys.RightShift);
            Keys[] diffPressed = pressed.Except(keyStates).ToArray();
            Keys[] diffReleased = pressed.Except(keyStates).ToArray();

            if (diffPressed.Length != 0)
            {
                foreach (var key in diffPressed)
                {
                    KeyPressed?.Invoke(this, key, shift);
                }
            }
            if (diffReleased.Length != 0)
            {
                foreach (var key in diffReleased)
                {
                    KeyReleased?.Invoke(this, key, shift);
                }
            }

            keyStates = pressed;
        }

        private void boundsChanged(Rectangle newBounds, Rectangle oldBounds)
        {
            Point change = newBounds.Location - oldBounds.Location;
            this.bounds = newBounds;

            foreach (var child in children)
            {
                if (child.Alignment == ContentAlignment.None)
                {
                    child.Bounds = new Rectangle(child.Bounds.Location + change, child.Bounds.Size);
                }
                else if (child.Alignment != ContentAlignment.None)
                {
                    child.Bounds = new Rectangle(Utilities.GetContentAlign(bounds, child.Alignment, child.Bounds.Size.ToVector2()).ToPoint(), child.Bounds.Size);
                }
            }

            if (newBounds.Location != oldBounds.Location)
            {
                LocationChanged?.Invoke(this);
            }

            if (newBounds.Size != oldBounds.Size)
            {
                SizeChanged?.Invoke(this);
            }
        }

        private void alignmentChanged(ContentAlignment align)
        {
            if (align != ContentAlignment.None && Parent != null)
            {
                this.Bounds = new Rectangle(Utilities.GetContentAlign(Parent.Bounds, align, bounds.Size.ToVector2()).ToPoint(), bounds.Size);
            }

            alignment = align;
        }

        private void parentChanged(IControl p)
        {
            parent = p;
            alignmentChanged(alignment);
        }

        private void focusChanged(bool val)
        {
            focused = val;
            if (focused)
            {
                if (Parent != null)
                {
                    Parent.Unfocus(this);
                }
                foreach (var child in Children)
                {
                    child.Unfocus(this);
                }
            }
        }


    }
}
