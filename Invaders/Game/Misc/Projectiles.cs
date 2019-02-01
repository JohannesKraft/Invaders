using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Invaders.Misc;

namespace Invaders.Game
{
    public class Projectile
    {
        public enum ProjectileType
        {
            Player_Proj_Def,
            Enemy_Proj_Def
        }

        public ProjectileType Type { get; set; }
        public Point Location { get; set; }
        public Vector2 Speed { get; set; }
        public int Damage { get; set; }
        public int ExplosionRadius { get; set; }
        public bool IsExploded { get => isExploded; set { if (value) OnProjectileExploded(); if (value) OnProjectileRemoved(); isExploded = value; } }
        public bool OutOfBounds { get => outOfBounds; set { if(value) OnProjectileRemoved(); outOfBounds = value; } }
        public object Tag { get; set; }

        public delegate void ProjectileEventHandler(object sender);
        public event ProjectileEventHandler ProjectileFired;
        public event ProjectileEventHandler ProjectileExploded;
        public event ProjectileEventHandler ProjectileRemoved;
        public event ProjectileEventHandler ProjectileMoved;

        private bool fired;
        private bool isExploded;
        private bool outOfBounds;

        public Projectile(Point location, Vector2 speed, int damage, int expRadius, ProjectileType type)
        {
            Location = location;
            Speed = speed;
            Damage = damage;
            ExplosionRadius = expRadius;
            Type = type;

            fired = false;
            isExploded = false;
            outOfBounds = false;
        }

        public void Move()
        {
            if (!fired)
            {
                fired = true;
                OnProjectileFired();
            }
            Location = new Point(Location.X + Convert.ToInt32(Speed.X), Location.Y + Convert.ToInt32(Speed.Y));

            OnProjectileMoved();
        }

        protected virtual void OnProjectileFired()
        {
            ProjectileFired?.Invoke(this);
        }

        protected virtual void OnProjectileMoved()
        {
            ProjectileMoved?.Invoke(this);
        }

        protected virtual void OnProjectileExploded()
        {
            ProjectileExploded?.Invoke(this);
        }

        protected virtual void OnProjectileRemoved()
        {
            ProjectileRemoved?.Invoke(this);
        }
    }

    public class ProjectileList
    {
        public Projectile[] Projectiles { get => projectiles.ToArray(); }

        public event Projectile.ProjectileEventHandler ProjectileFired;
        public event Projectile.ProjectileEventHandler ProjectileExploded;
        public event Projectile.ProjectileEventHandler ProjectileRemoved;
        public event Projectile.ProjectileEventHandler ProjectileMoved;

        private List<Projectile> projectiles;

        private Player[] players;
        private StructureList structures;
        private EnemyList enemies;
        private Rectangle bounds;

        public ProjectileList(Player[] _player, StructureList _structures, EnemyList _enemies, Rectangle _bounds)
        {
            projectiles = new List<Projectile>();
            players = _player;
            structures = _structures;
            enemies = _enemies;
            bounds = _bounds;
        }

        public Projectile Add(Point location, Vector2 speed, int damage, int expRadius, Projectile.ProjectileType type)
        {
            Projectile proj = new Projectile(location, speed, damage, expRadius, type);
            proj.ProjectileFired += ProjectileFired;
            proj.ProjectileMoved += ProjectileMoved;
            proj.ProjectileExploded += ProjectileExploded;
            proj.ProjectileRemoved += ProjectileRemoved;

            projectiles.Add(proj);
            return proj;
        }

        public void Remove(Projectile projectile)
        {
            projectile.OutOfBounds = true;
            projectiles.Remove(projectile);
        }

        public int IndexOf(Projectile projectile)
        {
            return projectiles.IndexOf(projectile);
        }

        public void Clear()
        {
            foreach (var proj in projectiles)
            {
                proj.OutOfBounds = true;
            }
            projectiles.Clear();
        }

