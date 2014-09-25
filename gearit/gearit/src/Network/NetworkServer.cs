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
    class NetworkServer : INetwork
    {
		public static NetworkServer Instance { get; private set; }
        public string _buffer;
        private Thread serverThread;
        private bool _server_launched;
		private NetworkServerGame Game;


		private NetworkServer(NetworkServerGame game, NetPeerConfiguration config, int port)
			: base(new NetServer(config), 2, game.PacketManager)
		{
			Game = game;
			Port = port;
			IsServer = true;
		}

       static public void Start(int port)
		{
            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            config.MaximumConnections = 100;
            config.Port = port;
			Instance = new NetworkServer(new NetworkServerGame(), config, port);
			Instance.PrivateStart();
		}

        private void PrivateStart()
        {
            _server_launched = false;
            try
            {
				Peer.Start();
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

        public void Stop()
        {
            if (_server_launched)
            {
                serverThread.Abort();
                Peer.Shutdown("Requested by user");
            }
        }

        public void ServerMainLoop()
        {
			Game.LoadContent();
			bool toRecycle;
			Stopwatch clock = Stopwatch.StartNew();
			ResetToSends();
            while (true)
            {
				ApplyRequests();
				while (clock.Elapsed.TotalMilliseconds < 18)
					Thread.Sleep(1);
				while (clock.Elapsed.TotalMilliseconds < 19)
					continue;
				clock.Stop();
				float delta = (float) clock.Elapsed.TotalMilliseconds;
				Game.Update(delta / 1000);
				SendRequests();

				clock = Stopwatch.StartNew();
                NetIncomingMessage msg;
                while ((msg = Peer.ReadMessage()) != null)
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
                                foreach (NetConnection conn in Peer.Connections)
                                {
                                    string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " [" + conn.Status + "]";
                                    OutputManager.LogMessage(str);
                                }
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            Server_ManageRequest(msg);
							toRecycle = false;
                            break;
                        default:
                            OutputManager.LogError("SERVER - Unhandled type: " + msg.MessageType);
                            break;
                    }
					if (toRecycle)
						Peer.Recycle(msg);
                }
            }
        }

		public void Server_ManageRequest(NetIncomingMessage msg)
		{
			int id = 0;
			if (Peer.Connections[0] == msg.SenderConnection)
				id = 1;
			ManageRequest(msg, id);
			PushRequest(msg.Data, id);
		}

#if false
        public void oldSend(string text) // deprecated
        {
                NetOutgoingMessage om = Peer.CreateMessage();
                om.Write(text);
                Peer.SendMessage(om, Peer.Connections, NetDeliveryMethod.Unreliable, 0);
        }

        public void oldSend(byte[] data, int id) // deprecated
        {
			if (id >= Peer.Connections.Count)
				return;
            NetOutgoingMessage om = Peer.CreateMessage();
            om.Write(data);
            Peer.SendMessage(om, Peer.Connections[id], NetDeliveryMethod.Unreliable, 0);
        }
#endif
	}
}
