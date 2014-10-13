using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using gearit.src.game;
using System.Diagnostics;
using gearit.src.robot;
using Lidgren.Network;
using gearit.src.editor.map;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;

namespace gearit.src.Network
{
	class InGamePacketManager
	{
		#region enum
		public enum GameCommand
		{
			End,
			Pause,
			UnPause,
		};
		public enum ERobotCommand
		{
			Remove,
			Win,
			Lose,
			Teleport,
		};
		public enum EChunkCommand
		{
			Static,
			Mass,
			IgnoreGravity,
			Gravity,
			Friction,
		};

		public enum CommandId
		{
			GameCommand,
			RobotCommand,
			ObjectTransform,
			MotorForce,
			Message,
			EndOfPacket,
			BeginTransform,
			ChunkCommand,
		};
		#endregion

//----------------------------------------------------------------------------

		#region Packets
		private struct Packet_GameCommand // size 1
		{
			public byte CommandId;
		}

		private struct Packet_ChunkCommand
		{
			public byte ChunkId;
			public byte Command;
			public bool BoolData;
			public float FloatData;
		}

		private struct Packet_RobotCommand // size 2
		{
			public byte RobotId;
			public byte Command;
			public Vector2 Position;
		}

		enum TransformType
		{
			Robot,
			Piece,
			Chunk,
		};

		private struct Packet_ObjectTransform // size 28
		{
			public byte Type;
			public byte RobotId;
			public ushort Id;
			public Sweep Sweep;
		}


		public enum MotorType
		{
			Frozen,
			Force,
			AddLimit,
		};

		private struct Packet_Motor // size 8
		{
			public byte RobotId;
			public ushort MotorId;
			public byte Type;
			public float Force;
			public bool Frozen;
			public short NbCycle;
		}

		private struct Packet_Message
		{
			public string msg;
			public int duration;
		}
		#endregion

//----------------------------------------------------------------------------

		private T RawDataToPacket<T>() where T : struct
		{
			Idx++;
			unsafe
			{
				fixed (byte* p = &Data[Idx])
				{
					Idx += Marshal.SizeOf(typeof(T));
					return (T)Marshal.PtrToStructure(new IntPtr(p), typeof(T));
				}
			}
		}

		//http://stackoverflow.com/questions/3278827/how-to-convert-a-structure-to-a-byte-array-in-c

		private byte[] PacketToRawData<T>(T packet, CommandId id) where T : struct
		{
			int size = Marshal.SizeOf(packet);
			byte[] arr = new byte[size + 1];
			arr[0] = (byte) id;
			IntPtr ptr = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(packet, ptr, true);
			Marshal.Copy(ptr, arr, 1, size);
			Marshal.FreeHGlobal(ptr);

			return arr;
		}

#region Attributes

		private IGearitGame Game;
		private int Idx;
		public byte[] Data;
#endregion

		public InGamePacketManager(IGearitGame game)
		{
			Game = game;
		}

//----------------------------------------------------------------------------

#region GetRawData
		public byte[] RobotCommand(int id, ERobotCommand command)
		{
			var packet = new Packet_RobotCommand();
			packet.RobotId = (byte) id;
			packet.Command = (byte) command;
			return PacketToRawData(packet, CommandId.RobotCommand);
		}

		public byte[] TeleportRobot(int id, Vector2 pos)
		{
			var packet = new Packet_RobotCommand();
			packet.RobotId = (byte) id;
			packet.Command = (byte)ERobotCommand.Teleport;
			packet.Position = pos;
			return PacketToRawData(packet, CommandId.RobotCommand);
		}

#if false
		private struct Packet_SpotTransform
		{
			public byte RobotId;
			public ushort SpotId;
			public float Angle;
		}
		private void ApplyPacket(Packet_SpotTransform packet)
		{
			Debug.Assert(Game.Robots.Count > packet.RobotId);
			if (Game.Robots.Count <= packet.RobotId)
				return;
			Robot r = Game.Robots[packet.RobotId];
			Debug.Assert(!r.Extracted);
			if (r.Extracted)
				return;
			Debug.Assert(r.Spots.Count > packet.SpotId);
			if (r.Spots.Count <= packet.SpotId)
				return;
			r.Spots[packet.SpotId].JointAngle = packet.Angle;
		}
		public byte[] SpotTransform(int idRobot, int idSpot)
		{
			Packet_SpotTransform res = new Packet_SpotTransform();
			res.RobotId = (byte) idRobot;
			res.SpotId = (ushort) idSpot;
			res.Angle = Game.Robots[idRobot].Spots[idSpot].JointAngle;
			return PacketToRawData(res, CommandId.ObjectTransform);
		}
#endif


