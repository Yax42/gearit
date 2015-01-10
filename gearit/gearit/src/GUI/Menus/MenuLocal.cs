﻿using System;
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
        static Task taskMasterServer = null;
        Desktop _desktop;
        static bool refreshing = false;
        Button btn_refresh = new Button();
        static TcpClient client;
		ScreenPickManager picker;

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
				//Console.WriteLine(">>> " + NetUtility.GetBroadcastAddress().ToString());
				
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
            _desktop.Position = new Squid.Point(ScreenMainMenu.MENU_WIDTH + MenuPlay.MENU_WIDTH + 1, 0);
            _desktop.Size = new Squid.Point(ScreenManager.Width - ScreenMainMenu.MENU_WIDTH  - MenuPlay.MENU_WIDTH - 1, ScreenManager.Height);

			olv.Position = new Squid.Point(0, 100);
			olv.Dock = DockStyle.Fill;
            olv.Columns.Add(new ListView.Column { Text = "", Aspect = "", Width = 0, MinWidth = 4 });
            olv.Columns.Add(new ListView.Column { Text = "NAME", Aspect = "Name", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "IP", Aspect = "Host", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "PLAYERS", Aspect = "Players", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "MAP", Aspect = "Map", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "TIME", Aspect = "Time", Width = 20, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "PING", Aspect = "Ping", Width = 20, MinWidth = 48 });
            olv.Columns[0].Width = (4);
            olv.Columns[1].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 8;
            olv.Columns[2].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 8;
            olv.Columns[3].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 8;
            olv.Columns[4].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 8;
            olv.Columns[5].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 8;
            olv.Columns[6].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 8;
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


			btn_refresh.Parent = _desktop;
			btn_refresh.Position = new Squid.Point(0, _desktop.Size.y - 30);
			btn_refresh.Size = new Squid.Point(80, 30);
			btn_refresh.Text = "REFRESH";
			btn_refresh.Style = "button";
            btn_refresh.MouseClick += new MouseEvent(btn_MouseClick);
			btn_refresh.Visible = false;


            Label lb = new Label();
            lb.TextAlign = Alignment.MiddleCenter;
            lb.Text = "Login";
			lb.Style = "itemMenuSubtitle";
			lb.Parent = _desktop;
            lb.Position = new Squid.Point(0, _desktop.Size.y - 30);
            lb.Size = new Squid.Point(80, 30);

            TextBox tb_login = new TextBox();
            tb_login.Text = "test";
            tb_login.Size = new Squid.Point(200, 30);
			tb_login.Position = new Squid.Point(80, _desktop.Size.y - 30);
			tb_login.Parent = _desktop;
            //dialog_co.Content.Controls.Add(tb_login);
            tb_login.Focus();
			tb_login.Style = "menuTextbox";
			tb_login.TextChanged += delegate(Control snd)
			{
				GI_Data.Pseudo = ((TextBox)snd).Text;
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
					endThread();
					picker = new ScreenPickManager(false, true,
					delegate()
					{
						ScreenManager.AddScreen(new NetworkClientGame(ScreenPickManager.RobotPath, ((MyData) args.Model).Host));
						picker = null;
					});
					ScreenManager.Instance.AddScreen(picker);
                };

                return cell;
            };

			olv.SetObjects(models);
			//btn_refresh.Click(0);
			btn_MouseClick(null, null);
		}

		public override void UnloadContent()
		{
			base.UnloadContent();
		}

		void endThread()
		{
			if (refreshing)
            {
				Console.WriteLine("exited");
				refreshing = false;
				taskMasterServer.Wait();
			}
		}

        void btn_MouseClick(Control sender, MouseEventArgs args)
        {
			endThread();
            refreshing = !refreshing;
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
