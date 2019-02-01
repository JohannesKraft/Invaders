using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Invaders.Misc
{
    public static class ImageProcessing
    {
        public static Color TransparentColor = Color.LimeGreen;

        public static Bitmap DrawEllipse(Bitmap bmp, int x, int y, int width, int height, Color color)
        {
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    graphics.FillEllipse(brush, x - width / 2, y - height / 2, width, height);
                }
            }

            return bmp;
        }

        public static Bitmap DrawBitmap(Bitmap background, Bitmap foreground, int x, int y, int width, int height)
        {
            using (Graphics graphics = Graphics.FromImage(background))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.DrawImage(foreground, x - width / 2, y - height / 2, width, height);
            }

            return background;
        }

        public static void RecolorBitmap(ref Bitmap bmp, Color oldColor, Color newColor)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMap[] colorMap = new ColorMap[1];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = oldColor;
                colorMap[0].NewColor = newColor;
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                // Draw using the color map
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                g.DrawImage(bmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
            }
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap bitmap = new Bitmap(bmp, width, height);

            return bitmap;
        }

        public static Texture2D TextureFromBitmap(GraphicsDevice device, Bitmap bitmap)
        {
            bitmap = new Bitmap(bitmap);
            bitmap.MakeTransparent(TransparentColor);

            Texture2D tex = new Texture2D(device, bitmap.Width, bitmap.Height, false, SurfaceFormat.Color);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            int bufferSize = data.Height * data.Stride;
            byte[] bytes = new byte[bufferSize];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            tex.SetData(bytes);

            bitmap.UnlockBits(data);

            return tex;
        }
    }
}
