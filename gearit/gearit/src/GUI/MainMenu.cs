﻿using System;
using System.Collections.Generic;
using System.Text;
using Squid;
using gearit.xna;
using gearit.src.utility;
using gearit.src.editor.robot;
using gearit.src.game;
using gearit.src.editor.map;
using SquidXNA;

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
        private MenuScreen _menuScreen;
        private MyGame _Gearit;
        private BruteRobot _bruteRobot;
        private GladiatoRobot _gladiator;
        private SpiderBot _spiderBot;
        private RobotEditor _robot_editor;
        private MapEditor _map_editor;
        private MainOptions _Options;
        private SoundManager _sound;
        private GearitGame _game2;

        public MainMenu(ScreenManager ScreenManager)
        {
            ShowCursor = true;

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
            _game2 = new GearitGame();
            _Options = new MainOptions("Options", _ScreenManager);

            // Add ItemMenu
            addMenuItem(_Gearit, _Gearit.GetTitle());
            addMenuItem(_bruteRobot, _bruteRobot.GetTitle());
            addMenuItem(_spiderBot, _spiderBot.GetTitle());
            addMenuItem(_gladiator, _gladiator.GetTitle());
            addMenuItem(_robot_editor, _robot_editor.GetTitle());
            addMenuItem(_map_editor, _map_editor.GetTitle());
            addMenuItem(_game2, _game2.GetTitle());
            addMenuItem(_Options, _Options.GetTitle());

            menu_listbox.Items[4].Click(0);
            ScreenManager.AddScreen(_robot_editor);
            current_screen = _robot_editor;

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
            item.MousePress += delegate(Control snd, MouseEventArgs e)
            {
                if (current_screen == screen)
                    return;

                if (current_screen != null)
                    _ScreenManager.RemoveScreen(current_screen);

                _ScreenManager.AddScreen(screen);
                current_screen = screen;
            };
        }
    }
}