		public byte[] RobotTransform(int idRobot)
		{
			return RobotTransform(Game.Robots[idRobot]);
		}

		public byte[] CreatePacket(CommandId cmd)
		{
			byte[] res = new byte[1];
			res[0] = (byte) cmd;
			return res;
		}

		public byte[] ChunkCommand(EChunkCommand cmd, int id, bool data)
		{
			var packet = new Packet_ChunkCommand();
			packet.ChunkId = (byte) id;
			packet.Command = (byte) cmd;
			packet.BoolData = data;
			return PacketToRawData(packet, CommandId.ChunkCommand);
		}

		public byte[] ChunkCommand(EChunkCommand cmd, int id, float data)
		{
			var packet = new Packet_ChunkCommand();
			packet.ChunkId = (byte) id;
			packet.Command = (byte) cmd;
			packet.FloatData = data;
			return PacketToRawData(packet, CommandId.ChunkCommand);
		}

		public byte[] RobotPieceTransform(Robot r, int id)
		{
			return TransformBody(r.Pieces[id], TransformType.Piece, id, r.Id);
		}

		public byte[] RobotTransform(Robot r)
		{
			return TransformBody(r.Heart, TransformType.Robot, 0, r.Id);
		}

		public byte[] ChunkTransform(int chunkId)
		{
			return TransformBody(Game.Map.Chunks[chunkId], TransformType.Chunk, chunkId, 0);
		}

		private byte[] TransformBody(Body b, TransformType type, int id, int robotId)
		{
			Packet_ObjectTransform res = new Packet_ObjectTransform();
			res.Type = (byte)type;
			res.RobotId = (byte) robotId;
			res.Id = (ushort) id;
			res.Sweep = b.Sweep;
			return PacketToRawData(res, CommandId.ObjectTransform);
		}

		public byte[] MotorForce(int motorId)
		{
			Robot r = Game.Robots[Game.MainRobotId];
			Debug.Assert(motorId < r.Spots.Count);
			Packet_Motor res = new Packet_Motor();
			res.Type = (byte)MotorType.Force;
			res.MotorId = (ushort)motorId;
			res.Force = r.Spots[motorId].Force;
			res.RobotId = (byte)Game.MainRobotId;
			return PacketToRawData(res, CommandId.MotorForce);
		}

		public byte[] Motor(RevoluteSpot spot, float force)
		{
			return Motor(spot, MotorType.Force, false, 0, force);
		}

		public byte[] Motor(RevoluteSpot spot, int cycle)
		{
			return Motor(spot, MotorType.AddLimit, false, cycle, 0);
		}

		public byte[] Motor(RevoluteSpot spot, bool frozen)
		{
			return Motor(spot, MotorType.Frozen, frozen, 0, 0);
		}

		public byte[] Motor(RevoluteSpot spot, MotorType type, bool frozen, int cycle, float force)
		{
			Packet_Motor res = new Packet_Motor();
			res.MotorId = (ushort) spot.Id;
			res.Frozen = frozen;
			res.Force = force;
			res.Type = (byte)type;
			res.NbCycle = (short)cycle;
			res.RobotId = (byte)Game.MainRobotId;
			return PacketToRawData(res, CommandId.MotorForce);
		}

#endregion

//----------------------------------------------------------------------------

#region ApplyPacket

		public void ApplyRequest(NetIncomingMessage request, bool proceedTransform)
		{
			Idx = 4;
			Data = request.Data;
			while (ApplyNextPacket(proceedTransform)) ;
		}

		public bool ApplyNextPacket(bool proceedTransform = false)
		{
			if (Data == null)
				return false;
			if (Idx >= Data.Count())
			{
				Debug.Assert(false);
				return false;
			}
			switch (Data[Idx])
			{
				case (byte)CommandId.GameCommand:
					ApplyPacket(RawDataToPacket<Packet_GameCommand>());
					break;
				case (byte)CommandId.MotorForce:
					ApplyPacket(RawDataToPacket<Packet_Motor>());
					break;
				case (byte)CommandId.RobotCommand:
					ApplyPacket(RawDataToPacket<Packet_RobotCommand>());
					break;
				case (byte)CommandId.ObjectTransform:
					ApplyPacket(RawDataToPacket<Packet_ObjectTransform>());
					break;
				case (byte)CommandId.ChunkCommand:
					ApplyPacket(RawDataToPacket<Packet_ChunkCommand>());
					break;
				case (byte)CommandId.BeginTransform:
					Idx++;
					return proceedTransform;
					break;
				case (byte)CommandId.EndOfPacket:
					return false;
				case (byte)CommandId.Message:
					ApplyPacket(RawDataToPacket<Packet_Message>());
					break;
				default:
					return false;
					break;
			}
			//Debug.Assert(Idx == Data.Count());
			return Idx != Data.Count();
		}

