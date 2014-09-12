using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace gearit.src.Network
{
	class InGamePacket
	{
		enum RobotCommand
		{
			Remove,
			Win,
			Lose,
		};
		enum CommandId
		{
			RobotCommand,
			RobotTransform,
			MotorForce,
		};

		#region Packets
		private struct Packet_CommandRobot
		{
			public byte Id;
			public byte Command;

#if false
			public byte[]	GetRawData()
			{
				var res = new byte[3];
				res[0] = (byte)CommandId.RobotCommand;
				res[1] = Id;
				res[2] = Command;
				return res;

			}
#endif
		}

		private struct Packet_TransformRobot
		{
			public byte Id;
			public Vector2 Position;
			public float Angle;

#if false
			public byte[]	GetRawData()
			{
				var res = new byte[14];
				res[0] = (byte)CommandId.RobotTransform;
				res[1] = Id;
				CopyFloat(res, Position.X, 2);
				CopyFloat(res, Position.Y, 6);
				CopyFloat(res, Angle, 10);
				return res;
			}
#endif
		}

		private struct Packet_Motor
		{
			public byte RobotId;
			public byte MotorId;
			public float Force;

#if false
			public byte[]	GetRawData()
			{
				/*
			unsafe
			{
				//SizeOf
				fixed (Packet_Motor* packet = &this)
				{
					return (byte*)packet;
				}
			}
				*/
				var res = new byte[7];
				res[0] = (byte)CommandId.MotorForce;
				res[1] = RobotId;
				res[2] = MotorId;
				CopyFloat(res, Force, 3);
				return res;
			}
#endif
		}
		#endregion

		static byte[] CommandRobot(int id, RobotCommand command)
		{
			var packet = new Packet_CommandRobot();
			packet.Id = (byte) id;
			packet.Command = (byte) id;

			return PaquetToRawData(packet);
		}

		//static InGamePacket CreateFromRawData(byte[] data)

		static T RawDataToPacket<T>(byte[] data) where T : struct
		{
			unsafe
			{
				fixed (byte* p = &data[0])
				{
					return (T)Marshal.PtrToStructure(new IntPtr(p), typeof(T));
				}
			}
		}

		//http://stackoverflow.com/questions/3278827/how-to-convert-a-structure-to-a-byte-array-in-c

		static byte[] PaquetToRawData<T>(T paquet) where T : struct
		{
			int size = Marshal.SizeOf(paquet);
			byte[] arr = new byte[size];
			IntPtr ptr = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(paquet, ptr, true);
			Marshal.Copy(ptr, arr, 0, size);
			Marshal.FreeHGlobal(ptr);

			return arr;
		}

		private static void CopyFloat(byte[] source, float v, int idx)
		{
			Array.Copy(BitConverter.GetBytes(v), 0, source, idx, 4);
		}
	}
}
