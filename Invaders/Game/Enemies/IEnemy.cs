using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Invaders.Game
{
    public delegate void EnemyEventHandler(object sender);
    public delegate void EnemyHurtEventHandler(object sender, int damage, bool alive, bool selfDestruct);

    public enum EnemyType
    {
        Default
    };

    public interface IEnemy
    {
        Point Center { get; set; }
        float Speed { get; set; }
        int Direction { get; set; }

        Rectangle Hitbox { get; set; }

        int HealthPoints { get; set; }
        bool IsAlive { get; set; }
        bool IsSpawned { get; set; }

        Vector2[] ProjectileSpeed { get; set; }
        int ProjectileDamage { get; set; }
        int ProjectileExplosionRange { get; set; }
        int ProjectileCooldown { get; set; }

        int SelfDestructDamage { get; set; }
        int SelfDestructExplosionRange { get; set; }

        int SpawnCooldown { get; set; }

        event EnemyEventHandler EnemyShoot;
        event EnemyEventHandler EnemyMove;
        event EnemyHurtEventHandler EnemyHurt;

        void Move(Point bounds, double ticks, float tickModifier);
        void Shoot(ProjectileList list, long ticks, Random random);
        void Hurt(int damage, bool selfDestruct);

        IEnemy Clone();
    }
}
