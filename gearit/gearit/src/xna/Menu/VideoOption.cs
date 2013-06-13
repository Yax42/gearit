using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gearit.xna
{
    class VideoOption : PhysicsGameScreen, IDemoScreen
    {
        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Graphique";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Regler vos parametres graphique");
            return sb.ToString();
        }

        #endregion
        public void LoadMenu()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (ScreenManager.IsFullScreen == false)
                ScreenManager.activeFullScreen();
            else
                ScreenManager.deactivFullScreen();
            this.ExitScreen();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
