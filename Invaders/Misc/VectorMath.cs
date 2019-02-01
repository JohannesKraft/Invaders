using System;
using Microsoft.Xna.Framework;

namespace Invaders.Misc
{
    public static class VectorMath
    {
        public static float AngleBetween(Vector2 vec1, Vector2 vec2)
        {
            Vector2 scalar = (vec1 * vec2);
            return (float)Math.Acos((scalar.X + scalar.Y) / (vec1.Length() * vec2.Length()));
        }

        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }

    }
}
