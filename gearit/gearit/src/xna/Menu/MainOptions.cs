using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gearit.xna
{
    class MainOptions : MenuScreen, IDemoScreen
    {
        private VideoOption _video;
        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Options";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Changer les parametres du jeu.");
            return sb.ToString();
        }

        #endregion

        public MainOptions(string menuTitle, ScreenManager manager)
            : base(menuTitle)
    {
        ScreenManager = manager;
    }
        public void LoadMenu()
        {
            base.LoadContent();

            _video = new VideoOption();
            AddMenuItem("Graphique", EntryType.Screen, _video);
            AddMenuItem("Audio", EntryType.Separator, null);
            AddMenuItem("Graphique", EntryType.Separator, null);
            AddMenuItem("", EntryType.Separator, null);
            AddMenuItem("Retour", EntryType.Cancel, null);
        }
    }
}
