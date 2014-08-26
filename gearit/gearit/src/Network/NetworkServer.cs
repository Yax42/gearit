using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using gearit.src.output;

using Lidgren.Network;

namespace gearit.src.server
{
    static class NetworkServer
    {
        public static int _port;
        private static NetServer s_server;
        public static string _buffer;
        private static Thread serverThread;
        private static bool _server_launched;

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
            while (true)
            {
                NetIncomingMessage msg;
                while ((msg = s_server.ReadMessage()) != null)
                {
                    OutputManager.LogMessage("Server received : " + msg.MessageType);
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
                                    //NetOutgoingMessage om = s_server.CreateMessage();
                                    //om.Write("You are connected!");
                                    //s_server.SendMessage(om, conn, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            string rcv = msg.ReadString();
                            OutputManager.LogMessage("SERVER - msg: " + rcv);
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
    }
}
