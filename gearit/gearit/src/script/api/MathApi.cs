using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gearit.src.script.api.game
{
	class MathApi
	{
		public MathApi()
		{
		}

		public float PI { get { return (float)Math.PI; } }

		public Vector2 Mul(Vector2 a, float b)
		{
			return a * b;
		}

		public Vector2 Add(Vector2 a, Vector2 b)
		{
			return a + b;
		}

		public Vector2 Vector2(float x, float y)
		{
			return new Vector2(x, y);
		}
	}
}