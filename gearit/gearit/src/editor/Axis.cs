using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gearit.src.editor
{
	class MirrorAxis
	{
		static public bool Active = false;
		static public Vector2 Origin { get; set; }
		static private Vector2 _Dir = new Vector2(1, 0);
		static public Vector2 Dir
		{
			get { return _Dir; }
			internal set
			{
				if (value.LengthSquared() != 0)
					_Dir = Vector2.Normalize(value);
			}
		}

		static public Vector2 Transform(Vector2 p)
		{
			return Vector2.Reflect(p - Origin, new Vector2(_Dir.Y, -_Dir.X)) + Origin;
		}
	}

	class LockAxis
	{
		static public bool Active = false;
		static public Vector2 Origin { get; set; }
		static private Vector2 _Dir = new Vector2(1, 0);
		static public Vector2 Dir
		{
			get { return _Dir; }
			internal set
			{
				if (value.LengthSquared() != 0)
					_Dir = Vector2.Normalize(value);
			}
		}

		static public Vector2 Transform(Vector2 p)
		{
			if (Vector2.DistanceSquared(Origin, p) < 0.001f)
				return Origin;
			return Origin + Dir * Vector2.Dot(Dir, p - Origin);
		}
	}
}
