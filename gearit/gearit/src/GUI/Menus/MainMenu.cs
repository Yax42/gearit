using System;
using System.Collections.Generic;
using System.Text;
using Squid;
using gearit.xna;
using gearit.src.utility;
using gearit.src.editor.robot;
using gearit.src.game;
using gearit.src.editor.map;
using SquidXNA;
using gearit.src;
using gearit.src.GUI;
using gearit.src.gui;
using gearit.src.GeneticAlgorithm;
using gearit.src.Network;
using gearit.src.GUI.OptionsMenu;
using gearit.src.GUI.Picker;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GUI
{
    /// <summary>
    /// Navigation menu to access every part of the game
    /// </summary>
	public class MainMenu : Desktop
	{
		private ScreenManager _ScreenManager;
		
		// Properties
		static public int MENU_WIDTH = 300;
		static public int MENU_LIST_WIDTH = 200;
		static public int HEIGHT_TITLE = 35;

		// Gui
		private Panel main_menu;
		private Panel list_container;
		private TextBox tb_title;
		private Desktop dk_listbox;
		private ListBox menu_listbox;

		// List of item menu
		private MyGame _Gearit;
		private BruteRobot _bruteRobot;
		private GladiatoRobot _gladiator;

		private SpiderBot _spiderBot;
		private SoundManager _sound;
		private GearitGame _game;
		private NetworkClientGame _networkGame;
		private LifeManager _geneticAlorithm;
		private MenuPlay _play;
		private MenuQuit _quit;
		private ScreenPickManager _soloGame;

		public MainMenu(ScreenManager ScreenManager)
		{
			_ScreenManager = ScreenManager;

			#region main

			Position = new Squid.Point(0, 0);
			Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);

			// MainMenu Container
			main_menu = new Panel();
			main_menu.Position = new Squid.Point(0, 0);
			main_menu.Size = new Squid.Point(MENU_WIDTH, _ScreenManager.Height);
			main_menu.Style = "mainMenu";
			main_menu.Parent = this;

			// MainMenu List Container
			list_container = new Panel();
			list_container.Position = new Squid.Point(MENU_WIDTH - MENU_LIST_WIDTH, 0);
			list_container.Size = new Squid.Point(MENU_LIST_WIDTH, (int)(_ScreenManager.Height / 1.5f));
			list_container.Style = "mainMenuListContainer";
			main_menu.Content.Controls.Add(list_container);

			// Private desktop
			dk_listbox = new Desktop();
			dk_listbox.Position = list_container.Position;
			dk_listbox.Size = list_container.Size;

			// MainMenu List
			menu_listbox = new ListBox();
			menu_listbox.Position = new Squid.Point(0, HEIGHT_TITLE * 2);
			menu_listbox.Size = new Squid.Point(MENU_LIST_WIDTH, list_container.Size.y - HEIGHT_TITLE * 2);
			menu_listbox.Style = "mainMenuList";
			menu_listbox.Parent = dk_listbox;

			// Title
			tb_title = new TextBox();
			tb_title.Position = new Squid.Point(0, 0);
			tb_title.Size = new Squid.Point(MENU_LIST_WIDTH, HEIGHT_TITLE);
			tb_title.Style = "titleMainMenu";
			tb_title.Text = "GEARIT";
			tb_title.Parent = dk_listbox;


			_Gearit = new MyGame();
			_bruteRobot = new BruteRobot();
			_spiderBot = new SpiderBot();
			_gladiator = new GladiatoRobot();
			_geneticAlorithm = new LifeManager();
			_game = new GearitGame();
			_networkGame = new NetworkClientGame();
			_play = new MenuPlay();
			_quit = new MenuQuit();
			_soloGame = new ScreenPickManager(_game);

			#region StaticInstance
			new MapEditor();
			new RobotEditor();
			new OptionsMenu(_ScreenManager);
			new ScreenPickRobot();
			new ScreenPickMap();
			#endregion

			// Add ItemMenu
			addMenuItem(_play, _play.GetTitle());
			addMenuItem(RobotEditor.Instance, RobotEditor.Instance.GetTitle());
			addMenuItem(MapEditor.Instance, MapEditor.Instance.GetTitle());
			addMenuItem(OptionsMenu.Instance , OptionsMenu.Instance.GetTitle());
			addMenuItem(_quit, _quit.GetTitle());

			//SEPARATOR
			addMenuSeparator();
			addMenuItem(_geneticAlorithm, _geneticAlorithm.GetTitle());
			addMenuSeparator();
			//

			addMenuItem(_game, "Default Game");
			addMenuItem(_networkGame, "Network Game");
			addMenuItem(_bruteRobot, "Brute Game");
			addMenuItem(_soloGame, "Solo");

			// ToRemove - Popup robot editor
			//menu_listbox.Items[0].Click(0);

			#endregion
		}

		public void goBack()
		{
			if (_ScreenManager.IsEmpty())
			{
				_ScreenManager.Exit();
				return;
			}
			_ScreenManager.RemoveLast();
			menu_listbox.SelectedItem.Selected = false;
			//Visible = true;
		}

		public void addMenuSeparator()
		{
			ListBoxItem item = new ListBoxItem();
			item.Text = "";
			item.BBCodeEnabled = true;
			item.Size = new Squid.Point(MENU_WIDTH, 42);
			item.Style = "itemMainMenu";
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
				_ScreenManager.ResetTo(screen);
			};
		}

		protected override void DrawCustom()
		{
			base.DrawCustom();

			// Draw  stripe
            ScreenManager.Instance.BasicEffect.CurrentTechnique.Passes[0].Apply(); // don't worry be happy
			drawStripes(8, 0, (int) (MainMenu.MENU_WIDTH * 1.5f), HEIGHT_TITLE, Color.White);
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
