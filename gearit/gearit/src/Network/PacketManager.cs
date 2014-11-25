using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using gearit.src.game;
using System.Diagnostics;
using gearit.src.robot;
using Lidgren.Network;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using System.IO;
using gearit.src.utility;
using gearit.src.map;
using gearit.src.editor;
using gearit.src.GUI;

namespace gearit.src.Network
{
	class PacketManager
	{
		#region enum
		public enum GameCommand
		{
			Go,
			End,
			Pause,
			Unpause,
		};
		public enum ERobotCommand
		{
			Remove,
			Win,
			Lose,
			Teleport,
			CameraDynamic,
			CameraStatic,
			Score,
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
			File,
			Disconnect,
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

		public enum FileType
		{
			Robot,
			Map,
		};

		private struct Packet_File
		{
			public byte Type;
			public int Size;
			public byte RobotId;
		}

		private struct Packet_RobotCommand // size 2
		{
			public byte RobotId;
			public byte Command;
			public Vector2 Position;
			public float Float;
			public int Int;
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
			public Transform Xf;
		}

		public enum MotorType
		{
			Frozen,
			Force,
			AddLimit,
			FreeWheel,
		};

		private struct Packet_Motor // size 8
		{
			public byte RobotId;
			public ushort MotorId;
			public byte Type;
			public float Force;
			public bool BoolVal;
			public short NbCycle;
		}

		private struct Packet_Message
		{
			public byte Id;
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
		public INetwork Network;
		private int Idx;
		public byte[] Data;
#endregion

		public PacketManager(IGearitGame game)
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

