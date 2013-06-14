using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.utility;

namespace gearit.xna
{
    class MainMenu : GameScreen, IDemoScreen
    {
        private ScreenManager _screenManager;
        private MenuScreen _menuScreen;
        private MyGame _Gearit;
        private MainOptions _Options;
        private RobotEditor _RobotEditor;

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

            _Gearit = new MyGame();
            _RobotEditor = new RobotEditor();
            _Options = new MainOptions("Options", _screenManager);
            _Options.LoadMenu();

            _menuScreen = new MenuScreen("Gear it!");
            _menuScreen.AddMenuItem("Play Game", EntryType.Separator, null);
            _menuScreen.AddMenuItem(_Gearit.GetTitle(), EntryType.Screen, _Gearit);
            _menuScreen.AddMenuItem(_Options.GetTitle(), EntryType.Screen, _Options);
            _menuScreen.AddMenuItem(_RobotEditor.GetTitle(), EntryType.Screen, _RobotEditor);
            _menuScreen.AddMenuItem("_______", EntryType.Separator, null);
            _menuScreen.AddMenuItem("Quitter", EntryType.ExitItem, null);

            _screenManager.AddScreen(_menuScreen);

            //_screenManager.AddScreen(_RobotEditor);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (ScreenManager.IsFullScreen == false)
                ScreenManager.activeFullScreen();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
