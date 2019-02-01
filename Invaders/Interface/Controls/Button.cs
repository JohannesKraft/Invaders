using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Invaders.Interface
{
    public class Button : Control
    {
        public string Text { get; set; }
        public ContentAlignment TextAlign { get; set; }
        public SpriteFont Font { get => font; set => fontChanged(value); }
        public Color FontColor { get; set; }

        private SpriteFont font;

        public Button(GraphicsDevice device) : base(device)
        {
            Text = "";
            TextAlign = ContentAlignment.MiddleCenter;
            FontColor = Color.Black;
            Drawn += Button_Drawn;
        }

        private void Button_Drawn(object sender, SpriteBatch batch)
        {
            Vector2 loc = Utilities.GetContentAlign(Bounds, TextAlign, Font.MeasureString(Text));
            batch.DrawString(Font, Text, loc, FontColor);
        }

        private void fontChanged(SpriteFont val)
        {
            font = val;
            foreach (var child in Children)
            {
                if (child is Button)
                {
                    Button btn = child as Button;
                    btn.Font = val;
                }
            }
        }
    }
}
