using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;

namespace gearit.src.GUI.Picker
{
	abstract class AScreenPickItem : GameScreen, IDemoScreen
	{
		private DrawGame DrawGame;
		private const int PropertiesMenuSize = 40;
		private bool IsMap;
		private MenuPickItem Menu;

		//Action
		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Pick an item";
		}

		public string GetDetails()
		{
			return "";
		}

		#endregion

		public AScreenPickItem(bool isMap) : base(true)
		{
			DrawGame = DrawGame.Instance;
			DrawPriority = 9999;
			IsMap = isMap;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			Menu = new MenuPickItem(ScreenManager, IsMap);
			ScreenManager.Game.ResetElapsedTime();
			HasVirtualStick = true;
		}

		public override void Update(GameTime gameTime)
		{
			Menu.Update();
			base.Update(gameTime);
		}


		public override void Draw(GameTime gameTime)
		{
			//_draw_game.Begin(_camera);

			//_draw_game.End();
			base.Draw(gameTime);
			Menu.Draw();

			// Ces deux lignes sont etrangement necessaire, sinon la gui du MenuPickMap ne s'affiche pas
			DrawGame.Begin();
			DrawGame.End();
		}
	}
}

