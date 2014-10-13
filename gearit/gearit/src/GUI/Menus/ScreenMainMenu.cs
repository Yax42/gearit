using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.GUI;
using Squid;
using gearit.src.game;
using gearit.src.Network;
using gearit.src.GeneticAlgorithm;
using gearit.src;
using gearit.src.gui;
using gearit.src.GUI.Picker;
using Microsoft.Xna.Framework.Graphics;
using gearit.src.editor.map;
using gearit.src.editor.robot;
using gearit.src.GUI.OptionsMenu;
using gearit.src.GUI.Tools;

namespace GUI
{
	class ScreenMainMenu : GameScreen
	{
		private static bool _GoBack = false;
		public static bool GoBack
		{
			get { return _GoBack; }
			set{_GoBack = value; }
		}

		// Properties
		static public int MENU_WIDTH = 300;
		static public int MENU_LIST_WIDTH = 200;
		static public int HEIGHT_TITLE = 35;

		// Gui
		private Color STRIPE_COLOR = Color.White;
		private Desktop _dk_main_menu;
		private Control main_menu;
		private Control list_container;
		private Label tb_title;
		private Desktop dk_listbox;
		private ListBox menu_listbox;

		// List of item menu
		private MyGame _Gearit;
		private GearitGame _game;
		private NetworkClientGame _networkGame;
		private MenuPlay _play;
		private MenuQuit _quit;
		private ScreenPickManager _soloGame;

		public ScreenMainMenu() : base(false)
		{
			DrawPriority = 999;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			VisibleMenu = true;
			_dk_main_menu = new Desktop();

			#region main

			_dk_main_menu.Position = new Squid.Point(0, 0);
			_dk_main_menu.Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);

			// MainMenu Container
			main_menu = new Control();
			main_menu.Position = new Squid.Point(0, HEIGHT_TITLE);
			main_menu.Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);
			main_menu.Style = "mainMenu";
			main_menu.Parent = _dk_main_menu;

			// MainMenu List Container
			list_container = new Control();
			list_container.Position = new Squid.Point(MENU_WIDTH - MENU_LIST_WIDTH, 0);
			list_container.Size = new Squid.Point(MENU_LIST_WIDTH, (int)(ScreenManager.Height / 1.5f));
			list_container.Style = "mainMenuListContainer";
			list_container.Parent = _dk_main_menu;

			// Private desktop
			dk_listbox = new Desktop();
			dk_listbox.Position = list_container.Position;
			dk_listbox.Size = list_container.Size;

			// MainMenu List
			menu_listbox = new ListBox();
			menu_listbox.Position = new Squid.Point(0, HEIGHT_TITLE * 3);
			menu_listbox.Size = new Squid.Point(MENU_LIST_WIDTH, list_container.Size.y - HEIGHT_TITLE * 2);
            menu_listbox.Scrollbar.Size = new Squid.Point(0, 0);
			menu_listbox.Style = "mainMenuList";
			menu_listbox.Parent = dk_listbox;

			// Title
			tb_title = new Label();
			tb_title.Position = new Squid.Point(0, HEIGHT_TITLE + 2);
			tb_title.Size = new Squid.Point(MENU_LIST_WIDTH, HEIGHT_TITLE);
			tb_title.Text = "GEARIT";
			tb_title.Style = "titleMainMenu";
			tb_title.Parent = dk_listbox;


			_Gearit = new MyGame();
			_game = new GearitGame();
			_networkGame = new NetworkClientGame();
			_play = new MenuPlay();
			_quit = new MenuQuit();
			_soloGame = new ScreenPickManager(_game);

			// Add ItemMenu
			addMenuItem(_play, _play.GetTitle().ToUpper());
			addMenuItem(RobotEditor.Instance, RobotEditor.Instance.GetTitle().ToUpper());
			addMenuItem(MapEditor.Instance, MapEditor.Instance.GetTitle().ToUpper());
			addMenuItem(OptionsMenu.Instance , OptionsMenu.Instance.GetTitle().ToUpper());
			addMenuSeparator();
			addMenuItem(_game, "DEFAULT GAME");
			addMenuItem(_networkGame, "NETWORK GAME");
			addMenuItem(_soloGame, "SOLO");

