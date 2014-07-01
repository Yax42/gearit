using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace gearit.src.script.api.game
{
	abstract class GameObjectApi
	{
		abstract public Vector2 Position { get; set; }

		public float Distance(GameObjectApi other)
		{
			return (Position - other.Position).Length();
		}

		public void MoveTo(GameObjectApi other)
		{
			Position = other.Position;
		}
	}
}
