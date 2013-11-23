using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using gearit.src.game;

namespace gearit.xna
{
    class MainMulti : MenuScreen, IDemoScreen
    {
        private VideoOption _video;
        private GearitNetworkGame _game;
        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Multiplayer";
        }

        public string GetDetails()
        {
            return (string.Empty);
        }

        #endregion

        public MainMulti(string menuTitle, ScreenManager manager)
            : base(menuTitle)
        {
            ScreenManager = manager;
        }

        public void LoadMenu()
        {
            base.LoadContent();
            _game = new GearitNetworkGame();

            AddMenuItem("Launch Server", EntryType.Screen, _game);
            AddMenuItem("Join Server", EntryType.Separator, null);
            AddMenuItem("", EntryType.Separator, null);
            AddMenuItem("Retour", EntryType.Cancel, null);
        }
    }
}
