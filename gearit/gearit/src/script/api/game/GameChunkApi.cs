using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using Microsoft.Xna.Framework;
using gearit.src.editor.map;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace gearit.src.script.api.game
{
	class GameChunkApi : GameObjectApi
	{
		private MapChunk _Chunk;

		public GameChunkApi(MapChunk chunk)
		{
			_Chunk = chunk;
		}

		public bool isTouching(GameChunkApi chunkapi)
		{
			for (ContactEdge c = _Chunk.ContactList; c != null;  c = c.Next)
			{
				if (c.Other == chunkapi._Chunk)
					return (true);
			}

			return (false);
		}

		public override Vector2 Position
		{
			get
			{
				return _Chunk.Position;
			}

			set
			{
				_Chunk.Position = value;
			}
		}
	}
}
