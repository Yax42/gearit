using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Diagnostics;

namespace gearit.src.Network
{
	abstract class INetwork
	{
		protected byte[] TransformToSend;
		protected byte[][] ToSend;
		protected int Port;
		protected string Host;

		public List<NetIncomingMessage> Requests = new List<NetIncomingMessage>();
		private InGamePacketManager PacketManager;
		protected NetIncomingMessage[] YoungestRequests;
		protected NetPeer Peer;
		protected int NumberOfPeers;

		protected bool IsServer = false;
		private int FrameCount;

		protected INetwork(NetPeer peer, int numberOfPeers, InGamePacketManager packetManager)
		{
			Peer = peer;
			PacketManager = packetManager;
			NumberOfPeers = numberOfPeers;
			ToSend = new byte[NumberOfPeers][];
			YoungestRequests = new NetIncomingMessage[NumberOfPeers];
			CleanRequests();
			FrameCount = 0;
		}

		public void CleanRequests()
		{
			foreach (NetIncomingMessage request in Requests)
				Peer.Recycle(request);
			Requests.Clear();
			for (int i = 0; i < NumberOfPeers; i++)
				YoungestRequests[i] = null;
		}

		protected void ManageRequest(NetIncomingMessage msg, int idx)
		{
			Requests.Add(msg);
			if (YoungestRequests[idx] == null ||
				BitConverter.ToInt32(YoungestRequests[idx].Data, 0) < BitConverter.ToInt32(msg.Data, 0))
				YoungestRequests[idx] = msg;
		}

		private int UserId(NetIncomingMessage request)
		{
			for (int i = 0; i < NumberOfPeers; i++)
			{
				if (Peer.Connections[i] == request.SenderConnection)
					return i;
			}
			Debug.Assert(false);
			return (0);
		}

		private bool IsYoungest(NetIncomingMessage request)
		{
			return request == YoungestRequests[UserId(request)];
		}

		protected abstract byte[] Events { get; set; }

		public void SendRequests()
		{
			for (int i = 0; i < NumberOfPeers; i++)
			{
				if (i >= Peer.Connections.Count)
					continue;
				PushRequest(Events, i);
				if (TransformToSend.Count() > 0)
				{
					PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.BeginTransform), i);
					PushRequest(TransformToSend, i);
				}
				PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.EndOfPacket), i);
				NetOutgoingMessage om = Peer.CreateMessage();
				om.Write(ToSend[i]);
				Peer.SendMessage(om, Peer.Connections[i], NetDeliveryMethod.Unreliable, 0);
			}
			ResetToSends();
		}

		protected void ResetToSends()
		{
			TransformToSend = new byte[0];
			Events = null;
			for (int i = 0; i < NumberOfPeers; i++)
			{
				ToSend[i] = BitConverter.GetBytes(FrameCount++);
				if (BitConverter.IsLittleEndian)
					Array.Reverse(ToSend[i]);
			}
		}

		public void PushRequest(byte[] data, int userId = 0)
		{
			ToSend[userId] = ToSend[userId].Concat(data).ToArray();
		}

		public void PushRequestTransform(byte[] data)
		{
			TransformToSend = TransformToSend.Concat(data).ToArray();
		}

		public void ApplyRequests()
		{
			foreach (NetIncomingMessage request in Requests)
				PacketManager.ApplyRequest(request, IsYoungest(request));
			CleanRequests();
		}
	}
}
