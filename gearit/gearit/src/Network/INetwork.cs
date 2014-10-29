using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Diagnostics;
using gearit.src.robot;

namespace gearit.src.Network
{
    /// <summary>
    /// Abstract class Network for server and client
    /// </summary>
	class Peer
	{
		public int					Id;
		public byte[]				ToSend;
		public NetIncomingMessage	YoungestRequest;
		public NetConnection		Connect;
		public Peer(int id, NetConnection co)
		{
			Id = id;
			ToSend = BitConverter.GetBytes(0);
			YoungestRequest = null;
			co.Tag = this;
			Connect = co;
		}
	}

	abstract class INetwork
	{
		public static int SERVER_PORT = 25552;

		protected byte[] TransformToSend;
		protected int Port;
		protected string Host;

		public List<NetIncomingMessage> Requests = new List<NetIncomingMessage>();
		protected PacketManager PacketManager;
		protected NetPeer Peer;
		protected List<Peer> Peers;

		abstract public string Path { get; }
		protected bool IsServer = false;
		private int FrameCount;

		protected INetwork(NetPeer peer, int numberOfPeers, PacketManager packetManager)
		{
			Peer = peer;
			PacketManager = packetManager;
			Peers = new List<Peer>();
			CleanRequests();
			FrameCount = 0;
		}
		
		public Peer PeerFromId(int id)
		{
			foreach (Peer p in Peers)
				if (p.Id == id)
					return p;
			return null;
		}

		public int FirstUnusedId
		{
			get
			{
				bool done = false;
				int res = 0;
				while (!done)
				{
					done = true;
					foreach (Peer p in Peers)
					{
						if (p.Id == res)
						{
							done = false;
							res++;
							break;
						}
					}
				}
				return res;
			}
		}

		public Peer AddPeer(NetConnection co)
		{
			Peer p = new Peer(FirstUnusedId, co);
			Peers.Add(p);
			return p;
		}

		public void CleanRequests()
		{
			foreach (NetIncomingMessage request in Requests)
				Peer.Recycle(request);
			Requests.Clear();
			foreach (Peer p in Peers)
				p.YoungestRequest = null;
		}

		protected void ManageRequest(NetIncomingMessage msg)
		{
			Peer p = GetPeer(msg);
			Requests.Add(msg);
			if (p.YoungestRequest == null ||
				BitConverter.ToInt32(p.YoungestRequest.Data, 0) < BitConverter.ToInt32(msg.Data, 0))
				p.YoungestRequest = msg;
		}

		public Peer GetPeer(NetIncomingMessage request)
		{
			return ((Peer)request.SenderConnection.Tag);
		}

		private bool IsYoungest(NetIncomingMessage request)
		{
			return request == GetPeer(request).YoungestRequest;
		}

		protected abstract byte[] Events { get; set; }

		public void SendRequests()
		{
			foreach (Peer p in Peers)
			{
				PushRequest(Events, p);
				if (TransformToSend.Count() > 0)
				{
					PushRequest(PacketManager.CreatePacket(PacketManager.CommandId.BeginTransform), p);
					PushRequest(TransformToSend, p);
				}
				PushRequest(PacketManager.CreatePacket(PacketManager.CommandId.EndOfPacket), p);
				NetOutgoingMessage om = Peer.CreateMessage();
				om.Write(p.ToSend);
				Peer.SendMessage(om, p.Connect, NetDeliveryMethod.Unreliable, 0);
			}
			FrameCount++;
			ResetToSends();
		}

		protected void ResetToSends()
		{
			TransformToSend = new byte[0];
			Events = null;
			foreach (Peer p in Peers)
				p.ToSend = FrameCountByte;
		}

		protected byte[] FrameCountByte
		{
			get
			{
				byte[] res = BitConverter.GetBytes(FrameCount);
				if (BitConverter.IsLittleEndian)
					Array.Reverse(res);
				return res;
			}
		}

		public void Send(Robot r)
		{
			byte[] res = FrameCountByte;
		}

		public void PushRequest(byte[] data, Peer p = null)
		{
            if (Peers.Count == 0)
                return;
			if (p == null)
				p = Peers.First();
			p.ToSend = p.ToSend.Concat(data).ToArray();
		}

		public void PushRequestTransform(byte[] data)
		{
			TransformToSend = TransformToSend.Concat(data).ToArray();
		}

		public void ApplyRequests()
		{
			ApplyBruteRequests();
			foreach (NetIncomingMessage request in Requests)
				PacketManager.ApplyRequest(request, IsYoungest(request));
			CleanRequests();
		}

		public void BruteSend(Peer p, byte[] dataPart2)
		{
			if (p == null)
				p = Peers.First();
			byte[] data = BitConverter.GetBytes(-1);
			data = data.Concat(dataPart2).ToArray();
			NetOutgoingMessage om = Peer.CreateMessage();
			om.Write(data);
			Peer.SendMessage(om, p.Connect, NetDeliveryMethod.ReliableOrdered, 0);
		}

		public void ApplyBruteRequests()
		{
			Requests.RemoveAll(request => PacketManager.ApplyBruteRequest(request));
		}
	}
}
