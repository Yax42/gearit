﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.GUI;

namespace GUI
{
	class ScreenMainMenu : GameScreen
	{
		private static bool _GoBack = false;
		public static bool GoBack
		{
			get
			{
				return _GoBack;
			}
			set
			{
				_GoBack = value;
			}
		}
		private Squid.Desktop _background;
		private MainMenu _menu;


		public ScreenMainMenu() : base(false)
		{
			DrawPriority = 999;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			_menu = new GUI.MainMenu(ScreenManager);
			VisibleMenu = true;

			// Mouse Cursor bug fix
			//_background = new Squid.Desktop();
			//_background.ShowCursor = false;
			//_background.Size = new Squid.Point(ScreenManager.Height, ScreenManager.Height);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (GoBack)
			{
				GoBack = false;
				_menu.goBack();
			}

			_menu.Update();
			_menu.Visible = ScreenManager.MenuVisible;
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_menu.Draw();
		}
	}
}