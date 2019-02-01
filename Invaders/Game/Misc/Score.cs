using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invaders.Game
{
    public class Score
    {
        public float Value { get; set; } // Perfect Score: 2233,33

        private Player player;
        private EnemyList enemies;

        public Score(Player player, EnemyList enemies)
        {
            Value = 0;
            this.player = player;
            this.enemies = enemies; 

            player.PlayerShoot += Player_PlayerShoot;
            player.PlayerHurt += Player_PlayerHurt;
            enemies.EnemyHurt += Enemy_EnemyHurt;
        }

        private void Player_PlayerHurt(object sender, int damage, bool alive)
        {
            if (!alive)
            {
                Value -= 500;
            }
        }

        private void Enemy_EnemyHurt(object sender, int damage, bool alive, bool selfDestruct)
        {
            Enemy enemy = sender as Enemy;
            if (!alive && !selfDestruct)
            {
                Value += damage * ((float)enemy.Speed / enemies.MaxSpeed) * ((float)player.HealthPoints / player.MaxHealthPoints);
            }

            if (!alive && selfDestruct)
            {
                Value -= 250;
            }
        }

        private void Player_PlayerShoot(object sender)
        {
            Value -= 10;
        }
    }
}
