using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Squid;
using gearit.xna;
using GUI;
using gearit.src.utility;
using System.Globalization;
using gearit.src.editor.map.action;
using gearit.src.GUI;
using gearit.src.editor.map;
using Microsoft.Xna.Framework;
using gearit.src.gui;
using gearit.src.GUI.Picker;
using gearit.src.game;
using gearit.src.Network;

namespace gearit.src.editor.map
{
    /// <summary>
    /// Menu Play for playing duh
    /// </summary>
	class MenuPlay : GameScreen, IDemoScreen
	{
		// Propertie
		static public int MENU_WIDTH = 220;
		static public int ITEM_HEIGHT = 30;
		static public int PADDING = 2;
		static public int PADDING_NODE = 24;
		static public int HEIGHT_NODE = 50;

		private Desktop dk_play = new Desktop();
		private Panel background = new Panel();

		private ScreenManager _ScreenManager;

		private GameScreen current_menu = null;

		public MenuPlay() : base(false)
		{
			dk_play.Position = new Squid.Point(0, 0);
			dk_play.Size = new Squid.Point(ScreenManager.Instance.Width, ScreenManager.Instance.Height);

			int y = 0;

			// Background
			background.Parent = dk_play;
			background.Style = "menu";
			background.Position = new Squid.Point(0, 0);

			Button btn = new Button();
			btn.Text = "ONLINE";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			background.Content.Controls.Add(btn);
			btn.Cursor = Cursors.Move;
			y += btn.Size.y + PADDING;
			btn.MouseClick += delegate(Control snd, MouseEventArgs evt)
			{
				changeSubmenu(new MenuMultiplayer());
			};

			btn = new Button();
			btn.Text = "LOCAL";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			background.Content.Controls.Add(btn);
			btn.Cursor = Cursors.Move;
			y += btn.Size.y + PADDING;

			btn.MouseClick += delegate(Control snd, MouseEventArgs evt)
			{
				changeSubmenu(new MenuLocal());
			};

			btn.Click(0);

			btn = new Button();
			btn.Text = "SOLO";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			background.Content.Controls.Add(btn);
			btn.Cursor = Cursors.Move;
			y += btn.Size.y;

			btn.MouseClick += delegate(Control snd, MouseEventArgs evt)
			{
				changeSubmenu(new MenuSolo());
			};

			y += btn.Size.y;
			btn = new Button();
			btn.Text = "RUN SERVER";
			btn.Style = "itemMenuButton";
			btn.Size = new Squid.Point(MENU_WIDTH, ITEM_HEIGHT);
			btn.Position = new Squid.Point(0, y);
			background.Content.Controls.Add(btn);
			btn.Cursor = Cursors.Move;
			y += btn.Size.y;

			btn.MouseClick += delegate(Control snd, MouseEventArgs evt)
			{
				changeSubmenu(new MenuRunServer());
			};


			background.Size = new Squid.Point(MENU_WIDTH, y);
		}

		public override void UnloadContent()
		{
			if (current_menu != null)
				ScreenManager.Instance.RemoveScreen(current_menu);
		}

		public void changeSubmenu(GameScreen submenu)
		{
			ScreenPickManager.Exit = true;
			if (current_menu != null)
				ScreenManager.Instance.RemoveScreen(current_menu);
			current_menu = submenu;
			ScreenManager.Instance.AddScreen(current_menu);
		}

		public override void positionChanged(int x, int y)
		{
			background.Position = new Squid.Point(x, y);
		}

		public override Squid.Point getMenuSize()
		{
			return (background.Size);
		}

		public override Squid.Point getMenuPosition()
		{
			return (background.Position);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
            dk_play.Update();
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
            dk_play.Draw();
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