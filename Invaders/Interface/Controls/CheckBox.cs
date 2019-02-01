using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Invaders.Interface
{
    public class CheckBox : Control
    {
        public bool Checked { get { return @checked; } set { @checked = value; CheckedChanged?.Invoke(this); } }
        public Texture2D TickTexture { get; set; }

        public event ControlEventHandler CheckedChanged;

        private bool @checked;

        public CheckBox(GraphicsDevice device) : base(device)
        {
            this.@checked = false;

            this.Drawn += CheckBox_Drawn;
            this.MouseClick += CheckBox_MouseClick;
        }

        private void CheckBox_MouseClick(object sender, Vector2 location, MouseButtons button, Microsoft.Xna.Framework.Input.ButtonState state)
        {
            if (button == MouseButtons.Left && state == ButtonState.Released)
            {
                Checked = !@checked;
            }
        }

        private void CheckBox_Drawn(object sender, SpriteBatch batch)
        {
            if (TickTexture != null && @checked)
            {
                batch.Draw(TickTexture, Bounds, Color.White);
            }
            else if (@checked)
            {
                Rectangle rect = new Rectangle(Bounds.X + Bounds.Width / 10, Bounds.Y + Bounds.Height / 10, Bounds.Width / 10 * 8, Bounds.Height / 10 * 8);
                batch.Draw(whiteTexture, rect, Color.Black);
            }
        }
    }
}
