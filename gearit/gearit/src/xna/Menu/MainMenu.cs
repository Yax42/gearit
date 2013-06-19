﻿using System.Text;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using gearit.src.editor.robot;

namespace gearit.xna
{
    class MainMenu : GameScreen, IDemoScreen
    {
        private ScreenManager _screenManager;
        private MenuScreen _menuScreen;
        private MyGame _Gearit;
        private BruteRobot _bruteRobot;
        private GladiatoRobot _gladiator;
        private SpiderBot _spiderBot;
        private RobotEditor _robot_editor;
        private MainOptions _Options;
        private SoundManager _sound;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Gear it!";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            return sb.ToString();
        }

        #endregion

        public MainMenu(ScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _sound = new SoundManager(_screenManager, "Audio/MenuSound");
            _sound.ActiveLoop();
            //_sound.playSound();

            _Gearit = new MyGame();
            _bruteRobot = new BruteRobot();
            _spiderBot = new SpiderBot();
            _gladiator = new GladiatoRobot();
            _robot_editor = new RobotEditor();
            _Options = new MainOptions("Options", _screenManager);
            _Options.LoadMenu();

            _menuScreen = new MenuScreen("Gear it!");
            //_menuScreen.AddMenuItem("Play Game", EntryType.Separator, null);
            _menuScreen.AddMenuItem(_robot_editor.GetTitle(), EntryType.Screen, _robot_editor);
            _menuScreen.AddMenuItem(_Gearit.GetTitle(), EntryType.Screen, _Gearit);
            _menuScreen.AddMenuItem(_bruteRobot.GetTitle(), EntryType.Screen, _bruteRobot);
            _menuScreen.AddMenuItem(_spiderBot.GetTitle(), EntryType.Screen, _spiderBot);
            _menuScreen.AddMenuItem(_gladiator.GetTitle(), EntryType.Screen, _gladiator);
            _menuScreen.AddMenuItem("\n", EntryType.Separator, null);
            _menuScreen.AddMenuItem(_Options.GetTitle(), EntryType.Screen, _Options);
            _menuScreen.AddMenuItem("Quitter", EntryType.ExitItem, null);

            _screenManager.AddScreen(_menuScreen);
            _screenManager.AddScreen(_robot_editor);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            /*if (ScreenManager.IsFullScreen == false)
                ScreenManager.activeFullScreen();*/
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
