using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using gearit.src.output;

using Lidgren.Network;
using System.Diagnostics;
using gearit.src.robot;
using gearit.src.editor.map;

namespace gearit.src.Network
{
    /// <summary>
    /// Network specific server
    /// </summary>
    class NetworkServer : INetwork
    {
		public static bool Running { get; private set; }
		public static NetworkServer Instance { get; private set; }
        public string _buffer;
        public string Name = "Test server";
        private Thread serverThread;
		private NetworkServerGame Game;
		override public string Path { get {return "data/net/server/";} }
		public int MaxPlayers { get; set; }
		public bool LockConnections { get; set; }


		private NetworkServer(NetworkServerGame game, NetPeerConfiguration config, int port)
			: base(new NetServer(config), 2, game.PacketManager)
		{
			Game = game;
			ResetToSends();
			Port = port;
			IsServer = true;
			LockConnections = false;
			MaxPlayers = -1;
		}

       static public void Start(int port, string mapPath)
		{
			Debug.Assert(!Running);
			if (Running)
				Instance.Stop();
			MenuPlay.Instance.SetServerBtn(true);
            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
            config.MaximumConnections = 100;
            config.Port = port;
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			Instance = new NetworkServer(new NetworkServerGame(mapPath), config, port);
			Instance.PrivateStart();
		}

        private void PrivateStart()
        {
			Running = false;
            try
            {
				Peer.Start();
                serverThread = new Thread(ServerMainLoop);
                serverThread.Start();
                OutputManager.LogMessage(serverThread.IsAlive.ToString());
                Running = true;
                OutputManager.LogNetwork("(Server) Launched");
            }
            catch
            {
                OutputManager.LogNetwork("Server - Fail to Launch server");
            }
        }

        public void Stop()
        {
            if (Running)
            {
                serverThread.Abort();
                Peer.Shutdown("Requested by user");
            }
			MenuPlay.Instance.SetServerBtn(false);
			Running = false;
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

							if (status == NetConnectionStatus.Connected
								&& !LockConnections
								&& (Peers.Count() < MaxPlayers || MaxPlayers < 0))
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
							else if (status == NetConnectionStatus.Disconnected)
							{
								if (IsSenderValid(msg))
									RemovePeer(GetPeer(msg));
							}
                            break;
                        case NetIncomingMessageType.Data:
							if (IsSenderValid(msg))
							{
								Server_ManageRequest(msg);
								toRecycle = false;
							}
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
				if (value == null)
					Game.Events = new byte[0];
				else
					Game.Events = value;
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
				{
					BruteSend(p, PacketManager.Robot(r));
				}
		}

		public override void RemovePeer(Peer p)
		{
			PushEvent(PacketManager.RemoveRobot(p.Id));
			Game.RemoveRobot(Game.RobotFromId(p.Id));
			p.Connect.Disconnect("cya");
			Peers.Remove(p);
		}
	}
}
