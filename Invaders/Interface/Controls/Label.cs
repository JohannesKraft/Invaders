using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Invaders.Interface
{
    public class Label : Control
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public Label(GraphicsDevice device) : base(device)
        {
            BackgroundColor = Color.Transparent;
            Text = "";
            FontColor = Color.Black;
            Drawn += Label_Drawn;
            SizeChanged += Label_SizeChanged;
        }

        private void Label_SizeChanged(object sender)
        {
            ReAlign();
        }

        private void Label_Drawn(object sender, SpriteBatch batch)
        {
            Bounds = new Rectangle(Bounds.Location, Font.MeasureString(Text).ToPoint());
            batch.DrawString(Font, Text, Bounds.Location.ToVector2(), FontColor);
        }
    }
}
