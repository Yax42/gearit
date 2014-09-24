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

		public override Vector2 Position
		{
			get
			{
				return __Chunk.Position;
			}

			set
			{
				__Chunk.Position = value;
			}
		}
	}
}
