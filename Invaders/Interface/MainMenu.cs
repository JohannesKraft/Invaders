using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Invaders.Misc.Config.Graphics.Fonts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Invaders.Interface
{
    public class MainMenu : Control
    {
        public Texture2D ButtonTexture { get; set; }

        private InterfaceManager manager;
        private GraphicsDevice device;

        private Button startButton;
        private Button settingsButton;
        private TextBox testTextBox;
        private TextBox testTextBox1;

        public MainMenu(GraphicsDevice device, InterfaceManager manager) : base(device)
        {
            this.Bounds = manager.Bounds;
            this.manager = manager;
            this.device = device;   
        }

        public void CreateControls()
        {
            startButton = new Button(device)
            {
                Bounds = new Rectangle(700, 650, 200, 70),
                BackgroundColor = Color.Blue,
                BackgroundTexture = ButtonTexture,
                Enabled = true,
                Visible = true,
                Name = "StartButton",
                Text = "Start Game",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = MenuFont_Text,
                FontColor = Color.Orange,
            };

            settingsButton = new Button(device)
            {
                Bounds = new Rectangle(700, 740, 200, 70),
                BackgroundColor = Color.Blue,
                BackgroundTexture = ButtonTexture,
                Enabled = true,
                Visible = true,
                Name = "SettingsButton",
                Text = "Settings",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = MenuFont_Text,
                FontColor = Color.Orange,
            };

            testTextBox = new TextBox(device)
            {
                Bounds = new Rectangle(100, 100, 300, 70),
                BackgroundColor = Color.Blue,
                Enabled = true,
                Visible = true,
                Name = "TextBox",
                Text = "",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = MenuFont_Text
            };

            testTextBox1 = new TextBox(device)
            {
                Bounds = new Rectangle(100, 200, 300, 70),
                BackgroundColor = Color.Blue,
                Enabled = true,
                Visible = true,
                Name = "TextBox",
                Text = "",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = MenuFont_Text
            };

            startButton.MouseClick += Button_OnClick;
            startButton.MouseEnter += Button_MouseEnter;
            startButton.MouseLeave += Button_MouseLeave;

            settingsButton.MouseClick += Button_OnClick;
            settingsButton.MouseEnter += Button_MouseEnter;
            settingsButton.MouseLeave += Button_MouseLeave;

            this.AddControl(startButton);
            this.AddControl(settingsButton);
        }

        private void Button_OnClick(object sender, Vector2 location, MouseButtons button, ButtonState state)
        {
            Button btn = sender as Button;

            if (button == MouseButtons.Left && state == ButtonState.Pressed)
            {
                btn.BackgroundColor = Color.Orange;
                btn.FontColor = Color.DarkRed;
            }
            else if (button == MouseButtons.Left && state == ButtonState.Released)
            {
                btn.BackgroundColor = Color.SlateGray;
                btn.FontColor = Color.OrangeRed;

                if (btn.Name.Contains("StartButton"))
                {
                    manager.State = InterfaceManager.MenueState.Running;
                }
                else if (btn.Name.Contains("SettingsButton"))
                {
                    manager.State = InterfaceManager.MenueState.Settings;
                }
            }
        }

        private void Button_MouseLeave(object sender, Vector2 location, MouseButtons button, ButtonState state)
        {
            Button btn = sender as Button;

            btn.BackgroundColor = Color.Blue;
            btn.FontColor = Color.Orange;
        }

        private void Button_MouseEnter(object sender, Vector2 location, MouseButtons button, ButtonState state)
        {
            Button btn = sender as Button;

            btn.BackgroundColor = Color.SlateGray;
            btn.FontColor = Color.OrangeRed;
        }
    }
}
