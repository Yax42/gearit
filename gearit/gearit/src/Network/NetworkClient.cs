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
    static class NetworkClient
    {
		private static byte[][] ToSend = new byte[2][];
        public static int Port;
        public static string Host;
        private static NetClient s_client;

        public enum EState
        {
            Connecting,
            Connected,
            Disconnected
        }
        public static EState State = EState.Disconnected;
        private static Mutex mutex = new Mutex();
        private static Thread clientThread;
        private static InGamePacketManager PacketManager;
        public static List<NetIncomingMessage> Requests = new List<NetIncomingMessage>();
		private static NetIncomingMessage YoungestRequest = null;

        private static void tryToConnect()
        {
            try
            {
                s_client.Start();
                NetOutgoingMessage hail = s_client.CreateMessage("Hello I want connection kthxbye");
                s_client.Connect(Host, Port, hail);
            }
            catch
            {
				//Lock tchat
                //OutputManager.LogError("CLIENT - Fail to connect to server");
            }
        }

        public static void Connect(string host, int port, InGamePacketManager packetManager)
        {
            if (State == EState.Connecting)
                return ;
			Debug.Assert(packetManager != null);
			PacketManager = packetManager;
			PacketsList.Clear();

            Disconnect();

            Port = port;
            Host = host;
            State = EState.Connecting;

            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            s_client = new NetClient(config);
            s_client.RegisterReceivedCallback(new SendOrPostCallback(Receive));

            clientThread = new Thread(tryToConnect);
            clientThread.Start();
        }

        public static void Disconnect()
        {
            if (State == EState.Connected)
            {
                s_client.Disconnect("Requested by user");
                s_client.Shutdown("Bye");
            }
        }

        public static void Receive(object peer)
        {
            NetIncomingMessage im = ((NetClient)peer).ReadMessage();
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
                    OutputManager.LogError("CLIENT - Status Changed: " + status + " (" + im.ReadString() + ")");
                    break;
                case NetIncomingMessageType.Data:
                    ManageRequest(im);
					return;
					break;
                default:
                    OutputManager.LogError("CLIENT - Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                    break;
            }
            s_client.Recycle(im);
        }

        public static void Send(string text)
		{
            if (State == EState.Connected)
            {
                NetOutgoingMessage om = s_client.CreateMessage();
                om.Write(text);
                s_client.SendMessage(om, s_client.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

		public static void CleanRequests()
		{
			foreach (NetIncomingMessage request in Requests)
				s_client.Recycle(request);
			Requests.Clear();
			YoungestRequest = null;
		}

        public static void Send(byte[] data)
		{
            if (State == EState.Connected)
            {
                NetOutgoingMessage om = s_client.CreateMessage();
                om.Write(data);
                s_client.SendMessage(om, s_client.Connections, NetDeliveryMethod.Unreliable, 0);
            }
        }


		static private Mutex PacketMutex = new Mutex();
		static private List<NetIncomingMessage> PacketsList = new List<NetIncomingMessage>();
		static public NetIncomingMessage Packet
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

        public static void ManageRequest(NetIncomingMessage msg)
        {
            Requests.Add(msg);
			if (YoungestRequest == null ||
				BitConverter.ToInt32(YoungestRequest.Data, 0) < BitConverter.ToInt32(msg.Data, 0))
				YoungestRequest = msg;
        }

		public static void ApplyRequests()
		{
			Console.Out.WriteLine("" + Requests.Count);
			foreach (NetIncomingMessage request in Requests)
			{
				PacketManager.Client_ApplyRequest(Requests.First(), request == YoungestRequest);
			}
			CleanRequests();
		}

        public static void SendRequests()
        {
			for (int i = 0; i < 2; i++)
			{
				if (i >= s_client.Connections.Count)
					continue;
				PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.EndOfPacket), i);
				NetOutgoingMessage om = s_client.CreateMessage();
				om.Write(ToSend[i]);
				s_client.SendMessage(om, s_client.Connections[, NetDeliveryMethod.Unreliable, 0);
			}
			ResetToSends();
        }

		private static void ResetToSends()
		{
			for (int i = 0; i < 2; i++)
			{
				ToSend[i] = new byte[0];
			}
		}

		public static void PushRequest(byte[] data, int robotId)
		{
			ToSend[robotId] = ToSend[robotId].Concat(data).ToArray();
		}
	}
}
