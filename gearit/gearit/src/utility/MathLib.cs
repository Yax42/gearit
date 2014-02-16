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
			if (from.LengthSquared() == 0 || to.LengthSquared() == 0)
				return 0;
			return (float)Math.Acos(MathHelper.Clamp(Vector2.Dot(Vector2.Normalize(from), Vector2.Normalize(to)), -1f, 1f));
		}

		static public float AngleFromUp(Vector2 v)
		{
			float res = Angle(Vector2.UnitY, v);
			if (v.X > 0)
				res = -res;
			return res;
		}

		static public float Angle(Vector2 v)
		{
			float res = Angle(Vector2.UnitX, v);
			if (v.Y < 0)
				res = -res;
			return res;
		}

		static public Vector2 RotateDelta(Vector2 origin, Vector2 A, float angle)
		{
			Vector2 relativA = A - origin;
			float newAngle = Angle(relativA) + angle;
			float length = relativA.Length();
			return PolarCoor(length, newAngle) + origin;
		}

		static public Vector2 PolarCoor(float length, float angle)
		{
			return new Vector2(length * (float)Math.Cos(angle), length * (float)Math.Sin(angle));
		}

		/// <summary>
		/// Converts an angle in decimal degress to radians.
		/// </summary>
		static public double DegreesToRadians(float angleInDegrees)
		{
			return angleInDegrees * ((float) Math.PI / 180);
		}

		static public double RadiansToDegrees(float angleInRadian)
		{
			return angleInRadian * (180 / (float) Math.PI);
		}

	}
}
