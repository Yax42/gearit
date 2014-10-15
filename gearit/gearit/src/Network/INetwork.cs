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
		protected byte[] TransformToSend;
		protected int Port;
		protected string Host;

		public List<NetIncomingMessage> Requests = new List<NetIncomingMessage>();
		private InGamePacketManager PacketManager;
		protected NetPeer Peer;
		protected List<Peer> Peers;

		protected bool IsServer = false;
		private int FrameCount;

		protected INetwork(NetPeer peer, int numberOfPeers, InGamePacketManager packetManager)
		{
			Peer = peer;
			PacketManager = packetManager;
			Peers = new List<Peer>();
			CleanRequests();
			FrameCount = 0;
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

		public void AddPeer(NetConnection p)
		{
			Peers.Add(new Peer(FirstUnusedId, p));
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
					PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.BeginTransform), p);
					PushRequest(TransformToSend, p);
				}
				PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.EndOfPacket), p);
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
			foreach (NetIncomingMessage request in Requests)
				PacketManager.ApplyRequest(request, IsYoungest(request));
			CleanRequests();
		}
	}
}
