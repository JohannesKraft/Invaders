using static Invaders.Misc.Config.Graphics.Fonts;
using System;
using Invaders.Misc;
using Invaders.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Invaders.Interface
{
    public class InterfaceManager
    {
        public enum MenueState
        {
            None,
            Intro,
            MainMenu,
            Settings,
            Running,
            Pause,
            Exit
        }

        public MenueState State { get; set; }
        public IControl[] Menus { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Bounds { get; set; }

        public Texture2D ButtonTexture { get; set; }
        public Texture2D BackgroundTexture { get; set; }

        private GameDisplay display;

        private bool escapeReset;

        public InterfaceManager(int width, int height, GameDisplay display, Engine engine)
        {
            State = MenueState.None;
            Width = width;
            Height = height;
            Bounds = new Rectangle(0, 0, this.Width, this.Height);
            Menus = new IControl[3];
            this.display = display;

            escapeReset = true;
        }

        public void CreateMenus()
        {
            MainMenu mMenu = new MainMenu(display.GraphicsDevice, this)
            {
                Bounds = this.Bounds,
                Enabled = true,
                Visible = true,
                BackgroundColor = Color.White,
                BackgroundTexture = BackgroundTexture,
                ButtonTexture = ButtonTexture
            };
            mMenu.CreateControls();

            SettingsMenu sMenu = new SettingsMenu(display.GraphicsDevice, this)
            {
                Bounds = this.Bounds,
                Enabled = true,
                Visible = true,
                BackgroundColor = Color.White,
                BackgroundTexture = BackgroundTexture,
                ButtonTexture = ButtonTexture
            };
            sMenu.CreateControls();

            Menus[0] = mMenu;
            Menus[1] = sMenu;
            State = MenueState.MainMenu;
        }

        public void Update(double totalMilliseconds, KeyboardState kstate, MouseState mstate, GamePadState gpstate)
        {
            if (escapeReset && (kstate.IsKeyDown(Keys.P) || gpstate.IsButtonDown(Buttons.Back)))
            {
                if (State == MenueState.Pause)
                {
                    State = MenueState.Running;
                }
                else
                {
                    State = MenueState.Pause;
                }

                escapeReset = false;
            }

            if (kstate.IsKeyUp(Keys.P) && gpstate.IsButtonUp(Buttons.Back))
            {
                escapeReset = true;
            }

            switch (State)
            {
                case MenueState.MainMenu:
                    Menus[0].Update(mstate, kstate);
                    break;
                case MenueState.Settings:
                    Menus[1].Update(mstate, kstate);
                    break;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            switch (State)
            {
                case MenueState.MainMenu:
                    Menus[0].Draw(batch);
                    break;
                case MenueState.Settings:
                    Menus[1].Draw(batch);
                    break;
            }
        }

        public void UpdateSettings(string resolution, bool fullscreen)
        {
            int div = resolution.IndexOf("x");
            int x = Convert.ToInt32(resolution.Substring(0, div));
            int y = Convert.ToInt32(resolution.Substring(div + 1, resolution.Length - div - 1));

            Config.ScalingFactor = x / Config.StaticResolution.X;

            display.Graphics.PreferredBackBufferWidth = x;
            display.Graphics.PreferredBackBufferHeight = y;

            display.Graphics.IsFullScreen = fullscreen;
            display.Graphics.ApplyChanges();
        }
    }
}