		public byte[] RemoveRobot(int id)
		{
			var packet = new Packet_RobotCommand();
			packet.RobotId = (byte) id;
			packet.Command = (byte)ERobotCommand.Remove;
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

		public byte[] Camera(int id, bool dynamic, Vector2 pos, float zoom)
		{
			var packet = new Packet_RobotCommand();
			packet.RobotId = (byte) id;
			if (dynamic)
				packet.Command = (byte)ERobotCommand.CameraDynamic;
			else
				packet.Command = (byte)ERobotCommand.CameraStatic;
			packet.Position = pos;
			packet.Float = zoom;
			return PacketToRawData(packet, CommandId.RobotCommand);
		}

		public byte[] Score(Robot r)
		{
			var packet = new Packet_RobotCommand();
			packet.RobotId = (byte) r.Id;
			packet.Command = (byte) ERobotCommand.Score;
			packet.Float = r.Score.FloatScore;
			packet.Int = r.Score.IntScore;
			return PacketToRawData(packet, CommandId.RobotCommand);
		}

		public byte[] Message(string msg, int from, int duration = 0)
		{
			var packet = new Packet_Message();
			packet.Id = (byte)from;
			packet.msg = msg;
			packet.duration = duration;
			return PacketToRawData(packet, CommandId.Message);
		}

		public byte[] RobotTransform(int idRobot)
		{
			return RobotTransform(Game.Robots[idRobot]);
		}

		public byte[] GameCommandToBytes(GameCommand cmd)
		{
			byte[] res = new byte[2];
			res[0] = (byte) (CommandId.GameCommand);
			res[1] = (byte)(cmd);
			return res;
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
			res.Xf = b.Xf;
			return PacketToRawData(res, CommandId.ObjectTransform);
		}

		public byte[] FileToBytes(GI_File f)
		{
			return File.ReadAllBytes(f.FullPath);	
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

		public byte[] Motor(RevoluteSpot spot, MotorType type, bool BoolVal)
		{
			return Motor(spot, type, BoolVal, 0, 0);
		}

		public byte[] Motor(RevoluteSpot spot, MotorType type, bool BoolVal, int cycle, float force)
		{
			Packet_Motor res = new Packet_Motor();
			res.MotorId = (ushort) spot.Id;
			res.BoolVal = BoolVal;
			res.Force = force;
			res.Type = (byte)type;
			res.NbCycle = (short)cycle;
			res.RobotId = (byte)Game.MainRobotId;
			return PacketToRawData(res, CommandId.MotorForce);
		}

		public byte[] Map(Map m, int robotId)
		{
			Packet_File res = new Packet_File();
			res.RobotId = (byte) robotId;
			res.Type = (byte)FileType.Map;
			byte[] file = FileToBytes(m);
			res.Size = file.Count();
			byte[] byteRes = PacketToRawData(res, CommandId.File);
			return byteRes.Concat(file).ToArray();
		}

		public byte[] Robot(Robot r)
		{
			return Robot(r.FullPath, r.Id);
		}

		public byte[] Robot(string path, int id = 0)
		{
			Packet_File res = new Packet_File();
			res.RobotId = (byte)id;
			res.Type = (byte)FileType.Robot;
			byte[] file = File.ReadAllBytes(path);	
			res.Size = file.Count();
			byte[] byteRes = PacketToRawData(res, CommandId.File);
			return byteRes.Concat(file).ToArray();
		}
#endregion

//----------------------------------------------------------------------------

#region ApplyPacket

		public bool ApplyBruteRequest(NetIncomingMessage request)
		{
			Idx = 4;
			Data = request.Data;
			if (BitConverter.ToInt32(Data, 0) == -1)
			{
				ApplyNextPacket();
				return true;
			}
			return false;
		}

		public void ApplyRequest(NetIncomingMessage request, bool proceedTransform)
		{
			Idx = 4;
			Data = request.Data;
			while (ApplyNextPacket(proceedTransform))
				;
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
				case (byte)CommandId.File:
					ApplyPacket(RawDataToPacket<Packet_File>());
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
			if (packet.Id == -1)
				Game.Message(packet.msg, packet.duration);
			else
			{
				if (Network.GetType() == typeof(NetworkServer))
					Network.BruteSpread(Message(packet.msg, packet.Id));
				else
					ChatBox.addEntry(Game.Robots[packet.Id].Owner + ": " + packet.msg);
			}

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
				case (byte) GameCommand.Unpause:
					break;
				case (byte) GameCommand.Go:
					Game.Go();
					break;
			}
		}

		private void ApplyPacket(Packet_RobotCommand packet)
		{
			Robot r = Game.RobotFromId(packet.RobotId);
			if (r == null)
				return;
			Debug.Assert(!r.Extracted);
			if (r.Extracted)
				return;
			switch (packet.Command)
			{
				case (byte)ERobotCommand.Lose:
					break;
				case (byte)ERobotCommand.Win:
					Game.Finish();
					break;
				case (byte)ERobotCommand.Remove:
					r.ExtractFromWorld();
					Game.Robots.Remove(r);
					break;
				case (byte)ERobotCommand.Teleport:
					r.Position = packet.Position;
					if (r.Id == Game.MainRobotId && Game.Camera != null)
						Game.Camera.Jump2Target();
					//Vector2 deltaPos = packet.Position - r.Position;
					//foreach (Piece p in r.Pieces)
					//	p.Position += deltaPos;
					break;
				case (byte)ERobotCommand.CameraDynamic:
					Game.Camera.EnablePositionTracking = true;
					break;
				case (byte)ERobotCommand.CameraStatic:
					Game.Camera.StaticCamera(packet.Position, packet.Float);
					break;
				case (byte)ERobotCommand.Score:
					r.Score.IntScore = packet.Int;
					r.Score.FloatScore = packet.Float;
					break;
			}
		}

		private void ApplyPacket(Packet_ObjectTransform packet)
		{
			Body b = null;
			if (packet.Type == (byte) TransformType.Robot)
			{
				Robot r = Game.RobotFromId(packet.RobotId);
				if (r != null)
				{
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
				Robot r = Game.RobotFromId(packet.RobotId);
				if (r != null)
				{
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
				b.Xf = packet.Xf;
			}
		}

		private GI_File LoadFile(string path, int size)
		{
			byte[] data = new byte[size];
			Array.Copy(Data, Idx, data, 0, size);
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
			File.WriteAllBytes(path, data);
			return (GI_File) Serializer.DeserializeItem(path);
		}

		private void ApplyPacket(Packet_File packet)
		{
			if (packet.Type == (byte) FileType.Map)
			{
				Game.Robots[0].Id = packet.RobotId;
				Game.MainRobotId = packet.RobotId;
				Robot r = Game.Robots[0];
				string backupPath = r.FullPath;
				r.FullPath = GI_Data.ClientPath + "MyRobot.gir";
				r.Save();
				Network.BruteSend(null, Robot(r.FullPath, packet.RobotId));
				r.FullPath = backupPath;
				Game.Map = (Map)LoadFile(GI_Data.ServerPath + "map.gim", packet.Size);
			}
			else
			{
				string path = GI_Data.ServerPath + "robot_" + packet.RobotId + ".gim";
				SerializerHelper.World = Game.World;
				Robot r = (Robot) LoadFile(path, packet.Size);
				r.Id = packet.RobotId;
				Game.AddRobot(r);
				if (Network.GetType() == typeof(NetworkServer))
				{
					((NetworkServer)Network).Server_BruteSpreadRobot(r.Id);
					Network.BruteSend(Network.PeerFromId(packet.RobotId), GameCommandToBytes(GameCommand.Go));
				}
			}
		}

		private void ApplyPacket(Packet_Motor packet)
		{
			Robot r = Game.RobotFromId(packet.RobotId);
			if (r != null)
			{
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
							r.Spots[packet.MotorId].Frozen = packet.BoolVal;
							break;
						case (byte) MotorType.AddLimit:
							r.Spots[packet.MotorId].AddLimitsCycle(packet.NbCycle);
							break;
						case (byte) MotorType.FreeWheel:
							r.Spots[packet.MotorId].FreeWheel = packet.BoolVal;
							break;
					}
				}
			}
		}
#endregion
	}
}
