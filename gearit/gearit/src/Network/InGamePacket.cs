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
			Message,
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
			RobotTransform,
			MotorForce,
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

		private struct Packet_RobotTransform // size 28
		{
			public byte RobotId;
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
			string msg;
			int duration;
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

		static private byte[] PacketToRawData<T>(T packet, CommandId id) where T : struct
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

		public byte[] RobotTransform(Robot r)
		{
			Heart h = r.Heart;

			Packet_RobotTransform res = new Packet_RobotTransform();

			res.RobotId = (byte) r.Id;
			res.Position = r.Position;
			res.Rotation = h.Rotation;
			res.LinearVelocity = h.LinearVelocity;
			res.AngularVelocity = h.AngularVelocity;
			return PacketToRawData(res, CommandId.RobotTransform);
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
		public void ApplyRequest(NetIncomingMessage request)
		{
			Data = request.Data;
			Idx = 0;
			//while (ApplyNextPacket()) ;
			ApplyNextPacket();
		}

		public bool ApplyNextPacket()
		{
			if (Data == null)
				return false;
			if (Idx + 1 >= Data.Count())
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
				case (byte)CommandId.RobotTransform:
					ApplyPacket(RawDataToPacket<Packet_RobotTransform>());
					break;
				default:
					return false;
					break;
			}
			//Debug.Assert(Idx == Data.Count());
			return Idx != Data.Count();
		}

		private void ApplyPacket(Packet_GameCommand packet)
		{
			switch (packet.CommandId)
			{
				case (byte) GameCommand.End:
					break;
				case (byte) GameCommand.Pause:
					Game.Finish();
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

		private void ApplyPacket(Packet_RobotTransform packet)
		{
			Debug.Assert(Game.Robots.Count > packet.RobotId);
			if (Game.Robots.Count > packet.RobotId)
			{
				Robot r = Game.Robots[packet.RobotId];
				Debug.Assert(!r.Extracted);
				if (!r.Extracted)
				{
					Heart h = r.Heart;

					h.AngularVelocity = packet.AngularVelocity;
					h.LinearVelocity = packet.LinearVelocity;
					h.Rotation = packet.Rotation;
					r.Position = packet.Position;
				}
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
