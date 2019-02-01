using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Invaders.Game
{
    public class Engine
    {
        public enum EngineState
        {
            None,
            GameRunning,
            GamePaused,
            GameWon,
            GameLost
        }

        public EngineState State { get => state;}

        public const int Tickrate = 60;

        public int Width { get; set; }
        public int Height { get; set; }

        public long Ticks { get => ticks; }

        public Player Player { get; set; }
        public EnemyList Enemies { get; set; }
        public StructureList Structures { get; set; }
        public ProjectileList Projectiles { get; set; }

        public Score Score { get; set; }

        public delegate void EngineEventHandler(object sender);
        public event EngineEventHandler GameLost;

        private EngineState state;
        private int ticks = 0;
        private Rectangle bounds;
        private bool isRunning = false;
        private double gameRuntime;

        public Engine(int width, int height)
        {
            state = EngineState.None;
            Width = width;
            Height = height;
            bounds = new Rectangle(0, 0, this.Width, this.Height);

            Player = new Player();
            Enemies = new EnemyList();
            Structures = new StructureList();
            Projectiles = new ProjectileList(new Player[] { Player }, Structures, Enemies, bounds);

            Score = new Score(Player, Enemies);
        }

        public void StartEngine()
        {
            state = EngineState.GameRunning;
            LoadContent();
            isRunning = true;
        }

        public void StopEngine()
        {
            state = EngineState.None;
            isRunning = false;
        }

        public void RestartEngine()
        {
            state = EngineState.GameRunning;
            isRunning = true;
            LoadContent();
            Player.Revive();
            Enemies.RestoreEnemies();
            Projectiles.Clear();
            Score.Value = 0;
        }

        public void Update(double totalMilliseconds, KeyboardState kstate, MouseState mstate, GamePadState gpstate)
        {
            int milliseconds = 1000 / Tickrate;
            if (totalMilliseconds - gameRuntime >= milliseconds)
            {
                //End Game
                if (Enemies.Enemies.Length == 0)
                {
                    state = EngineState.GameWon;
                    isRunning = false;
                }
                else if(!Player.IsAlive)
                {
                    if (State != EngineState.GameLost)
                    {
                        GameLost.Invoke(this);
                    }
                    state = EngineState.GameLost;
                    isRunning = false;
                }

                // Restart Engine if enter is pressed
                if ((kstate.IsKeyDown(Keys.Enter) || gpstate.Buttons.Start == ButtonState.Pressed) && !isRunning)
                {
                    RestartEngine();
                }

                if (!isRunning)
                {
                    return;
                }

                //Movement
                bool pressed = (kstate.IsKeyDown(Keys.D) || kstate.IsKeyDown(Keys.A)) && (kstate.IsKeyDown(Keys.D) != kstate.IsKeyDown(Keys.A));
                bool right = kstate.IsKeyDown(Keys.D);
                float direction = 0.0F;

                if (pressed && right)
                {
                    direction = 1.0F;
                }
                else if(pressed && !right)
                {
                    direction = -1.0F;
                }
                else if(gpstate.IsConnected && gpstate.ThumbSticks.Left.Length() > 0.0F)
                {
                    direction = gpstate.ThumbSticks.Left.X;
                }
                Player.Move(direction, bounds);

                //Shoot
                if (mstate.LeftButton == ButtonState.Pressed || gpstate.Buttons.A == ButtonState.Pressed)
                {
                    Player.Shoot(Projectiles, Ticks);
                }

                // Enemies
                Enemies.Move(Projectiles);

                //projectiles
                Projectiles.Move();

                gameRuntime = totalMilliseconds;
                ticks++;
            }
            
        }

        private void LoadContent()
        {
            Player.Center = new Point(Width / 2, Height / 10 * 9);
            Enemies.Bounds = bounds.Size;

            Structures.Create();
        }
    }
}
