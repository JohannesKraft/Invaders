using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invaders.GameGraphics
{
    class Camera
    {
        public Vector3 Location;
        public Vector3 Target;
        public Matrix ProjectionMatrix;
        public Matrix ViewMatrix;
        public Matrix WorldMatrix;

        public Camera(Vector3 location, Vector3 target, float aspectRatio)
        {
            Update(location, target, aspectRatio);
        }

        public void Update(Vector3 location, Vector3 target, float aspectRatio)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               aspectRatio,
                               0.01f, 1000f);

            ViewMatrix = Matrix.CreateLookAt(location, target,
                         Vector3.Up);

            WorldMatrix = Matrix.CreateWorld(Target, Vector3.
                          Forward, Vector3.Up);
        }
    }
}
