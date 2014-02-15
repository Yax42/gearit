using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace GUI
{
	class ScreenMainMenu : GameScreen
	{
		private Squid.Desktop _background;
		private MainMenu _menu;

		public ScreenMainMenu()
		{
			DrawPriority = 999;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			_menu = new GUI.MainMenu(ScreenManager);

			// Mouse Cursor bug fix
			//_background = new Squid.Desktop();
			//_background.ShowCursor = false;
			//_background.Size = new Squid.Point(ScreenManager.Height, ScreenManager.Height);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (Input.justPressed(Keys.Escape))
				_menu.goBack();

			_menu.Update();
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_menu.Draw();
		}
	}
}
