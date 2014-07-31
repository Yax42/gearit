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

namespace gearit.src.gui
{

    class MenuPlay : GameScreen, IDemoScreen
	{
        Task taskMasterServer = null;
        Desktop _desktop;
        bool refreshing = false;
        Button btn_refresh = new Button();

        const int TAB_WIDTH = 156;
        ListView olv = new ListView();
        List<MyData> models = new List<MyData>();
        public class MyData
        {
            public string Name;
            public string Players;
            public int Ping;
        }

        public void th_getServers()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", 7979);

                // Authentification
                const int packet_size = 65;
                byte[] req = new byte[packet_size];

                req[0] = 4;
                string login = "test";
                string password = "test";
                int i;
                for (i = 0; i < login.Length; ++i)
                    req[i + 1] = Convert.ToByte(login[i]);
                req[i + 1] = 0;
                for (i = 0; i < password.Length; ++i)
                    req[i + 33] = Convert.ToByte(password[i]);
                req[i + 33] = 0;

                client.GetStream().Write(req, 0, packet_size);

                // Get response
                byte[] data = new byte[1024];
                client.GetStream().Read(data, 0, 1);
                if (data[0] != 5)
                {
                    ChatBox.addEntry("Wrong identification to master server", ChatBox.Entry.Info);
                    return;
                }
                //

                // Add dummy servers - TO REMOVE
                Array.Clear(req, 0, req.Length);
                req[0] = 10;
                string name = "Test Server #1";
                for (i = 0; i < name.Length; ++i)
                    req[i + 1] = Convert.ToByte(name[i]);
                Int32[] ports = new Int32[1];
                ports[0] = 9555;
                req[33] = 1;
                req[34] = 0;
                Buffer.BlockCopy(ports, 0, req, 35, 4);
                req[39] = 6;
                client.GetStream().Write(req, 0, 40);

                // Get the servers
                req[0] = 1;
                client.GetStream().Write(req, 0, 1);
                Int16[] versions = new Int16[1];
                byte max_player;
                string ip;
                models.Clear();
                char[] name_s = new char[33];
                while (client.Connected)
                {
                    if (!refreshing)
                        return;
                    int receivedDataLength = client.GetStream().Read(data, 0, 1024);
                    Console.WriteLine(">> " + receivedDataLength);
                    if (receivedDataLength < 12)
                       break;
                    for (i = 0; i < 32; ++i)
                       name_s[i] = (char) data[i + 1];
                    ip = data[35] + "." + data[36] + "." + data[37] + "." + data[38];
                    Buffer.BlockCopy(data, 39, ports, 0, 4);
                    Buffer.BlockCopy(data, 34, versions, 0, 2);
                    max_player = data[44];

                    // Adding entry
                    MyData entry = new MyData();
                    entry.Name = name + " - " + ip + ":" + ports[0];
                    entry.Players = "?/" + max_player;
                    entry.Ping = 0;
                    models.Add(entry);
                    olv.SetObjects(models);

                    // Shutdown
                    client.GetStream().Dispose();
                }

            }
            catch (SocketException e)
            {
                ChatBox.addEntry("Could not connect to master server", ChatBox.Entry.Error);
            }

            refreshing = false;
            btn_refresh.Text = "Refresh";
        }

		public override void LoadContent()
		{
			base.LoadContent();
            _desktop = new Desktop();
            _desktop.Position = new Squid.Point(MainMenu.MENU_WIDTH, 0);
            _desktop.Size = new Squid.Point(ScreenManager.Width - MainMenu.MENU_WIDTH, ScreenManager.Height);


            TabControl tabcontrol = new TabControl();
            tabcontrol.ButtonFrame.Style = "item";
            tabcontrol.Dock = DockStyle.Fill;
            tabcontrol.Parent = _desktop;


            TabPage tabInternet = new TabPage();
            tabInternet.Margin = new Margin(0, -1, 0, 0);
            tabInternet.Button.Style = "button";
            tabInternet.Button.Text = "Internet";
            tabInternet.Button.Margin = new Margin(0, 0, -1, 0);
            tabInternet.Button.Size = new Squid.Point(TAB_WIDTH, 120);
            tabcontrol.TabPages.Add(tabInternet);

            olv.Dock = DockStyle.Fill;
            olv.Columns.Add(new ListView.Column { Text = "Name", Aspect = "Name", Width = 120, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "Players", Aspect = "Players", Width = 120, MinWidth = 48 });
            olv.Columns.Add(new ListView.Column { Text = "Ping", Aspect = "Ping", Width = 120, MinWidth = 48 });
            olv.Columns[0].Width = (ScreenManager.Width - MainMenu.MENU_WIDTH) / 3;
            olv.Columns[1].Width = (ScreenManager.Width - MainMenu.MENU_WIDTH) / 3;
            olv.Columns[2].Width = (ScreenManager.Width - MainMenu.MENU_WIDTH) / 3;
            olv.FullRowSelect = true;
            olv.Parent = tabInternet;
            olv.Style = "button";

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

            TabPage tabPage = new TabPage();
            tabPage.Button.Size = new Squid.Point(TAB_WIDTH, 120);
            tabPage.Button.Text = "Local";
            tabcontrol.TabPages.Add(tabPage);

            tabcontrol.SelectedTab = tabInternet;

            btn_refresh.Parent = _desktop;
            btn_refresh.Size = new Squid.Point(100, 26);
            btn_refresh.Position = new Squid.Point(TAB_WIDTH * 2 + 24, 4);
            btn_refresh.Text = "Refresh";
            btn_refresh.Style = "addEventButton";
            btn_refresh.MouseClick += new MouseEvent(btn_MouseClick);

            VisibleMenu = true;
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
