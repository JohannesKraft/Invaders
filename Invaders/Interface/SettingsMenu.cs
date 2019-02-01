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
    public class SettingsMenu : Control
    {
        public Texture2D ButtonTexture { get; set; }

        private InterfaceManager manager;
        private GraphicsDevice device;

        private Control backgroundPanel;
        private SelectionBox resolutionSBox;
        private CheckBox fullscreenCheckBox;

        private Button applyButton;
        private Button backButton;

        public SettingsMenu(GraphicsDevice device, InterfaceManager manager) : base(device)
        {
            this.Bounds = manager.Bounds;
            this.manager = manager;
            this.device = device;
        }

        public void CreateControls()
        {
            backgroundPanel = new Control(device)
            {
                Bounds = new Rectangle(new Point(), (this.Bounds.Size.ToVector2() * 2 / 3).ToPoint()),
                Alignment = ContentAlignment.MiddleCenter,
                BackgroundColor = new Color(5, 60, 147, 255),
                Name = "BackPanel",

                Focused = true
            };

            Label titleLabel = new Label(device)
            {
                Font = MenuFont_Title,
                FontColor = Color.Orange,
                Text = "Settings",
                Alignment = ContentAlignment.TopCenter,
            };

            this.AddControl(backgroundPanel);
            this.AddControl(titleLabel);

            // Resolution
            Label resolutionLabel = new Label(device)
            {
                Bounds = new Rectangle(backgroundPanel.Bounds.Location + new Point(100, 50), new Point()),
                Font = MenuFont_Text,
                FontColor = Color.Orange,
                Text = "Resolutions:",
                Alignment = ContentAlignment.None
            };

            resolutionSBox = new SelectionBox(device)
            {
                Bounds = new Rectangle(backgroundPanel.Bounds.Location + new Point(100, 100), new Point(300, 70)),
                BackgroundColor = Color.Gray,
                Font = MenuFont_Text,
                Strings = new string[] {"800x450", "960x540", "1280x720", "1600x900", "1920x1080", "2560x1440", "3200x1800", "3840x2160", "4096x2304", "5120x2880", "7680x4320" }
            };

            Label fullscreenLabel = new Label(device)
            {
                Bounds = new Rectangle(backgroundPanel.Bounds.Location + new Point(600, 50), new Point()),
                Font = MenuFont_Text,
                FontColor = Color.Orange,
                Text = "Fullscreen:",
                Alignment = ContentAlignment.None
            };

            fullscreenCheckBox = new CheckBox(device)
            {
                Bounds = new Rectangle(backgroundPanel.Bounds.Location + new Point(600, 100), new Point(70, 70)),
                BackgroundColor = Color.Gray,
                Checked = false,
            };


            // Control Buttons
            applyButton = new Button(device)
            {
                Bounds = new Rectangle(0, 0, 250, 70),
                Alignment = ContentAlignment.BottomRight,
                BackgroundColor = Color.Blue,
                BackgroundTexture = ButtonTexture,
                Enabled = true,
                Visible = true,
                Name = "ApplyButton",
                Text = "Apply Settings",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = MenuFont_Text,
                FontColor = Color.Orange,
            };

            backButton = new Button(device)
            {
                Bounds = new Rectangle(0, 0, 250, 70),
                Alignment = ContentAlignment.BottomLeft,
                BackgroundColor = Color.Blue,
                BackgroundTexture = ButtonTexture,
                Enabled = true,
                Visible = true,
                Name = "BackButton",
                Text = "Back to Menu",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = MenuFont_Text,
                FontColor = Color.Orange,
            };
            applyButton.MouseClick += Button_OnClick;
            applyButton.MouseEnter += Button_MouseEnter;
            applyButton.MouseLeave += Button_MouseLeave;

            backButton.MouseClick += Button_OnClick;
            backButton.MouseEnter += Button_MouseEnter;
            backButton.MouseLeave += Button_MouseLeave;

            backgroundPanel.AddControl(resolutionSBox);
            backgroundPanel.AddControl(resolutionLabel);
            backgroundPanel.AddControl(fullscreenCheckBox);
            backgroundPanel.AddControl(fullscreenLabel);
            backgroundPanel.AddControl(applyButton);
            backgroundPanel.AddControl(backButton);
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

                if (btn.Name.Contains("BackButton"))
                {
                    manager.State = InterfaceManager.MenueState.MainMenu;
                }

                if (btn.Name.Contains("ApplyButton"))
                {
                    manager.UpdateSettings(resolutionSBox.Text, fullscreenCheckBox.Checked);
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