        public void Move()
        {
            if (projectiles.Count != 0)
            {
                // Remove all exploded and out of bounds projectiles
                for (int i = 0; i < projectiles.Count; i++)
                {
                    Projectile proj = projectiles[i];
                    if (proj.IsExploded || proj.OutOfBounds)
                    {
                        projectiles.Remove(proj);
                        i--;
                    }
                }

                //Projectiles
                foreach (var proj in projectiles)
                {
                    bool outOfBounds = !bounds.Contains(proj.Location);

                    if (!proj.IsExploded && !outOfBounds)
                    {
                        proj.Move();

                        // Player
                        foreach (var p in players)
                        {
                            if (p.Hitbox.Contains(proj.Location) && proj.Type != Projectile.ProjectileType.Player_Proj_Def)
                            {
                                p.Hurt(proj.Damage);
                                proj.IsExploded = true;

                                Console.WriteLine(string.Format("Player hit: {0} hp left, Alive: {1}", p.HealthPoints, p.IsAlive));
                            }
                        }

                        //Structures
                        foreach (var str in structures.Structures)
                        {
                            if (str.Hitbox.Contains(proj.Location) && !str.Destroyed)
                            {
                                int x = proj.Location.X - str.Hitbox.X;
                                int y = proj.Location.Y - str.Hitbox.Y;
                                x = x * str.StructureBitmap.Width / str.Hitbox.Width;
                                y = y * str.StructureBitmap.Height / str.Hitbox.Height;

                                Vector2 speed = proj.Speed;
                                System.Drawing.Color col = str.StructureBitmap.GetPixel(x, y);

                                float i = 0.1F;
                                bool outOfBmp = false;
                                while (col.ToArgb() == ImageProcessing.TransparentColor.ToArgb() && !outOfBmp)
                                {
                                    int _x = Convert.ToInt32(x + speed.X * i);
                                    int _y = Convert.ToInt32(y + speed.Y * i);
                                    Rectangle rect = new Rectangle(0, 0, str.StructureBitmap.Width, str.StructureBitmap.Height);
                                    if (rect.Contains(_x, _y))
                                    {
                                        col = str.StructureBitmap.GetPixel(_x, _y);
                                        i += 0.1F;
                                    }
                                    else
                                    {
                                        outOfBmp = true;
                                    }


                                }

                                if (i < 1.0F && !outOfBmp && !proj.IsExploded)
                                {
                                    proj.IsExploded = true;
                                    str.StructureBitmap = ImageProcessing.DrawBitmap(
                                        str.StructureBitmap,
                                        str.ExplosionBitmap,
                                        Convert.ToInt32(x + speed.X * i),
                                        Convert.ToInt32(y + speed.Y * i),
                                        str.StructureBitmap.Width * proj.ExplosionRadius / str.Hitbox.Width,
                                        str.StructureBitmap.Height * proj.ExplosionRadius / str.Hitbox.Height);
                                    str.RecentlyChanged = true;

                                    Console.WriteLine(string.Format("Structure hit at {0}", proj.Location));
                                }


                            }
                        }

                        // Enemies
                        foreach (var e in enemies.Enemies)
                        {
                            if (e.Hitbox.Contains(proj.Location) && e.IsAlive && proj.Type != Projectile.ProjectileType.Enemy_Proj_Def && e.IsSpawned)
                            {
                                proj.IsExploded = true;
                                e.Hurt(proj.Damage, false);

                                Console.WriteLine(string.Format("Enemy hit: {0} hp left, Alive: {1}", e.HealthPoints, e.IsAlive));
                            }
                        }
                    }
                    else if (outOfBounds)
                    {
                        proj.OutOfBounds = outOfBounds;
                    }
                }

                
            }

            //Enemies AS projectiles
            if (enemies.Enemies.Length != 0)
            {
                foreach (var proj in enemies.Enemies)
                {
                    foreach (var p in players)
                    {
                        if (p.Hitbox.Contains(proj.Center))
                        {
                            p.Hurt(proj.SelfDestructDamage);
                            proj.Hurt(proj.HealthPoints, true);

                            Console.WriteLine(string.Format("Player colided with Enemy: {0} hp left, Alive: {1}", p.HealthPoints, p.IsAlive));
                        }
                    }

                    foreach (var str in structures.Structures)
                    {
                        if (str.Hitbox.Contains(proj.Center) && !str.Destroyed)
                        {
                            proj.Hurt(proj.HealthPoints, true);
                            str.Destroyed = true;

                            Console.WriteLine(string.Format("Structure destroyed at {0}", str.Center));
                        }
                    }
                }
            }
        }
    }
}
