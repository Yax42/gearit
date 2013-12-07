using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using Squid;

namespace GUI
{
    class ScreenMainMenu : GameScreen
    {
        private MainMenu _menu;

        public ScreenMainMenu()
        {
            DrawPriority = 999;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _menu = new GUI.MainMenu(ScreenManager);
        }

        public override void Update(GameTime gameTime)
        {
            _menu.Update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _menu.Draw();

            base.Draw(gameTime);
        }
    }
}
