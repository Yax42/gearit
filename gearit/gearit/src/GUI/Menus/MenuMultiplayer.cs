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

namespace gearit.src.gui
{
    /// <summary>
    /// List server / interaction with Game Master server / Connexion
    /// </summary>

	class MenuMultiplayer : GameScreen, IDemoScreen
	{
        TextBox tb_login;
        TextBox tb_password;
        Panel list_server;
        Task taskMasterServer = null;
        Desktop _desktop;
        bool refreshing = false;
        Button btn_refresh = new Button();
        TcpClient client;
        Label lb_error;
        Panel dialog_co;
        Button btn_co;

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
            public string host;
            public int port;
        }

		public MenuMultiplayer() : base(false)
		{
		}

        public void th_authenticate()
        {
            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 7979);

                // Authentification
                const int packet_size = 65;
                byte[] req = new byte[packet_size];

                req[0] = 4;
                string login = tb_login.Text;
                string password = tb_password.Text;
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
                    lb_error.Text = "Login/Password incorrect";
                    btn_co.Enabled = true;
                    client.GetStream().Dispose();
                    return;
                }
                //
                dialog_co.Parent = null;
                list_server.Parent = _desktop;
            }
            catch (SocketException e)
            {
                ChatBox.addEntry("Could not connect to master server", ChatBox.Entry.Error);
                lb_error.Text = "Could not connect to master server";
            }

            btn_co.Enabled = true;
        }

        public void th_getServers()
        {
            try
            {
                const int packet_size = 65;
                byte[] req = new byte[packet_size];
                int i;
                byte[] data = new byte[1024];

                // Add dummy servers - TO REMOVE
                Array.Clear(req, 0, req.Length);
                req[0] = 10;
                string name = "Test Server #1";
                for (i = 0; i < name.Length; ++i)
                    req[i + 1] = Convert.ToByte(name[i]);
                Int32[] ports = new Int32[1];
				ports[0] = INetwork.SERVER_PORT;
                req[33] = 1;
                req[34] = 0;
                Buffer.BlockCopy(ports, 0, req, 35, 4);
                req[39] = 6;
                client.GetStream().Write(req, 0, 40);
                //

                // Get the servers
                req[0] = 1;
                client.GetStream().Write(req, 0, 1);
                Int16[] versions = new Int16[1];
                byte max_player;
                string ip;
                models.Clear();
                char[] name_s = new char[33];
                int id_host = 0;
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

                    ScreenManager.beginDrawing();
                    // Adding entry
                    MyData entry = new MyData();
                    entry.Name = name + " - " + ip + ":" + ports[0];
                    entry.Players = "?/" + max_player;
                    entry.host = ip;
                    entry.port = ports[0];
                    entry.id = id_host++;
                    entry.Ping = 0;
                    models.Add(entry);
                    olv.SetObjects(models);
                    ScreenManager.stopDrawing();
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

            VisibleMenu = true;

            _desktop = new Desktop();
            _desktop.Position = new Squid.Point(ScreenMainMenu.MENU_WIDTH, 0);
            _desktop.Size = new Squid.Point(ScreenManager.Width - ScreenMainMenu.MENU_WIDTH, ScreenManager.Height);

            dialog_co = new Panel();
            dialog_co.Position = new Squid.Point(ScreenManager.Width / 2 - DIALOG_WIDTH, ScreenManager.Height / 4);
            dialog_co.Size = new Squid.Point((ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 2, DIALOG_WIDTH / 2 + 24);
            dialog_co.Parent = _desktop;
            dialog_co.Style = "menu";

            Label TitleLabel = new Label();
            TitleLabel.Size = new Squid.Point(100, 45);
            TitleLabel.Dock = DockStyle.Top;
            TitleLabel.Text = "CONNEXION";
            TitleLabel.Style = "itemMenuTitle";
            TitleLabel.TextAlign = Alignment.MiddleLeft;
            TitleLabel.Margin = new Margin(0, 0, 0, -1);
            dialog_co.Content.Controls.Add(TitleLabel);

            Frame ButtonFrame = new Frame();
            ButtonFrame.Size = new Squid.Point(100, 35);
            ButtonFrame.Dock = DockStyle.Bottom;
            dialog_co.Content.Controls.Add(ButtonFrame);

            tb_login = new TextBox();
            tb_login.Text = "test";
            tb_login.Size = new Squid.Point(158, 34);
            tb_login.Position = new Squid.Point(dialog_co.Size.x / 2 - 16, 70);
            dialog_co.Content.Controls.Add(tb_login);
            tb_login.Focus();

            Label lb = new Label();
            lb.Position = new Squid.Point(0, 70);
            lb.Size = new Squid.Point((int)((float)dialog_co.Size.x / 2.5), 34);
            lb.TextAlign = Alignment.MiddleRight;
            lb.Text = "Login";
            dialog_co.Content.Controls.Add(lb);

            lb = new Label();
            lb.Position = new Squid.Point(0, 120);
            lb.Size = new Squid.Point((int) ((float) dialog_co.Size.x / 2.5), 34);
            lb.TextAlign = Alignment.MiddleRight;
            lb.Text = "Password";
            dialog_co.Content.Controls.Add(lb);

            tb_password = new TextBox();
            tb_password.Text = "test";
            tb_password.Size = new Squid.Point(158, 34);
            tb_password.Position = new Squid.Point(dialog_co.Size.x / 2 - 16, 120);
            dialog_co.Content.Controls.Add(tb_password);

            btn_co = new Button();
            btn_co.Size = new Squid.Point(124, 34);
            btn_co.Position = new Squid.Point(dialog_co.Size.x - 124 - 8, dialog_co.Size.y - 34 - 8);
            btn_co.Text = "Connexion";
            dialog_co.Content.Controls.Add(btn_co);

            lb_error = new Label();
            lb_error.Size = new Squid.Point(dialog_co.Size.x / 2, 34);
            lb_error.Position = new Squid.Point(0, dialog_co.Size.y - 34 - 8);
            lb_error.TextAlign = Alignment.MiddleCenter;
            lb_error.TextColor = ColorInt.RGBA(.8f, .0f, .0f, 1);
            dialog_co.Content.Controls.Add(lb_error);

            btn_co.MouseClick += delegate(Control snd, MouseEventArgs e)
            {
                btn_co.Enabled = false;
                taskMasterServer = new Task(() =>
                {
                    th_authenticate();
                });
                taskMasterServer.Start();
            };


            list_server = new Panel();
            list_server.Position = new Squid.Point(0, 0);
            list_server.Parent = null;
            list_server.Dock = DockStyle.Fill;
            TabControl tabcontrol = new TabControl();
            tabcontrol.ButtonFrame.Style = "item";
            tabcontrol.Dock = DockStyle.Fill;
            list_server.Content.Controls.Add(tabcontrol);


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
            olv.Columns[0].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 3;
            olv.Columns[1].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 3;
            olv.Columns[2].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 3;
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

            olv.CreateCell = delegate(object sender, ListView.FormatCellEventArgs args)
            {
                string text = olv.GetAspectValue(args.Model, args.Column);

                Button cell =  new Button
                {
                    Size = new Squid.Point(26, 26),
                    Style = "label",
                    Dock = DockStyle.Top,
                    Text = text
                };

                cell.MouseDoubleClick += delegate(Control snd, MouseEventArgs e)
                {
					Debug.Assert(false);
                    //NetworkClient.Connect(((MyData)args.Model).host, ((MyData)args.Model).port, null);
                };

                return cell;
            };

            TabPage tabPage = new TabPage();
            tabPage.Button.Size = new Squid.Point(TAB_WIDTH, 120);
            tabPage.Button.Text = "Local";
            tabcontrol.TabPages.Add(tabPage);

            tabcontrol.SelectedTab = tabInternet;

			btn_refresh.Size = new Squid.Point(100, 26);
            btn_refresh.Position = new Squid.Point(TAB_WIDTH * 2 + 24, 4);
            btn_refresh.Text = "Refresh";
            btn_refresh.Style = "addEventButton";
            btn_refresh.MouseClick += new MouseEvent(btn_MouseClick);
            list_server.Content.Controls.Add(btn_refresh);

            Button btn_disconnect = new Button();
            btn_disconnect.Size = new Squid.Point(100, 26);
            btn_disconnect.Position = new Squid.Point(TAB_WIDTH * 2 + 24 + 126, 4);
            btn_disconnect.Text = "Disconnect";
            btn_disconnect.MouseClick += delegate(Control snd, MouseEventArgs e)
            {
                client.GetStream().Dispose();
                list_server.Parent = null;
                dialog_co.Parent = _desktop;
            };
            list_server.Content.Controls.Add(btn_disconnect);
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