			addMenuSeparator();

			addMenuItem(_quit, _quit.GetTitle());

			// ToRemove - Popup robot editor
			//menu_listbox.Items[0].Click(0);

			#endregion
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_dk_main_menu.Update();
			dk_listbox.Update();

			if (GoBack)
			{
				GoBack = false;
				goBack();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_dk_main_menu.Draw();

			// Draw  stripe title
            ScreenManager.Instance.BasicEffect.CurrentTechnique.Passes[0].Apply(); // don't worry be happy
			drawStripes(8, HEIGHT_TITLE, (int) (ScreenMainMenu.MENU_WIDTH * 1.5f), HEIGHT_TITLE, STRIPE_COLOR);
			Color color;

			// Draw stripe listview
			for (int i = 0; i < menu_listbox.Items.Count; ++i)
			{
				ListBoxItem item = menu_listbox.Items[i];
				if (item.Text == "")
					continue;
				color = (item.State > ControlState.Default) ? Theme.CurrentTheme.Primitive: STRIPE_COLOR;
				drawStripes(item.Location.x + 8, item.Location.y + 6, item.Size.x - 8, 25, color);
			}

			dk_listbox.Draw();
		}

		public void goBack()
		{
			if (ScreenManager.IsEmpty())
			{
				ScreenManager.Exit();
				return;
			}
			ScreenManager.RemoveLast();
			menu_listbox.SelectedItem.Selected = false;
			//Visible = true;
		}

		public void addMenuSeparator()
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = "";
			item.BBCodeEnabled = true;
			item.Size = new Squid.Point(MENU_WIDTH, 24);
			menu_listbox.Items.Add(item);
		}

		public void addMenuItem(GameScreen screen, string title)
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = title;
			item.BBCodeEnabled = true;
			item.Size = new Squid.Point(MENU_WIDTH, 42);
			item.Style = "itemMainMenu";
			menu_listbox.Items.Add(item);

			// Callback
			item.MouseClick += delegate(Control snd, MouseEventArgs e)
			{
				ScreenManager.ResetTo(screen);
			};
		}

		private int BASE_STRIP_WIDTH = 7;
		private int BASE_STRIP_HEIGHT = 35;
		private int BASE_STRIP_SPACE = 35;
		private int drawStripes(int x, int y, int width, int height, Color color)
		{
			BASE_STRIP_HEIGHT = height;
			BASE_STRIP_SPACE = BASE_STRIP_HEIGHT;
			BASE_STRIP_WIDTH = (int) (BASE_STRIP_HEIGHT * 0.2f);
			return (drawStripes(x, y, (width - (BASE_STRIP_SPACE - BASE_STRIP_WIDTH))/ (BASE_STRIP_WIDTH * 2 + 2), color));
		}

		private int drawStripes(int x, int y, int number, Color color)
		{
            VertexPositionColor[] verts = new VertexPositionColor[number * 6];

			int pos = 0;
			for (int i = 0; i < number; ++i)
			{
				verts[pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				verts[pos + 5] = new VertexPositionColor(new Vector3(x, y, 0), color);
				x += BASE_STRIP_WIDTH;
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				y += BASE_STRIP_HEIGHT;
				x += BASE_STRIP_SPACE;
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				x -= BASE_STRIP_WIDTH;
				verts[++pos] = new VertexPositionColor(new Vector3(x, y, 0), color);
				y -= BASE_STRIP_HEIGHT;
				x = x - BASE_STRIP_SPACE + (int) (BASE_STRIP_WIDTH * 2 + 2);
				pos += 2;
			}

            ScreenManager.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verts, 0, number * 2);

			return (number);
		}
	}
}
