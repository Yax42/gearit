using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gearit.xna
{
    class MainMulti : MenuScreen, IDemoScreen
    {
        private VideoOption _video;
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

            AddMenuItem("Launch Server", EntryType.Separator, null);
            AddMenuItem("Join Server", EntryType.Separator, null);
            AddMenuItem("", EntryType.Separator, null);
            AddMenuItem("Retour", EntryType.Cancel, null);
        }
    }
}
