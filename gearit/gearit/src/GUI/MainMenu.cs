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

namespace GUI
{
	public class MainMenu : Desktop
	{
		private ScreenManager _ScreenManager;
		
		// Properties
		static public int MENU_WIDTH = 200;

		// Gui
		private ListBox menu_listbox;
		private GameScreen current_screen = null;

		// List of item menu
		private MyGame _Gearit;
		private BruteRobot _bruteRobot;
		private GladiatoRobot _gladiator;
		private SpiderBot _spiderBot;
		private SoundManager _sound;
		private RobotEditor _robot_editor;
		private MapEditor _map_editor;
		private MainOptions _Options;
		private GearitGame _game;
		private MasterClient _masterClient;

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
			_robot_editor = new RobotEditor();
			_map_editor = new MapEditor();
			_game = new GearitGame();
			_Options = new MainOptions("Options", _ScreenManager);
			_masterClient = new MasterClient();

			// Add ItemMenu
			addMenuItem(_game, _game.GetTitle());
			addMenuItem(_robot_editor, _robot_editor.GetTitle());
			addMenuItem(_map_editor, _map_editor.GetTitle());
			addMenuItem(_Options, _Options.GetTitle());
			addMenuItem(_masterClient, _masterClient.GetTitle());

			//SEPARATOR
			addMenuItem(_Options, "");
			//

			addMenuItem(_Gearit, _Gearit.GetTitle());
			addMenuItem(_bruteRobot, _bruteRobot.GetTitle());
			addMenuItem(_spiderBot, _spiderBot.GetTitle());
			addMenuItem(_gladiator, _gladiator.GetTitle());

			// ToRemove - Popup robot editor
			menu_listbox.Items[1].Click(0);

			#endregion
		}

		public void goBack()
		{
			if (current_screen == null)
			{
				_ScreenManager.Exit();
				return;
			}
			
			_ScreenManager.RemoveScreen(current_screen);
			current_screen = null;
			menu_listbox.SelectedItem.Selected = false;

			Visible = true;
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
				if (current_screen == screen)
					return;

				if (current_screen != null)
					_ScreenManager.RemoveScreen(current_screen);

				_ScreenManager.AddScreen(screen);
				current_screen = screen;

				Visible = current_screen.VisibleMenu;
			};
		}
	}
}
