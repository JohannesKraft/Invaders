using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Invaders.Misc;

namespace Invaders.Game
{
    public class Enemy : IEnemy
    {
        public EnemyType Type { get; set; }

        public Point Center { get => center; set => SetCenter(value); }
        public float Speed { get; set; }
        public int Direction { get; set; }

        public Rectangle Hitbox { get => hitbox; set { Center = value.Center; hitbox = value; } }

        public int HealthPoints { get; set; }
        public bool IsAlive { get; set; }
        public bool IsSpawned { get; set; }

        public Vector2[] ProjectileSpeed { get; set; }
        public int ProjectileDamage { get; set; }
        public int ProjectileExplosionRange { get; set; }
        public int ProjectileCooldown { get; set; }

        public int SelfDestructDamage { get; set; }
        public int SelfDestructExplosionRange { get; set; }

        public int SpawnCooldown { get; set; }

        public event EnemyEventHandler EnemyShoot;
        public event EnemyEventHandler EnemyMove;
        public event EnemyHurtEventHandler EnemyHurt;

        protected long cooldown;

        protected Rectangle hitbox;
        protected Point center;

        public Enemy()
        {
            ProjectileDamage = 100;
            ProjectileExplosionRange = 50;
            ProjectileCooldown = 120;
            ProjectileSpeed = new Vector2[]{ new Vector2(0, 15)};
            SelfDestructDamage = 1000;
            SelfDestructExplosionRange = 1000;
            SpawnCooldown = 100;
            IsAlive = true;
            IsSpawned = false;
            Direction = 1;
        }       

        public void Move(Point bounds, double ticks, float tickModifier)
        {
            if (ticks > SpawnCooldown)
            {
                double val = (ticks - SpawnCooldown) * tickModifier;

                int x = Convert.ToInt32(Math.Acos(Math.Cos(val * Direction * (MathHelper.TwoPi / 100) + 1.570796)) * (bounds.X - hitbox.Size.X) / Math.PI);
                int y = Convert.ToInt32(val);

                hitbox.Location = new Point(x, y);
                Center = hitbox.Center;
                IsSpawned = true;

                OnEnemyMove();
            }
        }

        public void Shoot(ProjectileList list, long ticks, Random random)
        {
            int rndVal = random.Next(0, 1000);
            if (rndVal <= 2 && ticks > SpawnCooldown && IsAlive)
            {
                if (ticks - (cooldown + ProjectileCooldown) > 0)
                {
                    Point loc = new Point(this.Center.X, this.hitbox.Bottom);
                    foreach (var speed in ProjectileSpeed)
                    {
                        list.Add(loc, speed, 100, ProjectileExplosionRange, Projectile.ProjectileType.Enemy_Proj_Def);
                    }

                    cooldown = ticks;

                    EnemyShoot?.Invoke(this);
                }
            }
        }

        public void Hurt(int damage, bool selfDestruct)
        {
            HealthPoints -= damage;
            if (HealthPoints <= 0)
            {
                IsAlive = false;
                HealthPoints = 0;
            }
            EnemyHurt?.Invoke(this, damage, IsAlive, selfDestruct);
        }

        public IEnemy Clone()
        {
            Enemy clone = new Enemy();

            clone.Type = this.Type;

            clone.Center = this.center;
            clone.Hitbox = this.hitbox;
            clone.HealthPoints = this.HealthPoints;
            clone.Speed = this.Speed;
            clone.Direction = this.Direction;

            clone.IsAlive = this.IsAlive;
            clone.IsSpawned = this.IsSpawned;

            clone.ProjectileSpeed = this.ProjectileSpeed;
            clone.ProjectileDamage = this.ProjectileDamage;
            clone.ProjectileExplosionRange = this.ProjectileExplosionRange;
            clone.ProjectileCooldown = this.ProjectileCooldown;

            clone.SelfDestructDamage = this.SelfDestructDamage;
            clone.SelfDestructExplosionRange = this.SelfDestructExplosionRange;

            clone.SpawnCooldown = this.SpawnCooldown;

            return clone;
        }

        protected void SetCenter(Point _center)
        {
            hitbox.Location = new Point(_center.X - hitbox.Width / 2, _center.Y - hitbox.Height / 2);
            center = _center;
        }

        protected virtual void OnEnemyMove()
        {
            EnemyMove?.Invoke(this);
        }
    }
}
