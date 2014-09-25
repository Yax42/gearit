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
		};
		#endregion

		#region Packets
		private struct Packet_GameCommand // size 1
		{
			public byte CommandId;
		}

		private struct Packet_RobotCommand // size 2
		{
			public byte RobotId;
			public byte Command;
		}

		private struct Packet_ObjectTransform // size 28
		{
			public bool IsRobot;
			public ushort Id;
			public Vector2 Position;
			public float Rotation;
			public Vector2 LinearVelocity;
			public float AngularVelocity;
		}

		private struct Packet_MotorForce // size 8
		{
			public byte RobotId;
			public ushort MotorId;
			public float Force;
		}

		private struct Packet_Message
		{
			public string msg;
			public int duration;
		}
		#endregion

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

#region GetRawData
		public byte[] RobotCommand(int id, ERobotCommand command)
		{
			var packet = new Packet_RobotCommand();
			packet.RobotId = (byte) id;
			packet.Command = (byte) command;
			return PacketToRawData(packet, CommandId.RobotCommand);
		}

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

		public byte[] RobotTransform(Robot r)
		{
			return TransformBody(r.Heart, true, r.Id);
		}

		public byte[] ChunkTransform(int chunkId)
		{
			return TransformBody(Game.Map.Chunks[chunkId], false, chunkId);
		}

		private byte[] TransformBody(Body b, bool isRobot, int id)
		{
			Packet_ObjectTransform res = new Packet_ObjectTransform();
			res.IsRobot = isRobot;
			res.Id = (ushort) id;
			res.Position = b.Position;
			res.Rotation = b.Rotation;
			res.LinearVelocity = b.LinearVelocity;
			res.AngularVelocity = b.AngularVelocity;
			return PacketToRawData(res, CommandId.ObjectTransform);
		}


		public byte[] MotorForce(int motorId)
		{
			Robot r = Game.Robots[Game.MainRobotId];
			Debug.Assert(motorId < r.Spots.Count);
			Packet_MotorForce res = new Packet_MotorForce();
			res.MotorId = (ushort)motorId;
			res.Force = r.Spots[motorId].Force;
			res.RobotId = (byte)Game.MainRobotId;
			return PacketToRawData(res, CommandId.MotorForce);
		}
#endregion


#region ApplyPacket
#if false
		public void Server_ApplyRequest(NetIncomingMessage request)
		{
			Idx = 0;
			Data = request.Data;
			ApplyNextPacket();
		}
#endif

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
					ApplyPacket(RawDataToPacket<Packet_MotorForce>());
					break;
				case (byte)CommandId.RobotCommand:
					ApplyPacket(RawDataToPacket<Packet_RobotCommand>());
					break;
				case (byte)CommandId.ObjectTransform:
					ApplyPacket(RawDataToPacket<Packet_ObjectTransform>());
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
			}
		}

		private void ApplyPacket(Packet_ObjectTransform packet)
		{
			Body b = null;
			if (packet.IsRobot)
			{
				if (Game.Robots.Count > packet.Id)
				{
					Robot r = Game.Robots[packet.Id];
					Debug.Assert(!r.Extracted);
					if (!r.Extracted)
					{
						b = r.Heart;
					}
				}
			}
			else
			{
				if (Game.Map.Chunks.Count > packet.Id)
				{
					b = Game.Map.Chunks[packet.Id];
				}
			}

			Debug.Assert(b != null);
			if (b != null)
			{
				b.AngularVelocity = packet.AngularVelocity;
				b.LinearVelocity = packet.LinearVelocity;
				b.Rotation = packet.Rotation;
				b.Position = packet.Position;
			}
		}

		private void ApplyPacket(Packet_MotorForce packet)
		{
			Debug.Assert(Game.Robots.Count > packet.RobotId);
			if (Game.Robots.Count > packet.RobotId)
			{
				Robot r = Game.Robots[packet.RobotId];
				Debug.Assert(r.Spots.Count > packet.MotorId);
				Debug.Assert(!r.Extracted);
				if (!r.Extracted && r.Spots.Count > packet.MotorId)
				{
					r.Spots[packet.MotorId].Force = packet.Force;
				}
			}
		}
#endregion
	}
}
