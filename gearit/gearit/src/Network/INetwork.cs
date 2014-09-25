using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Diagnostics;

namespace gearit.src.Network
{
	class INetwork
	{
		private static byte[][] TransformToSend;
		private static byte[][] ToSend;
		public static int Port;
		public static string Host;

		public static List<NetIncomingMessage> Requests = new List<NetIncomingMessage>();
		protected static InGamePacketManager PacketManager;
		protected static NetIncomingMessage[] YoungestRequests;
		protected static NetPeer Peer;
		protected static int NumberOfPeers;

		protected static void init (int numberOfPeers)
		{
			NumberOfPeers = numberOfPeers;
			ToSend = new byte[NumberOfPeers][];
			TransformToSend = new byte[NumberOfPeers][];
		}

		public static void CleanRequests()
		{
			foreach (NetIncomingMessage request in Requests)
				Peer.Recycle(request);
			Requests.Clear();
			for (int i = 0; i < NumberOfPeers; i++)
				YoungestRequests[i] = null;
		}

		public static void ManageRequest(NetIncomingMessage msg, int idx)
		{
			Requests.Add(msg);
			if (YoungestRequests[idx] == null ||
				BitConverter.ToInt32(YoungestRequests[idx].Data, 0) < BitConverter.ToInt32(msg.Data, 0))
				YoungestRequests[idx] = msg;
			/*
			Requests.Add(msg);
			int id = 0;
			if (s_server.Connections[0] == msg.SenderConnection)
				id = 1;
			PushRequest(msg.Data, id);
			*/
		}

		public static int UserId(NetIncomingMessage request)
		{
			for (int i = 0; i < NumberOfPeers; i++)
			{
				if (Peer.Connections[i] == request.SenderConnection)
					return i;
			}
			Debug.Assert(false);
			return (0);
		}

		public static bool IsYoungest(NetIncomingMessage request)
		{
			return request == YoungestRequests[UserId(request)];
		}

		public static void ApplyRequests()
		{
			Console.Out.WriteLine("" + Requests.Count);
			foreach (NetIncomingMessage request in Requests)
			{
				PacketManager.Client_ApplyRequest(request, IsYoungest(request));
			}
			CleanRequests();
		}

		public static void SendRequests()
		{
			for (int i = 0; i < NumberOfPeers; i++)
			{
				if (i >= Peer.Connections.Count)
					continue;
				if (TransformToSend.Count() > 0)
				{
					PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.BeginTransform), i);
					PushRequest(TransformToSend[i], i);
				}
				PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.EndOfPacket), i);
				NetOutgoingMessage om = Peer.CreateMessage();
				om.Write(ToSend[i]);
				Peer.SendMessage(om, Peer.Connections[i], NetDeliveryMethod.Unreliable, 0);
			}
			ResetToSends();
		}

		private static void ResetToSends()
		{
			for (int i = 0; i < 2; i++)
			{
				TransformToSend[i] = new byte[0];
				ToSend[i] = BitConverter.GetBytes(Game.FrameCount);
				if (BitConverter.IsLittleEndian)
					Array.Reverse(ToSend[i]);
			}
		}

		public static void PushRequest(byte[] data, int robotId)
		{
			ToSend[robotId] = ToSend[robotId].Concat(data).ToArray();
		}


		public static void ManageRequest(NetIncomingMessage msg)
		{
			Requests.Add(msg);
			int id = 0;
			if (s_server.Connections[0] == msg.SenderConnection)
				id = 1;
			PushRequest(msg.Data, id);
		}



		public static void PushRequestTransform(byte[] data, int robotId)
		{
			TransformToSend[robotId] = TransformToSend[robotId].Concat(data).ToArray();
		}

		public static void ApplyRequests()
		{
			foreach (NetIncomingMessage request in Requests)
				PacketManager.Server_ApplyRequest(request);
			CleanRequests();
		}

	}
}
	}
}
