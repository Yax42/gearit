﻿using System;
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
    /// <summary>
    /// Network specific server
    /// </summary>
    class NetworkServer : INetwork
    {
		public static NetworkServer Instance { get; private set; }
        public string _buffer;
        public string Name = "Test server";
        private Thread serverThread;
        private bool _server_launched;
		private NetworkServerGame Game;
		override public string Path { get {return "data/net/server/";} }


		private NetworkServer(NetworkServerGame game, NetPeerConfiguration config, int port)
			: base(new NetServer(config), 2, game.PacketManager)
		{
			Game = game;
			ResetToSends();
			Port = port;
			IsServer = true;
		}

       static public void Start(int port)
		{
            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            config.MaximumConnections = 100;
            config.Port = port;
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
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
                OutputManager.LogNetwork("(Server) Launched");
            }
            catch
            {
                OutputManager.LogNetwork("Server - Fail to Launch server");
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
            while (true)
            {
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
						case NetIncomingMessageType.DiscoveryRequest:
							NetOutgoingMessage response = Peer.CreateMessage();
							response.Write(Name);
							response.Write(Game.Robots.Count);
							response.Write(Game.Map.Name);
							response.Write(Game.Time);
							Peer.SendDiscoveryResponse(response, msg.SenderEndPoint);
							break;
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
								Peer p = AddPeer(msg.SenderConnection);
								Server_BruteSend(p);
                                OutputManager.LogNetwork("SERVER - Remote hail: " + msg.SenderConnection.RemoteHailMessage.ReadString());
                                foreach (NetConnection conn in Peer.Connections)
                                {
                                    string str = NetUtility.ToHexString(conn.RemoteUniqueIdentifier) + " from " + conn.RemoteEndPoint.ToString() + " (" + conn.Status + ")";
                                    OutputManager.LogNetwork(str);
                                }
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            Server_ManageRequest(msg);
							toRecycle = false;
                            break;
                        default:
                            OutputManager.LogNetwork("SERVER - Unhandled type: " + msg.MessageType);
                            break;
                    }
					if (toRecycle)
						Peer.Recycle(msg);
                }
            }
        }

		public void Server_ManageRequest(NetIncomingMessage msg)
		{
			ManageRequest(msg);
			Peer peer = GetPeer(msg);
			foreach (Peer p in Peers)
				;// if (p != peer)
				//	PushRequest(msg.Data, p);
		}

		protected override byte[] Events
		{
			get
			{
				return Game.Events;
			}
			set
			{
				Game.Events = new byte[0];
			}
		}
		
		private void Server_BruteSend(Peer target)
		{
			BruteSend(target, PacketManager.Map(Game.Map, target.Id));
			foreach (Peer p in Peers)
				if (p != target)
					BruteSend(target, PacketManager.Robot(Game.RobotFromId(p.Id)));
			BruteSend(target, PacketManager.GameCommandToBytes(PacketManager.GameCommand.Go));
		}

		public void Server_BruteSpreadRobot(int id)
		{
			Robot r = Game.RobotFromId(id);
			foreach (Peer p in Peers)
				if (id != p.Id)
					BruteSend(p, PacketManager.Robot(r));
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
