using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Invaders.Interface
{
    class TextBox : Control
    {
        public string Text { get => text; set { text = value; TextChanged?.Invoke(this); } }
        public ContentAlignment TextAlign { get; set; }
        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public event ControlEventHandler TextChanged;

        private string text;

        public TextBox(GraphicsDevice device) : base(device)
        {
            text = "";
            TextAlign = ContentAlignment.MiddleCenter;
            FontColor = Color.Black;
            Drawn += TextBox_Drawn;
            KeyPressed += TextBox_KeyPressed;
        }

        private void TextBox_KeyPressed(object sender, Keys button, bool shift)
        {
            if (Focused)
            {
                char character;
                if (Utilities.TryConvertKeyboardInput(button, shift, out character))
                {
                    Vector2 size = Font.MeasureString(Text + character);

                    if (size.X < this.Bounds.Size.ToVector2().X / 100 * 90 )
                    {
                        Text += character;
                    }
                }

                if (button == Keys.Back)
                {
                    Text = Text.Substring(0, Math.Max(Text.Length - 1, 0));
                }

            }
        }

        private void TextBox_Drawn(object sender, SpriteBatch batch)
        {
            Vector2 textSize = Font.MeasureString(Text);
            Vector2 loc = Utilities.GetContentAlign(Bounds, TextAlign, textSize);
            batch.DrawString(Font, Text, loc, FontColor);

            if (Focused)
            {
                if (Text == string.Empty)
                {
                    textSize = Font.MeasureString("0");
                    loc = Utilities.GetContentAlign(Bounds, TextAlign, textSize);
                }
                Point cursorLoc = new Vector2(textSize.X + loc.X, loc.Y - 5).ToPoint();
                Rectangle rect = new Rectangle(cursorLoc, new Vector2(7, textSize.Y + 5).ToPoint());
                batch.Draw(whiteTexture, rect, Color.Black);
            }
        }
    }
}
