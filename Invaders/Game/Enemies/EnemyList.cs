using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Invaders.Game
{
    public class EnemyList
    {
        public IEnemy[] Enemies { get => enemies.ToArray(); }
        public float MinSpeed { get => minSpeed; set { tickModifier = value / 120; minSpeed = value; } }
        public float MaxSpeed { get => maxSpeed; set { maxSpeed = value; } }
        public int Amount { get => amount; }
        public Point Bounds { get; set; }

        public event EnemyEventHandler EnemyShoot;
        public event EnemyEventHandler EnemyMove;
        public event EnemyHurtEventHandler EnemyHurt;

        private List<IEnemy> enemies;
        private int amount;
        private float tickModifier;
        private float speedModifier;
        private double ticks = 0;
        private Random random;
        private List<object> sampleEnemies;

        private float minSpeed;
        private float maxSpeed;

        public EnemyList()
        {
            amount = 0;
            random = new Random();
            enemies = new List<IEnemy>();
            sampleEnemies = new List<object>();
        }

        public void Move(ProjectileList list)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                IEnemy e = enemies[i];
                if (!e.IsAlive)
                {
                    enemies.Remove(e);
                    i--;
                }
            }

            float val = MaxSpeed - MinSpeed;
            float current = MinSpeed + val * (1 - (float)enemies.Count / Amount);
            speedModifier = current / MinSpeed;
            ticks += 1 * speedModifier;

            foreach (var e in Enemies)
            {
                e.Move(Bounds, ticks, tickModifier);
                e.Speed = current;
                e.Shoot(list, Convert.ToInt64(ticks), random);
            }
        }

        public void AddEnemies(IEnemy sample, int amount, int spawnCooldown, bool memorize)
        {
            if (memorize)
            {
                sampleEnemies.Add(new object[] { sample, amount, spawnCooldown });
            }

            for (int i = 0; i < amount; i++)
            {
                IEnemy e = sample.Clone();
                e.SpawnCooldown = spawnCooldown * i;
                e.Direction = 1;

                e.EnemyHurt += EnemyHurt;
                e.EnemyMove += EnemyMove;
                e.EnemyShoot += EnemyShoot;

                if (i % 2 != 0)
                {
                    e.Direction = -1;
                }
                enemies.Add(e);
            }

            this.amount += amount;
        }

        public void RemoveEnemies()
        {
            enemies = new List<IEnemy>();
            sampleEnemies = new List<object>();
        }

        public void RestoreEnemies()
        {
            enemies.Clear();
            ticks = 0;
            amount = 0;
            foreach (object[] obj in sampleEnemies)
            {
                AddEnemies((IEnemy)obj[0], (int)obj[1], (int)obj[2], false);
            }
        }
    }
}