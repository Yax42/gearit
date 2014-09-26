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
using gearit.src.Network;

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

		private void PushEvent(InGamePacketManager.EChunkCommand cmd, bool data)
		{
			if (GameLuaScript.IsServer)
				GameLuaScript.PacketManager.ChunkCommand(cmd, GameLuaScript.Instance.ServerGame.Map.Chunks.IndexOf(__Chunk) , data);
		}

		private void PushEvent(InGamePacketManager.EChunkCommand cmd, float data)
		{
			if (GameLuaScript.IsServer)
				GameLuaScript.PacketManager.ChunkCommand(cmd, GameLuaScript.Instance.ServerGame.Map.Chunks.IndexOf(__Chunk) , data);
		}

		public bool Static
		{
			get { return __Chunk.IsStatic; }
			set
			{
				PushEvent(InGamePacketManager.EChunkCommand.Static, value);
				__Chunk.IsStatic = value;
			}
		}

		public float Mass
		{
			get { return __Chunk.Mass; }
            set
			{
				PushEvent(InGamePacketManager.EChunkCommand.Mass, value);
				__Chunk.Mass = value;
			}
		}

		public bool IgnoreGravity
		{
			get { return __Chunk.IgnoreGravity; }
            set
			{
				PushEvent(InGamePacketManager.EChunkCommand.IgnoreGravity, value);
				__Chunk.IgnoreGravity = value;
			}
		}

		public float Gravity
		{
			get { return __Chunk.GravityScale; }
            set
			{
				PushEvent(InGamePacketManager.EChunkCommand.Gravity, value);
				__Chunk.GravityScale = value;
			}
		}

		public float Friction
		{
			get { return __Chunk.Friction; }
            set
			{
				PushEvent(InGamePacketManager.EChunkCommand.Friction, value);
				 __Chunk.Friction = value;
			}
		}

		public override Vector2 Position
		{
			get { return __Chunk.Position; }
            set { __Chunk.Position = value; }
		}

		public void Reset()
		{
			__Chunk.Rotation = 0;
			__Chunk.LinearVelocity = Vector2.Zero;
			__Chunk.AngularVelocity = 0;
		}
	}
}
