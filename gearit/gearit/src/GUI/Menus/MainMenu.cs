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

namespace GUI
{
    /// <summary>
    /// Navigation menu to access every part of the game
    /// </summary>
	public class MainMenu : Desktop
	{
		private ScreenManager _ScreenManager;
		
		// Properties
		static public int MENU_WIDTH = 200;

		// Gui
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
		private MasterClient _masterClient;
		private MenuPlay _play;
		private MenuQuit _quit;
		private ScreenPickManager _soloGame;

		public MainMenu(ScreenManager ScreenManager)
		{
			_ScreenManager = ScreenManager;

			#region main

			Position = new Squid.Point(0, 0);
			Size = new Squid.Point(MENU_WIDTH, ScreenManager.Height);

			// MainMenu
			menu_listbox = new ListBox();
			menu_listbox.Position = new Point(0, 0);
			menu_listbox.Size = new Point(MENU_WIDTH, _ScreenManager.Height);
			menu_listbox.Scrollbar.Size = new Squid.Point(14, 10);
			menu_listbox.Scrollbar.Slider.Style = "vscrollTrack";
			menu_listbox.Scrollbar.Slider.Button.Style = "vscrollButton";
			menu_listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
			menu_listbox.Scrollbar.ButtonUp.Size = new Squid.Point(10, 20);
			menu_listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
			menu_listbox.Scrollbar.ButtonDown.Size = new Squid.Point(10, 20);
			menu_listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
			menu_listbox.Multiselect = false;
			menu_listbox.Parent = this;
			menu_listbox.Style = "mainMenu";

			_Gearit = new MyGame();
			_bruteRobot = new BruteRobot();
			_spiderBot = new SpiderBot();
			_gladiator = new GladiatoRobot();
			_geneticAlorithm = new LifeManager();
			_game = new GearitGame();
			_networkGame = new NetworkClientGame();
			_masterClient = new MasterClient();
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
			addMenuItem(_masterClient, _masterClient.GetTitle());
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
	}
}
