using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invaders.GameGraphics
{
    class ModelControl
    {
        public Model Model;
        public Vector3 Location;
        public Matrix WorldMatrix;

        public ModelControl(Model model, Vector3 location)
        {
            Model = model;
            Location = location;
            Move(Vector3.Zero, 0);
        }

        public void Move(Vector3 speed, float angleY)
        {
            Location += speed;
            WorldMatrix = Matrix.CreateRotationY(angleY) * Matrix.CreateTranslation(Location);
        }
    }
}
