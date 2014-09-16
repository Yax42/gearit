using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using gearit.src.output;

using Lidgren.Network;

namespace gearit.src.Network
{
    static class NetworkServer
    {
        public static int _port;
        private static NetServer s_server;
        public static string _buffer;
        private static Thread serverThread;
        private static bool _server_launched;
		private static NetworkServerGame Game;
        public static List<NetIncomingMessage> Requests = new List<NetIncomingMessage>();

        public static void Start(int port)
        {
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
            while (true)
            {
				Game.Update(null);
                NetIncomingMessage msg;
                while ((msg = s_server.ReadMessage()) != null)
                {
                    //OutputManager.LogMessage("Server received : " + msg.MessageType);
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            NetIncomingMessage hail = msg.SenderConnection.RemoteHailMessage;
                            Console.WriteLine(hail.ReadString());
                            //OutputManager.LogMessage("SERVER - msg:" + hail.ReadString());
                            msg.SenderConnection.Approve();
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //OutputManager.LogError("SERVER - error: " + msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                            string reason = msg.ReadString();
                            //OutputManager.LogMessage(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                            {
                                //OutputManager.LogMessage("SERVER - Remote hail: " + msg.SenderConnection.RemoteHailMessage.ReadString());
                                foreach (NetConnection conn in s_server.Connections)
                                {
                                    string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                                    OutputManager.LogMessage(str);
                                    //NetOutgoingMessage om = s_server.CreateMessage();
                                    //om.Write("You are connected!");
                                    //s_server.SendMessage(om, conn, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            manageRequest(msg);
                            break;
                        default:
                            OutputManager.LogError("SERVER - Unhandled type: " + msg.MessageType);
                            break;
                    }
                    s_server.Recycle(msg);
                }
                Thread.Sleep(1);
            }
        }

        public static void Send(string text)
        {
                NetOutgoingMessage om = s_server.CreateMessage();
                om.Write(text);
                s_server.SendMessage(om, s_server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public static void Send(byte[] data, int id)
        {
			if (id >= s_server.Connections.Count)
				return;
            NetOutgoingMessage om = s_server.CreateMessage();
            om.Write(data);
            s_server.SendMessage(om, s_server.Connections[id], NetDeliveryMethod.ReliableOrdered, 0);
        }

        public static void manageRequest(NetIncomingMessage msg)
        {
            Requests.Add(msg);
			int id = 0;
			if (s_server.Connections[0] == msg.SenderConnection)
				id = 1;
			Send(msg.Data, id);

            //OutputManager.LogMessage("Server received new request");
        }

		public static void ApplyRequests(InGamePacketManager packetManager)
		{
			foreach (NetIncomingMessage request in NetworkClient.Requests)
				packetManager.ApplyRequest(request);
			NetworkClient.CleanRequests();
		}
    }
}
