using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace gearit.src.script.api.game
{
    /// <summary>
    /// Lua API for abstract object in World
    /// </summary>
	public abstract class GameObjectApi
	{
		abstract public Vector2 Position { get; set; }

		abstract public Vector2 Speed { get; set; }

		public float Distance(GameObjectApi other)
		{
			return (Position - other.Position).Length();
		}

		public void MoveTo(GameObjectApi other)
		{
			Position = other.Position;
		}

		public Vector2 Direction(GameObjectApi other, float mul = 1)
		{
			Vector2 res = other.Position - Position;
			if (res != Vector2.Zero)
				res.Normalize();
			return res * mul;
		}
	}
}