		private void ApplyPacket(Packet_ChunkCommand packet)
		{
			if (Game.Map.Chunks.Count <= packet.ChunkId)
			{
				Debug.Assert(false);
				return;
			}
			switch (packet.Command)
			{
				case (byte)EChunkCommand.Friction:
					// Not doing anything cause we do not modify the friction for now
					break;
				case (byte)EChunkCommand.Gravity:
					Game.Map.Chunks[packet.ChunkId].GravityScale = packet.FloatData;
					break;
				case (byte)EChunkCommand.IgnoreGravity:
					Game.Map.Chunks[packet.ChunkId].IgnoreGravity = packet.BoolData;
					break;
				case (byte)EChunkCommand.Mass:
					Game.Map.Chunks[packet.ChunkId].Mass = packet.FloatData;
					break;
				case (byte)EChunkCommand.Static:
					Game.Map.Chunks[packet.ChunkId].IsStatic = packet.BoolData;
					break;
			}
		}

		private void ApplyPacket(Packet_Message packet)
		{
			Game.Message(packet.msg, packet.duration);
		}

		private void ApplyPacket(Packet_GameCommand packet)
		{
			switch (packet.CommandId)
			{
				case (byte) GameCommand.End:
					Game.Finish();
					break;
				case (byte) GameCommand.Pause:
					break;
				case (byte) GameCommand.UnPause:
					break;
			}
		}

		private void ApplyPacket(Packet_RobotCommand packet)
		{
			Debug.Assert(Game.Robots.Count > packet.RobotId);
			if (Game.Robots.Count <= packet.RobotId)
				return;
			Robot r = Game.Robots[packet.RobotId];
			Debug.Assert(!r.Extracted);
			if (r.Extracted)
				return;
			switch (packet.Command)
			{
				case (byte) ERobotCommand.Lose:
					break;
				case (byte) ERobotCommand.Win:
					Game.Finish();
					break;
				case (byte) ERobotCommand.Remove:
					r.ExtractFromWorld();
					break;
				case (byte) ERobotCommand.Teleport:
					Vector2 deltaPos = packet.Position - r.Position;
					foreach (Piece p in r.Pieces)
						p.Position += deltaPos;
					break;
			}
		}

		private void ApplyPacket(Packet_ObjectTransform packet)
		{
			Body b = null;
			if (packet.Type == (byte) TransformType.Robot)
			{
				if (Game.Robots.Count > packet.RobotId)
				{
					Robot r = Game.Robots[packet.RobotId];
					Debug.Assert(!r.Extracted);
					if (!r.Extracted)
					{
						b = r.Heart;
					}
				}
			}
			else if (packet.Type == (byte) TransformType.Chunk)
			{
				if (Game.Map.Chunks.Count > packet.Id)
				{
					b = Game.Map.Chunks[packet.Id];
				}
			}
			else if (packet.Type == (byte) TransformType.Piece)
			{
				if (Game.Robots.Count > packet.RobotId)
				{
					Robot r = Game.Robots[packet.RobotId];
					Debug.Assert(!r.Extracted);
					if (!r.Extracted && r.Pieces.Count > packet.Id)
					{
						b = r.Pieces[packet.Id];
					}
				}
			}

			Debug.Assert(b != null);
			if (b != null)
			{
				b.Sweep = packet.Sweep;
			}
		}


		private void ApplyPacket(Packet_Motor packet)
		{
			Debug.Assert(Game.Robots.Count > packet.RobotId);
			if (Game.Robots.Count > packet.RobotId)
			{
				Robot r = Game.Robots[packet.RobotId];
				Debug.Assert(r.Spots.Count > packet.MotorId);
				Debug.Assert(!r.Extracted);
				if (!r.Extracted && r.Spots.Count > packet.MotorId)
				{
					switch (packet.Type)
					{
						case (byte) MotorType.Force:
							r.Spots[packet.MotorId].Force = packet.Force;
							break;
						case (byte) MotorType.Frozen:
							r.Spots[packet.MotorId].Frozen = packet.Frozen;
							break;
						case (byte) MotorType.AddLimit:
							r.Spots[packet.MotorId].AddLimitsCycle(packet.NbCycle);
							break;
					}
				}
			}
		}

#endregion
	}
}
