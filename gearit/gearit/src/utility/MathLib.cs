using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gearit.src.utility
{
    class MathLib
    {
        static float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Acos(MathHelper.Clamp(Vector2.Dot(Vector2.Normalize(from), Vector2.Normalize(to)), -1f, 1f)) * 57.29578f;
        }
        static public float AngleFromUp(Vector2 v)
        {
            float res = Angle(Vector2.UnitY, v);
            if (v.X > 0)
                res = -res;
            return res;
        }
    }
}
