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

namespace gearit.src.gui
{

	class MenuPlay : GameScreen, IDemoScreen
	{
        Desktop _desktop;

        const int TAB_WIDTH = 156;

        public class MyData
        {
            public string Name;
            public string Players;
            public int Ping;
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

            Random rnd = new Random();
            List<MyData> models = new List<MyData>();
            for (int i = 0; i < 8; i++)
            {
                MyData data = new MyData();
                data.Name = "Test server #" + i;
                data.Players = rnd.Next() % 10 + "/" + rnd.Next() % 20;
                data.Ping = rnd.Next() % 1000;
                models.Add(data);
            }

            ListView olv = new ListView();
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

            olv.SetObjects(models);



            TabPage tabPage = new TabPage();
            tabPage.Button.Size = new Squid.Point(TAB_WIDTH, 120);
            tabPage.Button.Text = "Local";
            tabcontrol.TabPages.Add(tabPage);

            tabcontrol.SelectedTab = tabInternet;


            VisibleMenu = true;
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
