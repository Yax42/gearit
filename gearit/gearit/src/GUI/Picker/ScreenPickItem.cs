using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;

namespace gearit.src.GUI.Picker
{
	class ScreenPickItem : GameScreen, IDemoScreen
	{
		private DrawGame DrawGame;
		private const int PropertiesMenuSize = 40;
		private bool IsMap;

		//Action
		public static ScreenPickItem Instance { set; get; }

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

		public ScreenPickItem(bool isMap)
		{
			IsMap = isMap;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			Instance = this;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			new MenuPickItem(ScreenManager, IsMap);

			ScreenManager.Game.ResetElapsedTime();
			HasVirtualStick = true;

			DrawGame = new DrawGame(ScreenManager.GraphicsDevice);
		}

		public override void Update(GameTime gameTime)
		{
			MenuPickItem.Instance.Update();
			base.Update(gameTime);
		}


		public override void Draw(GameTime gameTime)
		{
			//ScreenManager.GraphicsDevice.Clear(Color.LightSeaGreen);
			//_draw_game.Begin(_camera);

			//_draw_game.End();
			base.Draw(gameTime);
			MenuPickItem.Instance.Draw();

			// Ces deux lignes sont etrangement necessaire, sinon la gui du MenuPickMap ne s'affiche pas
			DrawGame.Begin();
			DrawGame.End();
		}
	}
}

