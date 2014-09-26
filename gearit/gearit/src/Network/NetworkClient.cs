using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using gearit.src.output;
using System.Threading;
using gearit.xna;
using System.Diagnostics;

namespace gearit.src.Network
{
    class NetworkClient : INetwork
    {
        public enum EState
        {
            Connecting,
            Connected,
            Disconnected
        }
        public EState State = EState.Disconnected;
        private Mutex mutex = new Mutex();
        private Thread clientThread;

		public NetworkClient(InGamePacketManager packetManager)
			: base(new NetClient(new NetPeerConfiguration("gearit")), 1, packetManager)
		{
			ResetToSends();
		}

        private void tryToConnect()
        {
            try
            {
                Peer.Start();
                NetOutgoingMessage hail = Peer.CreateMessage("Hello I want connection kthxbye");
                Peer.Connect(Host, Port, hail);
            }
            catch
            {
				//Lock tchat
                //OutputManager.LogError("CLIENT - Fail to connect to server");
            }
        }

        public void Connect(string host, int port)
        {
            if (State == EState.Connecting)
                return ;

            Disconnect();

            Port = port;
            Host = host;
            State = EState.Connecting;

            Peer.RegisterReceivedCallback(new SendOrPostCallback(Receive));

            clientThread = new Thread(tryToConnect);
            clientThread.Start();
        }

        public void Disconnect()
        {
            if (State == EState.Connected)
            {
                ((NetClient) Peer).Disconnect("Requested by user");
                Peer.Shutdown("Bye");
            }
        }

        public void Receive(object peer)
        {
            NetIncomingMessage im = ((NetClient)peer).ReadMessage();
            //while ((msg = Peer.ReadMessage()) != null) { toRecycle = true;
            switch (im.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = im.ReadString();
                    OutputManager.LogError(text);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                    if (status == NetConnectionStatus.Connected)
                        State = EState.Connected;
                    else
                        State = EState.Disconnected;
                    OutputManager.LogNetwork("CLIENT - Status Changed: " + status + " (" + im.ReadString() + ")");
                    break;
                case NetIncomingMessageType.Data:
                    Client_ManageRequest(im);
					return;
					break;
                default:
                    OutputManager.LogNetwork("CLIENT - Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                    break;
            }
            Peer.Recycle(im);
        }

		public void Client_ManageRequest(NetIncomingMessage msg)
		{
			ManageRequest(msg, 0);
		}

		protected override byte[] Events { get { return new byte[0]; } set { } }

#if false
        public void Send(string text)
		{
            if (State == EState.Connected)
            {
                NetOutgoingMessage om = Peer.CreateMessage();
                om.Write(text);
                Peer.SendMessage(om, Peer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        public void Send(byte[] data)
		{
            if (State == EState.Connected)
            {
                NetOutgoingMessage om = Peer.CreateMessage();
                om.Write(data);
                Peer.SendMessage(om, Peer.Connections, NetDeliveryMethod.Unreliable, 0);
            }
        }

		private Mutex PacketMutex = new Mutex();
		public NetIncomingMessage Packet
		{
			get
			{
				PacketMutex.WaitOne();
				var res = PacketsList.First();
				PacketsList.RemoveAt(0);
				PacketMutex.ReleaseMutex();
				return res;
			}
			set
			{
				PacketMutex.WaitOne();
				PacketsList.Add(value);
				PacketMutex.ReleaseMutex();
			}
		}
#endif
	}
}
