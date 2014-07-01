using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using gearit.src.output;

using Lidgren.Network;

namespace gearit.src.server
{
    class InGameServer
    {
        public int _port;
        private NetServer s_server;
        public string _buffer;
        private Thread serverThread;
        private bool _server_launched;

        public InGameServer(int port)
        {
            _port = port;
            NetPeerConfiguration config = new NetPeerConfiguration("gear it");
            config.MaximumConnections = 4;
            config.Port = _port;

            s_server = new NetServer(config);
            _server_launched = false;
        }

        public void Start()
        {
            try
            {
                s_server.Start();
                serverThread = new Thread(ServerMainLoop);
                serverThread.Start();
                OutputManager.LogMessage(serverThread.IsAlive.ToString());
                _server_launched = true;
            }
            catch
            {
                OutputManager.LogError("(Server)msg:Fail to Launch server");
            }
        }

        public void Stop()
        {
            if (_server_launched)
            {
                serverThread.Abort();
                s_server.Shutdown("Requested by user");
            }
        }

        public void ServerMainLoop()
        {
            while (true)
            {
                NetIncomingMessage msg;
                while ((msg = s_server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            NetIncomingMessage hail = msg.SenderConnection.RemoteHailMessage;
                            Console.WriteLine(hail.ReadString());
                            OutputManager.LogMessage("(Server)msg:" + hail.ReadString());
                            msg.SenderConnection.Approve();
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            OutputManager.LogError(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                            string reason = msg.ReadString();
                            OutputManager.LogMessage(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                                OutputManager.LogMessage("Remote hail: " + msg.SenderConnection.RemoteHailMessage.ReadString());
                            foreach (NetConnection conn in s_server.Connections)
                            {
                                string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                                OutputManager.LogMessage(str);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            string rcv = msg.ReadString();
                            OutputManager.LogMessage("(Server)msg:" + rcv);

                            List<NetConnection> all = s_server.Connections;
                            all.Remove(msg.SenderConnection);

                            if (all.Count > 0)
                            {
                                NetOutgoingMessage om = s_server.CreateMessage();
                                om.Write(rcv);
                                s_server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            break;
                        default:
                            OutputManager.LogError("(Server)Unhandled type: " + msg.MessageType);
                            break;
                    }
                    s_server.Recycle(msg);
                }
                Thread.Sleep(1);
            }
        }
    }
}
