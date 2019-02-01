using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Invaders.Game;
using Invaders.GameGraphics;
using Invaders.Misc;

namespace Invaders
{
    public partial class GameDisplay
    {
        Texture2D enemyTexture;
        Texture2D[] structureTextures;
        Texture2D projectileTexture;
        Texture2D smokeTexture;
        Texture2D whiteTexture;

        Texture2D backgroundTexture;
        Texture2D[] endScreenTexture;

        Texture2D explAnimation;

        Texture2D playerAnimation;

        Song backgroundSong;
        List<SoundEffect> soundEffects = new List<SoundEffect>();

        private void LoadTextures()
        {
            // Textures
            structureTextures = new Texture2D[gameEngine.Structures.Structures.Length];
            for (int i = 0; i < structureTextures.Length; i++)
            {
                structureTextures[i] = this.Content.Load<Texture2D>(@"misc\structure");
            }
            enemyTexture = this.Content.Load<Texture2D>(@"misc\enemy");
            projectileTexture = this.Content.Load<Texture2D>(@"misc\projectile");
            smokeTexture = this.Content.Load<Texture2D>(@"misc\smoke");
            whiteTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            whiteTexture.SetData<Color>(new Color[1] { Color.White });


            // fullscreen
            backgroundTexture = this.Content.Load<Texture2D>(@"fullscreen\background");
            endScreenTexture = new Texture2D[2];
            endScreenTexture[0] = this.Content.Load<Texture2D>(@"fullscreen\winscreen");
            endScreenTexture[1] = this.Content.Load<Texture2D>(@"fullscreen\losescreen");

            // animations
            explAnimation = this.Content.Load<Texture2D>(@"animations\ta_explosion");
            playerAnimation = this.Content.Load<Texture2D>(@"animations\ta_player");

            animationManager.AddSprite(explAnimation, 1, 12, (int)Config.Graphics.AnimationType.Projectile_Explosion);
            animationManager.AddSprite(smokeTexture, 10, 1, (int)Config.Graphics.AnimationType.Projectile_Trail);
            animationManager.AddSprite(playerAnimation, 2, 5, (int)Config.Graphics.AnimationType.Player_Movement);

            // fonts
            Config.Graphics.Fonts.DefaultFont = this.Content.Load<SpriteFont>(@"fonts\defaultFont");
            Config.Graphics.Fonts.MenuFont_Text = this.Content.Load<SpriteFont>(@"fonts\MenuFont_Text");
            Config.Graphics.Fonts.MenuFont_Title = this.Content.Load<SpriteFont>(@"fonts\MenuFont_Title");
            // sounds & songs
            backgroundSong = this.Content.Load<Song>(@"music\hold_the_line");
            soundEffects.Add(this.Content.Load<SoundEffect>(@"sounds\s_explode"));
            soundEffects.Add(this.Content.Load<SoundEffect>(@"sounds\s_shoot"));

            //Interface
            interfaceManager.ButtonTexture = this.Content.Load<Texture2D>(@"gui/button");
            interfaceManager.BackgroundTexture = this.Content.Load<Texture2D>(@"fullscreen\menu_background");

        }

        private void LoadEngineData()
        {
            // Player
            gameEngine.Player.Width = 100;
            gameEngine.Player.Height = 100;
            gameEngine.Player.Speed = new Vector2(10, 0);
            gameEngine.Player.Center = new Point(800, 840);
            gameEngine.Player.ProjectileDamage = 100;
            gameEngine.Player.ProjectileSpeed = new Vector2(0, -22.5F);
            gameEngine.Player.ProjectileExplosionRange = 50;
            gameEngine.Player.HealthPoints = 500;
            gameEngine.Player.ShootingCooldown = 60;

            // Enemies
            gameEngine.Enemies.MinSpeed = 20;
            gameEngine.Enemies.MaxSpeed = 60;
            gameEngine.Enemies.EnemyHurt += Enemies_EnemyHurt;

            Enemy enemy = new Enemy()
            {
                Hitbox = new Rectangle(new Point(800, 100), new Point(60, 60)),
                HealthPoints = 100,
                Speed = gameEngine.Enemies.MinSpeed,

                IsAlive = true,
                IsSpawned = false,

                ProjectileSpeed = new Vector2[] { new Vector2(0, 15) },
                ProjectileDamage = 100,
                ProjectileExplosionRange = 50,

                SelfDestructDamage = 1000,
                SelfDestructExplosionRange = 1000,

                SpawnCooldown = 100
            };
            gameEngine.Enemies.AddEnemies(enemy, 40, 120, true);

            Enemy enemySpecial = new Enemy()
            {
                Hitbox = new Rectangle(new Point(800, 100), new Point(100, 100)),
                HealthPoints = 100,
                Speed = gameEngine.Enemies.MinSpeed,

                IsAlive = true,
                IsSpawned = false,

                ProjectileSpeed = new Vector2[] { new Vector2(0, 15), new Vector2(7.5F, 7.5F), new Vector2(-7.5F, 7.5F) },
                ProjectileDamage = 100,
                ProjectileExplosionRange = 50,

                SelfDestructDamage = 1000,
                SelfDestructExplosionRange = 1000,

                SpawnCooldown = 100
            };
            gameEngine.Enemies.AddEnemies(enemy, 40, 120, true);

            // Structures
            gameEngine.Structures.Amount = 4;
            gameEngine.Structures.Spacing = 180;
            gameEngine.Structures.Area = new System.Drawing.Rectangle(0, 620, 1600, 130);

            gameEngine.Structures.StructureBitmap = new System.Drawing.Bitmap(@"Content\misc\structure.png");
            gameEngine.Structures.StructureTexture = ImageProcessing.TextureFromBitmap(this.GraphicsDevice, gameEngine.Structures.StructureBitmap);

            System.Drawing.Bitmap explBmp = new System.Drawing.Bitmap(@"Content\misc\explosion.png");
            ImageProcessing.RecolorBitmap(ref explBmp, System.Drawing.Color.Black, ImageProcessing.TransparentColor);
            gameEngine.Structures.ExplosionBitmap = explBmp;

            // Events
            gameEngine.Projectiles.ProjectileExploded += Projectiles_ProjectileExploded;
            gameEngine.Projectiles.ProjectileRemoved += Projectiles_ProjectileRemoved;
            gameEngine.Projectiles.ProjectileFired += Projectiles_ProjectileFired;
            gameEngine.Projectiles.ProjectileMoved += Projectiles_ProjectileMoved;

            gameEngine.GameLost += GameEngine_GameLost;
            gameEngine.StartEngine();
        }
    }
}
