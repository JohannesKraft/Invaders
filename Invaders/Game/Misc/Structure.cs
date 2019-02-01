using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Invaders.Game
{
    public class Structure
    {
        public Point Center { get; set; }

        public Rectangle Hitbox { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public System.Drawing.Bitmap StructureBitmap { get; set; }
        public System.Drawing.Bitmap ExplosionBitmap { get; set; }
        public bool RecentlyChanged { get; set; }
        public bool Destroyed { get; set; }

        public Structure()
        {
            RecentlyChanged = false;
            Destroyed = false;
        }
    }

    public class StructureList
    {
        public Structure[] Structures { get; set; }
        public int Amount { get; set; }
        public int Spacing { get; set; }
        public System.Drawing.Rectangle Area { get; set; }

        public System.Drawing.Bitmap StructureBitmap { get; set; }
        public Microsoft.Xna.Framework.Graphics.Texture2D StructureTexture { get; set; }
        public System.Drawing.Bitmap ExplosionBitmap { get; set; }

        public StructureList()
        {
            Amount = 4;
            Spacing = 50;
            Area = new System.Drawing.Rectangle(0, 0, 500, 500);
            StructureBitmap = new System.Drawing.Bitmap(10, 10);
            ExplosionBitmap = new System.Drawing.Bitmap(10, 10);
        }

        public void Create()
        {
            Structures = new Structure[Amount];

            int locationX = Spacing;
            int locationY = Area.Y;
            int width = (Area.Width - (Amount + 1) * Spacing) / Amount;
            int height = Area.Height;

            for (int i = 0; i < Structures.Length; i++)
            {
                Structure str = new Structure();

                str.Hitbox = new Rectangle(locationX, locationY, width, height);
                str.Center = str.Hitbox.Center;

                locationX += width + Spacing;

                str.StructureBitmap = new System.Drawing.Bitmap(StructureBitmap);
                str.ExplosionBitmap = new System.Drawing.Bitmap(ExplosionBitmap);
                str.RecentlyChanged = true;
                Structures[i] = str;
            }
        }
    }
}
