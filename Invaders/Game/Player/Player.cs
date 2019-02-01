using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Invaders.Game
{
    public class Player
    {
        public Point Center { get => center; set => SetCenter(value); }
        public Vector2 Speed { get; set; }
        public Vector2 CurrentSpeed { get; set; }

        public Rectangle Hitbox { get => hitbox;}
        public int Width { get => width; set => SetBounds(value, height); }
        public int Height { get => height; set => SetBounds(width, value);}

        public int HealthPoints { get => healthPoints; set { healthPoints = value; MaxHealthPoints = value;} }
        public int MaxHealthPoints { get; set; }
        public bool IsAlive { get; set; }

        public Vector2 ProjectileSpeed { get; set; }
        public int ProjectileDamage { get; set; }
        public int ProjectileExplosionRange{ get; set; }
        public int ShootingCooldown { get; set; }

        public object Tag { get; set; }

        public delegate void PlayerEvent(object sender);
        public delegate void PlayerHurtEvent(object sender, int damage, bool alive);
        public event PlayerEvent PlayerShoot;
        public event PlayerHurtEvent PlayerHurt;

        private Point center;
        private int width;
        private int height;
        private Rectangle hitbox;
        private int healthPoints;

        private long cooldown;

        public Player()
        {
            IsAlive = true;
        }

        public void Move(float direction, Rectangle bounds)
        {
            bool leftB = bounds.Contains(Hitbox.Left, Hitbox.Center.Y);
            bool rightB = bounds.Contains(Hitbox.Right, Hitbox.Center.Y);

            if (direction < 0.0F && !leftB)
            {
                direction = 0.0F;
            }
            else if (direction > 0.0F && !rightB)
            {
                direction = 0.0F;
            }

            Vector2 speed = Speed * direction;
            CurrentSpeed = speed;

            center.X += Convert.ToInt32(speed.X);
            center.Y += Convert.ToInt32(speed.Y);

            Point loc = Hitbox.Location;
            hitbox.Location = new Point(loc.X + Convert.ToInt32(speed.X), loc.Y + Convert.ToInt32(speed.Y));
        }

        public void Shoot(ProjectileList list, long ticks)
        {
            if (ticks - (cooldown + ShootingCooldown) > 0)
            {
                Point loc = new Point(this.Center.X, this.Hitbox.Top);
                list.Add(loc, ProjectileSpeed, ProjectileDamage, ProjectileExplosionRange, Projectile.ProjectileType.Player_Proj_Def);

                cooldown = ticks;

                PlayerShoot?.Invoke(this);
            }
        }

        public void Hurt(int damage)
        {
            healthPoints -= damage;
            if (healthPoints <= 0)
            {
                IsAlive = false;
                healthPoints = 0;
            }

            PlayerHurt?.Invoke(this, damage, IsAlive);
        }

        public void Revive()
        {
            IsAlive = true;
            healthPoints = MaxHealthPoints;
        }

        private void SetCenter(Point _center)
        {
            Point hitloc = new Point(_center.X - Width / 2, _center.Y - Height / 2);
            hitbox = new Rectangle(hitloc, new Point(Width, Height));
            center = _center;
        }

        private void SetBounds(int _width, int _height)
        {
            Point hitloc = new Point(center.X - _width / 2, center.Y - _height / 2);
            hitbox = new Rectangle(hitloc, new Point(_width, _height));

            width = _width;
            height = _height;
        }
    }
}
