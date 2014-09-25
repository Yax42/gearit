﻿using System;
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
            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            config.MaximumConnections = 100;
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
                OutputManager.LogNetwork(serverThread.IsAlive.ToString());
                _server_launched = true;
            }
            catch
            {
                OutputManager.LogNetwork("[Server] Fail to Launch server");
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
                    OutputManager.LogNetwork("Server received : " + msg.MessageType);
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            NetIncomingMessage hail = msg.SenderConnection.RemoteHailMessage;
                            Console.WriteLine(hail.ReadString());
                            OutputManager.LogNetwork("SERVER - msg:" + hail.ReadString());
                            msg.SenderConnection.Approve();
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            OutputManager.LogNetwork("SERVER - error: " + msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();

                            string reason = msg.ReadString();
                            OutputManager.LogMessage(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                            {
                                OutputManager.LogNetwork("SERVER - Remote hail: " + msg.SenderConnection.RemoteHailMessage.ReadString());
                                foreach (NetConnection conn in s_server.Connections)
                                {
                                    string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                                    OutputManager.LogMessage(str);
                                    NetOutgoingMessage om = s_server.CreateMessage();
                                    om.Write("You are connected!");
                                    s_server.SendMessage(om, conn, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            string rcv = msg.ReadString();
                            OutputManager.LogNetwork("SERVER - msg: " + rcv);

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
                            OutputManager.LogNetwork("SERVER - Unhandled type: " + msg.MessageType);
                            break;
                    }
                    s_server.Recycle(msg);
                }
                Thread.Sleep(1);
            }
        }

        public void Send(string text)
        {
                NetOutgoingMessage om = s_server.CreateMessage();
                om.Write(text);
                s_server.SendMessage(om, s_server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
