using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Invaders.Game;
using Invaders.GameGraphics;
using Invaders.Misc;
using Invaders.Interface;
using static Invaders.Misc.Config.Graphics.Fonts;
using Microsoft.Xna.Framework.Media;

namespace Invaders
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public partial class GameDisplay : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager Graphics { get; set; }
        SpriteBatch spriteBatch;

        InterfaceManager interfaceManager;
        Engine gameEngine;
        AnimationManager animationManager;
        ParticleEngine particleEngine;

        double prevTime = 0;
        int frames = 0;
        int framerate = 0;

        public GameDisplay()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Config.AddConfig(@"content/config/graphics.cfg");
            Config.LoadValues();

            Graphics.PreferredBackBufferWidth = (int)Config.Resolution.X;
            Graphics.PreferredBackBufferHeight = (int)Config.Resolution.Y;
            if (!Graphics.IsFullScreen)
                //graphics.ToggleFullScreen();
            Graphics.ApplyChanges();

            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            interfaceManager = new InterfaceManager((int)Config.StaticResolution.X, (int)Config.StaticResolution.Y, this, gameEngine);
            gameEngine = new Engine((int)Config.StaticResolution.X, (int)Config.StaticResolution.Y);
            particleEngine = new ParticleEngine();
            animationManager = new AnimationManager();

            LoadEngineData();
            LoadTextures();
            interfaceManager.CreateMenus();

            AnimationSequence playerSequence = new AnimationSequence(gameEngine.Player.Hitbox, 0, (int)Config.Graphics.AnimationType.Player_Movement, Color.White);
            playerSequence.Add(0, 4, 100);
            playerSequence.Add(5, 9, 100);
            animationManager.AddAnimationSequence(playerSequence);
            gameEngine.Player.Tag = playerSequence;

            MediaPlayer.Volume = 0.2F;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundSong); 
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            interfaceManager.Update(gameTime.TotalGameTime.TotalMilliseconds, Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

            if (interfaceManager.State == InterfaceManager.MenueState.Running)
            {
                gameEngine.Update(gameTime.TotalGameTime.TotalMilliseconds, Keyboard.GetState(), Mouse.GetState(), GamePad.GetState(PlayerIndex.One));
                particleEngine.Update(gameTime.TotalGameTime.TotalMilliseconds);
                animationManager.Update(gameTime.TotalGameTime.TotalMilliseconds);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SteelBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
            Matrix.CreateScale(Config.ScalingFactor));
            if (interfaceManager.State == InterfaceManager.MenueState.Running || interfaceManager.State == InterfaceManager.MenueState.Pause)
            {
                DrawGame(gameTime);
            }

            interfaceManager.Draw(spriteBatch);

            // Framerate
            frames++;
            if (prevTime + 1000 <= gameTime.TotalGameTime.TotalMilliseconds)
            {
                framerate = frames;
                frames = 0;
                prevTime = gameTime.TotalGameTime.TotalMilliseconds;
            }

            spriteBatch.DrawString(DefaultFont, framerate.ToString(), new Vector2(0, 0), Color.Red);

            Vector2 loc = Mouse.GetState().Position.ToVector2();
            loc = Config.ScreenLocationToPoint(loc);
            spriteBatch.Draw(whiteTexture, new Rectangle(loc.ToPoint(), new Point(10, 10)), Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawGame(GameTime gameTime)
        {
            spriteBatch.Draw(backgroundTexture, new Rectangle(new Point(0, 0), Config.StaticResolution.ToPoint()), Color.White);

            // Player
            if (gameEngine.Player.IsAlive)
            {
                Vector2 speed = gameEngine.Player.CurrentSpeed;
                float angle = VectorMath.VectorToAngle(speed); // get direction angle
                AnimationSequence seq = (AnimationSequence)gameEngine.Player.Tag;
                seq.DestionationRectangle = gameEngine.Player.Hitbox;

                if (angle == MathHelper.ToRadians(-90) && seq.ActiveSequence.CurrentFrame == seq.ActiveSequence.StartIndex) // if angle = -90° and animation is finished
                {
                    seq.Play(2);
                }
                else if (angle == MathHelper.ToRadians(-90) && seq.SequenceIndex == 1) // if angle = -90° and animation 1 is played
                {
                    seq.ActiveSequence.Reversed = true;
                }

                if (angle == MathHelper.ToRadians(90) && seq.ActiveSequence.CurrentFrame == seq.ActiveSequence.StartIndex) // if angle = 90° and animation is finished
                {
                    seq.Play(1);
                }
                else if (angle == MathHelper.ToRadians(90) && seq.SequenceIndex == 2) // if angle = -90° and animation 2 is played
                {
                    seq.ActiveSequence.Reversed = true;
                }

                if (angle == MathHelper.Pi && (seq.SequenceIndex == 1 || seq.SequenceIndex == 2)) // if player is not moveing
                {
                    seq.ActiveSequence.Reversed = true;
                }
            }
            else
            {

            }

            // Structures
            for (int i = 0; i < gameEngine.Structures.Structures.Length; i++)
            {
                Structure str = gameEngine.Structures.Structures[i];
                if (!str.Destroyed)
                {
                    if (str.RecentlyChanged)
                    {
                        structureTextures[i] = ImageProcessing.TextureFromBitmap(this.GraphicsDevice, str.StructureBitmap);
                        str.RecentlyChanged = false;
                    }

                    spriteBatch.Draw(gameEngine.Structures.StructureTexture, str.Hitbox, Color.DimGray);
                    spriteBatch.Draw(structureTextures[i], str.Hitbox, Color.White);
                }
            }

            // Enemies
            foreach (var e in gameEngine.Enemies.Enemies)
            {
                if (e.IsAlive && e.IsSpawned)
                {
                    spriteBatch.Draw(enemyTexture, e.Hitbox, Color.White);
                }
                else if (!e.IsAlive && e.IsSpawned)
                {
                    spriteBatch.Draw(enemyTexture, e.Hitbox, Color.Orange);
                }

            }
            // Particles
            particleEngine.Draw(spriteBatch);

            // Animations
            animationManager.Draw(spriteBatch);

            // Health
            string hp = gameEngine.Player.HealthPoints.ToString();
            spriteBatch.DrawString(MenuFont_Text, hp,
                Utilities.GetContentAlign(gameEngine.Player.Hitbox, ContentAlignment.BottomCenter, MenuFont_Text.MeasureString(hp)),
                Color.Red);

            //Score
            string score = "Score: " + gameEngine.Score.Value.ToString("0.00");
            spriteBatch.DrawString(MenuFont_Text, score,
                Utilities.GetContentAlign(new Rectangle(new Point(), Config.StaticResolution.ToPoint()), ContentAlignment.TopCenter, MenuFont_Text.MeasureString(score)),
                Color.Orange);

            // Projectiles
            if (gameEngine.Projectiles.Projectiles != null)
            {
                for (int i = 0; i < gameEngine.Projectiles.Projectiles.Length; i++)
                {
                    Projectile proj = gameEngine.Projectiles.Projectiles[i];

                    if (!proj.IsExploded && !proj.OutOfBounds)
                    {
                        float angle = VectorMath.VectorToAngle(proj.Speed);
                        Rectangle destRect = new Rectangle(proj.Location.X, proj.Location.Y, 30, 45);
                        Vector2 origin = new Vector2(projectileTexture.Width / 2, projectileTexture.Height / 2);

                        Color col = Color.White;
                        if (proj.Type == Projectile.ProjectileType.Enemy_Proj_Def)
                        {
                            col = Color.LawnGreen;
                        }

                        spriteBatch.Draw(projectileTexture, destRect, null, col, angle, origin, SpriteEffects.None, 0);
                    }
                }
            }

            if (gameEngine.State == Engine.EngineState.GameWon)
            {
                spriteBatch.Draw(endScreenTexture[0], new Rectangle(new Point(0, 0), Config.StaticResolution.ToPoint()), Color.White);
            }
            else if (gameEngine.State == Engine.EngineState.GameLost)
            {
                spriteBatch.Draw(endScreenTexture[1], new Rectangle(new Point(0, 0), Config.StaticResolution.ToPoint()), Color.White);
            }
        }

        private void Projectiles_ProjectileMoved(object sender)
        {
            Projectile proj = sender as Projectile;

            Point size = new Point(10, 50);
            Point loc = new Point(proj.Location.X - size.X / 2, proj.Location.Y - size.Y / 2);
            float rotation = VectorMath.VectorToAngle(proj.Speed);

            // Move Trail Emitter
            ParticleEmitter e = proj.Tag as ParticleEmitter;
            e.Location = proj.Location.ToVector2();
        }

        private void Projectiles_ProjectileFired(object sender)
        {
            Projectile proj = sender as Projectile;

            // Muzzleflash Animation
            Point size = new Point(50, 60);
            Point loc = new Point(proj.Location.X - size.X / 2, proj.Location.Y - size.Y / 2);
            float rotation = VectorMath.VectorToAngle(proj.Speed);
            Color col = Color.White;
            if (proj.Type == Projectile.ProjectileType.Enemy_Proj_Def)
            {
                col = Color.Green;
            }
            animationManager.AddAnimation(240, 12, 1, new Rectangle(loc, size), rotation, (int)Config.Graphics.AnimationType.Projectile_Explosion, col);

            // Trail Particle
            ParticleEmitter e = particleEngine.AddParticleSystem(new Texture2D[] { projectileTexture, smokeTexture }, col, proj.Location.ToVector2(),
                -1, 100,
                11, 17,
                1, 4,
                rotation + MathHelper.Pi, -0.2F, 0.2F,
                0.1F,
                5, 20);
            proj.Tag = e;

            // Shooting Sound
            if (proj.Type == Projectile.ProjectileType.Player_Proj_Def)
            {
                SoundEffectInstance instance = soundEffects[1].CreateInstance();
                instance.Volume = 0.1F;
                instance.Play();
            }
        }

        private void Projectiles_ProjectileExploded(object sender)
        {
            Projectile proj = sender as Projectile;

            // Explosion Animation
            Point size = new Point(120, 120);
            Point loc = new Point(proj.Location.X - size.X / 2, proj.Location.Y - size.Y / 2);
            float rotation = VectorMath.VectorToAngle(proj.Speed);
            animationManager.AddAnimation(400, 12, 1, new Rectangle(loc, size), rotation, (int)Config.Graphics.AnimationType.Projectile_Explosion);

            // Shrapnel Particles
            particleEngine.AddParticleSystem(new Texture2D[] { smokeTexture }, Color.White, proj.Location.ToVector2(),
                1, 100,
                3, 20,
                5, 8,
                rotation + MathHelper.Pi, MathHelper.Pi / -2, MathHelper.Pi / 2,
                0.1F,
                7, 13);

            // Explosion Sound
            if (proj.Type == Projectile.ProjectileType.Player_Proj_Def)
            {
                SoundEffectInstance instance = soundEffects[0].CreateInstance();
                instance.Volume = 0.1F;
                instance.Play();
            }
        }

        private void Projectiles_ProjectileRemoved(object sender)
        {
            Projectile proj = sender as Projectile;
            ParticleEmitter e = proj.Tag as ParticleEmitter;
            if (e != null)
            {
                e.IsActive = false;
            }

        }

        private void Enemies_EnemyHurt(object sender, int damage, bool alive, bool selfDestruct)
        {
            Enemy enemy = sender as Enemy;
            if (selfDestruct)
            {
                // Explosion Animation
                Point size = new Point(200, 200);
                Point loc = new Point(enemy.Hitbox.Center.X - size.X / 2, enemy.Hitbox.Center.Y - size.Y / 2);
                float rotation = 0;
                animationManager.AddAnimation(400, 12, 1, new Rectangle(loc, size), rotation, (int)Config.Graphics.AnimationType.Projectile_Explosion);

                SoundEffectInstance instance = soundEffects[0].CreateInstance();
                instance.Volume = 0.1F;
                instance.Play();
            }
        }

        private void GameEngine_GameLost(object sender)
        {
            //soundEffects[2].CreateInstance().Play();
        }
    }
}
