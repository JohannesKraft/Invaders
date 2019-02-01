using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Invaders.Misc;

namespace Invaders.Game
{
    public class EnemySpecial : Enemy
    {
        public EnemySpecial()
        {

        }

        public new void Move(Point bounds, double ticks, float tickModifier)
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

        public new IEnemy Clone()
        {
            EnemySpecial clone = new EnemySpecial();

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
    }
}
