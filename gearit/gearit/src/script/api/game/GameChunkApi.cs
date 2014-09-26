using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using Microsoft.Xna.Framework;
using gearit.src.editor.map;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using System.Runtime.CompilerServices;

namespace gearit.src.script.api.game
{
	public class GameChunkApi : GameObjectApi
	{
		public MapChunk __Chunk;

		public GameChunkApi(MapChunk chunk)
		{
			__Chunk = chunk;
		}

		public bool IsTouching(GameChunkApi chunkapi)
		{
			for (ContactEdge c = __Chunk.ContactList; c != null;  c = c.Next)
			{
				if (c.Other == chunkapi.__Chunk)
					return (true);
			}

			return (false);
		}

		public bool Static
		{
			get { return __Chunk.IsStatic; }
			set { __Chunk.IsStatic = value; }
		}

		public float Mass
		{
			get { return __Chunk.Mass; }
            set { __Chunk.Mass = value; }
		}

		public bool IgnoreGravity
		{
			get { return __Chunk.IgnoreGravity; }
            set { __Chunk.IgnoreGravity = value; }
		}

		public float Gravity
		{
			get { return __Chunk.GravityScale; }
            set { __Chunk.GravityScale = value; }
		}

		public float Friction
		{
			get { return __Chunk.Friction; }
            set { __Chunk.Friction = value; }
		}

		public override Vector2 Position
		{
			get { return __Chunk.Position; }
            set { __Chunk.Position = value; }
		}
	}
}
