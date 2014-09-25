using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using gearit.src.output;

using Lidgren.Network;
using System.Diagnostics;
using gearit.src.robot;

namespace gearit.src.Network
{
    static class NetworkServer
    {
		private static byte[][] ToSend = new byte[2][];
		private static byte[][] TransformToSend = new byte[2][];
        public static int _port;
        private static NetServer s_server;
        public static string _buffer;
        private static Thread serverThread;
        private static bool _server_launched;
		private static NetworkServerGame Game;
        public static List<NetIncomingMessage> Requests = new List<NetIncomingMessage>();
		private static InGamePacketManager PacketManager;

        public static void Start(int port)
        {
			ToSend[0] = null;
			ToSend[1] = null;

            _port = port;
            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            config.MaximumConnections = 100;
            config.Port = _port;

            s_server = new NetServer(config);
            _server_launched = false;

            try
            {
                s_server.Start();
                serverThread = new Thread(ServerMainLoop);
                serverThread.Start();
                OutputManager.LogMessage(serverThread.IsAlive.ToString());
                _server_launched = true;
                OutputManager.LogMessage("[Server] Launched");
            }
            catch
            {
                OutputManager.LogError("Server - Fail to Launch server");
            }
        }

        public static void Stop()
        {
            if (_server_launched)
            {
                serverThread.Abort();
                s_server.Shutdown("Requested by user");
            }
        }

        public static void ServerMainLoop()
        {
			Game = new NetworkServerGame();
			Game.LoadContent();
			bool toRecycle;
			Stopwatch clock = Stopwatch.StartNew();
			ResetToSends();
			PacketManager = Game.PacketManager;
            while (true)
            {
				while (clock.Elapsed.TotalMilliseconds < 18)
					Thread.Sleep(1);
				while (clock.Elapsed.TotalMilliseconds < 19)
					continue;
				clock.Stop();
				float delta = (float) clock.Elapsed.TotalMilliseconds;
				//Console.WriteLine("server delta: " + delta);
				Game.Update(delta / 1000);
				SendRequests();

				clock = Stopwatch.StartNew();
                NetIncomingMessage msg;
                while ((msg = s_server.ReadMessage()) != null)
                {
					toRecycle = true;
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            NetIncomingMessage hail = msg.SenderConnection.RemoteHailMessage;
                            Console.WriteLine(hail.ReadString());
                            OutputManager.LogMessage("SERVER - msg:" + hail.ReadString());
                            msg.SenderConnection.Approve();
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            OutputManager.LogError("SERVER - error: " + msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                            string reason = msg.ReadString();
                            OutputManager.LogMessage(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                            {
                                OutputManager.LogMessage("SERVER - Remote hail: " + msg.SenderConnection.RemoteHailMessage.ReadString());
                                foreach (NetConnection conn in s_server.Connections)
                                {
                                    string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                                    OutputManager.LogMessage(str);
                                }
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            ManageRequest(msg);
							toRecycle = false;
                            break;
                        default:
                            OutputManager.LogError("SERVER - Unhandled type: " + msg.MessageType);
                            break;
                    }
					if (toRecycle)
						s_server.Recycle(msg);
                }
            }
        }

        public static void oldSend(string text) // deprecated
        {
                NetOutgoingMessage om = s_server.CreateMessage();
                om.Write(text);
                s_server.SendMessage(om, s_server.Connections, NetDeliveryMethod.Unreliable, 0);
        }

        public static void oldSend(byte[] data, int id) // deprecated
        {
			if (id >= s_server.Connections.Count)
				return;
            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(data);
            s_server.SendMessage(om, s_server.Connections[id], NetDeliveryMethod.Unreliable, 0);
        }

        public static void SendRequests()
        {
			for (int i = 0; i < 2; i++)
			{
				if (i >= s_server.Connections.Count)
					continue;
				PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.BeginTransform), i);
				PushRequest(TransformToSend[i], i);
				PushRequest(PacketManager.CreatePacket(InGamePacketManager.CommandId.EndOfPacket), i);
				NetOutgoingMessage om = s_server.CreateMessage();
				om.Write(ToSend[i]);
				s_server.SendMessage(om, s_server.Connections[i], NetDeliveryMethod.Unreliable, 0);
			}
			ResetToSends();
        }

        public static void ManageRequest(NetIncomingMessage msg)
        {
			Requests.Add(msg);
			int id = 0;
			if (s_server.Connections[0] == msg.SenderConnection)
				id = 1;
			PushRequest(msg.Data, id);
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

		public static void CleanRequests()
		{
			foreach (NetIncomingMessage request in Requests)
				s_server.Recycle(request);
			Requests.Clear();
		}
    }
}
