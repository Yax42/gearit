using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using gearit.src.utility.Menu;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using gearit.src.editor.robot;
using FarseerPhysics.Factories;
using gearit.src.map;
using gearit.src.editor.map.action;
using gearit.src.editor.map;
using FarseerPhysics.DebugViews;
using Squid;
using GUI;
using System.Net.Sockets;
using gearit.src.GUI;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using gearit.src.output;
using gearit.src.Network;
using System.Diagnostics;
using Lidgren.Network;
using gearit.src.GUI.Picker;

namespace gearit.src.gui
{
    /// <summary>
    /// List server / interaction with Game Master server / Connexion
    /// </summary>

	class MenuLocal : GameScreen, IDemoScreen
	{
        TextBox tb_login;
        TextBox tb_password;
        Panel list_server;
        Task taskMasterServer = null;
        Desktop _desktop;
        bool refreshing = false;
        Button btn_refresh = new Button();
        TcpClient client;

        const int DIALOG_WIDTH = 400;
        const int TAB_WIDTH = 156;
        ListView olv = new ListView();
        List<MyData> models = new List<MyData>();
        public class MyData
        {
            public string Name;
            public string Players;
            public int Ping;
            public int id;
            public float Time;
            public string Map;
            public string Host;
            public int port;
        }

		public MenuLocal() : base(false)
		{
		}

        public void th_getServers()
        {
            try
            {
				NetPeerConfiguration config = new NetPeerConfiguration("gearit");
				config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
				NetClient client = new NetClient(config);
				client.Start();
				Console.WriteLine(">>> " + NetUtility.GetBroadcastAddress().ToString());
				
				client.DiscoverLocalPeers(INetwork.SERVER_PORT);

				models.Clear();
				int id_host = 0;

				NetIncomingMessage inc;
				while (true)
				{
					if (!refreshing)
						return;

					while ((inc = client.ReadMessage()) != null)
					{
						switch (inc.MessageType)
						{
							case NetIncomingMessageType.DiscoveryResponse:
								string name = inc.ReadString();
								int nbpl = inc.ReadInt32();
								string map = inc.ReadString();
								float time = inc.ReadFloat();

								ScreenManager.beginDrawing();
								// Adding entry
								MyData entry = new MyData();
								entry.Name = name;
								entry.Players = nbpl.ToString();
								entry.Host = inc.SenderEndPoint.Address.ToString();
								entry.port = INetwork.SERVER_PORT;
								entry.id = id_host++;
								entry.Time = time;
								entry.Map = map;
								entry.Ping = (int)inc.ReceiveTime;
								models.Add(entry);
								olv.SetObjects(models);
								ScreenManager.stopDrawing();
								break;
						}
					}
				}
            }
            catch (SocketException e)
            {
                ChatBox.addEntry("Failing refreshing local network", ChatBox.Entry.Error);
            }

            refreshing = false;
            btn_refresh.Text = "Refresh";
        }

		public override void LoadContent()
		{
			base.LoadContent();

            VisibleMenu = true;

            _desktop = new Desktop();
            _desktop.Position = new Squid.Point(ScreenMainMenu.MENU_WIDTH, 0);
            _desktop.Size = new Squid.Point(ScreenManager.Width - ScreenMainMenu.MENU_WIDTH, ScreenManager.Height);

			btn_refresh.Parent = _desktop;
			btn_refresh.Position = new Squid.Point(0, 0);
			btn_refresh.Size = new Squid.Point(80, 30);
			btn_refresh.Text = "REFRESH";
			btn_refresh.Style = "itemMenuButton";
            btn_refresh.MouseClick += new MouseEvent(btn_MouseClick);

			olv.Position = new Squid.Point(0, 100);
			olv.Size = new Squid.Point(ScreenManager.Width - ScreenMainMenu.MENU_WIDTH, ScreenManager.Height);
            olv.Columns.Add(new ListView.Column { Text = "NAME", Aspect = "Name", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "IP", Aspect = "Host", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "PLAYERS", Aspect = "Players", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "MAP", Aspect = "Map", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "TIME", Aspect = "Time", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "PING", Aspect = "Ping", Width = 20, MinWidth = 48 });
            olv.Columns[0].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[1].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[2].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[3].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[4].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[5].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.FullRowSelect = true;
            olv.Parent = _desktop;
            olv.Style = "itemList";

            olv.CreateHeader = delegate(object sender, ListView.FormatHeaderEventArgs args)
            {
                Button header = new Button
                {
                    Dock = DockStyle.Fill,
                    Text = args.Column.Text,
                    AllowDrop = true
                };

                header.MouseClick += delegate(Control snd, MouseEventArgs e)
                {
                    if (args.Column.Aspect == "Name")
                        olv.Sort<MyData>((a, b) => a.Name.CompareTo(b.Name));
                    else if (args.Column.Aspect == "Players")
                        olv.Sort<MyData>((a, b) => a.Players.CompareTo(b.Players));
                    else if (args.Column.Aspect == "Ping")
                        olv.Sort<MyData>((a, b) => a.Ping.CompareTo(b.Ping));
                };

                return header;
            };

            olv.CreateCell = delegate(object sender, ListView.FormatCellEventArgs args)
            {
                string text = olv.GetAspectValue(args.Model, args.Column);

                Button cell =  new Button
                {
                    Size = new Squid.Point(26, 26),
                    Style = "itemList",
                    Dock = DockStyle.Top,
                    Text = text
                };

                cell.MouseDoubleClick += delegate(Control snd, MouseEventArgs e)
                {
					ScreenManager.Instance.AddScreen(new ScreenPickManager(this));
					ScreenPickManager.Callback += delegate()
					{
						ScreenManager.AddScreen(new NetworkClientGame(ScreenPickManager.MapPath, ScreenPickManager.RobotPath, ((MyData) args.Model).Host));
					};
                };

                return cell;
            };

			// Test
			MyData entry = new MyData();
			entry.Name = "Test server";
			entry.Players = "42";
			entry.Host = "127.0.0.1";
			entry.port = 4448;
			entry.id = 0;
			entry.Ping = 123;
			models.Add(entry);
			olv.SetObjects(models);
		}

        void btn_MouseClick(Control sender, MouseEventArgs args)
        {
            if (refreshing)
            {
                refreshing = false;
                taskMasterServer.Wait();
                btn_refresh.Text = "Refresh";
                return;
            }
            refreshing = !refreshing;
            btn_refresh.Text = refreshing ? "Cancel" : "Refresh";
            taskMasterServer = new Task(() =>
            {
                th_getServers();
            });
            taskMasterServer.Start();
        }

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
            _desktop.Update();
		}

		private void HandleInput()
		{
			
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
            _desktop.Draw();
		}

        public string GetTitle()
        {
            return "Play";
        }

        public string GetDetails()
        {
            return "Play menu";
        }
	}
}
