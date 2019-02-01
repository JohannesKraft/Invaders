using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Invaders.Interface
{
    public class SelectionBox : Button
    {
        public string[] Strings { get; set; }
        public int Index { get => index; set => indexChanged(value); }

        public Button LeftButton;
        public Button RightButton;

        public event ControlEventHandler IndexChanged;

        private int index;

        public SelectionBox(GraphicsDevice device) : base(device)
        {
            index = 0;
            Strings = new string[1];
            Strings[0] = "";

            this.Updated += SelectionBox_Updated;
            this.SizeChanged += SelectionBox_SizeChanged;

            LeftButton = new Button(device)
            {
                Bounds = new Rectangle(),
                Alignment = ContentAlignment.MiddleLeft,
                Name = "Left",
                Text = "<",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = this.Font,
                Tag = -1
            };

            RightButton = new Button(device)
            {
                Bounds = new Rectangle(),
                Alignment = ContentAlignment.MiddleRight,
                Name = "Right",
                Text = ">",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = this.Font,
                Tag = 1
            };

            LeftButton.MouseClick += Button_OnClick;
            RightButton.MouseClick += Button_OnClick;

            this.AddControl(LeftButton);
            this.AddControl(RightButton);
        }

        private void SelectionBox_SizeChanged(object sender)
        {
            LeftButton.Bounds = new Rectangle(LeftButton.Bounds.Location, new Point(this.Bounds.Width / 10, this.Bounds.Height));
            RightButton.Bounds = new Rectangle(RightButton.Bounds.Location, new Point(this.Bounds.Width / 10, this.Bounds.Height));
            LeftButton.ReAlign();
            RightButton.ReAlign();
        }

        private void SelectionBox_Updated(object sender)
        {
            Text = Strings[index];
        }

        private void Button_OnClick(object sender, Vector2 location, MouseButtons button, ButtonState state)
        {
            Button btn = sender as Button;

            if (button == MouseButtons.Left && state == ButtonState.Released)
            {
                if (index + (int)btn.Tag >= 0 && index + (int)btn.Tag < Strings.Length)
                {
                    Index += (int)btn.Tag;
                }
            }
        }

        private void indexChanged(int val)
        {
            index = val;
            Text = Strings[index];

            IndexChanged?.Invoke(this);
        }
    }
}
