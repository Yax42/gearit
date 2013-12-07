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
    class FullScreen : GameScreen, IDemoScreen
    {
        #region IDemoScreen Members

        public string GetTitle()
        {
            return "FullScreen";
        }

        public string GetDetails()
        {
            return (string.Empty);
        }

        #endregion

        public void LoadMenu()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (ScreenManager.IsFullScreen == false)
                ScreenManager.activeFullScreen();
            else
                ScreenManager.deactivFullScreen();
            this.ExitScreen();
            base.Update(gameTime);
        }
    }
}
