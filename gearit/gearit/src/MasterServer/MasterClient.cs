using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;

namespace gearit.src
{
	class MasterClient : GameScreen, IDemoScreen
	{
		//private DrawGame _draw_game;
		private const int PropertiesMenuSize = 40;

		//Action
		public static MasterClient Instance { set; get; }

		#region IDemoScreen Members

		public string GetTitle()
		{
			return "Multi-player online";
		}

		public string GetDetails()
		{
			return "";
		}

		#endregion

		public MasterClient()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			Instance = this;
		}


		public override void LoadContent()
		{
			base.LoadContent();
			new MenuMasterClient(ScreenManager);

			ScreenManager.Game.ResetElapsedTime();
			HasVirtualStick = true;

			//_draw_game = new DrawGame(ScreenManager.GraphicsDevice);
		}

		public override void Update(GameTime gameTime)
		{
			MenuMasterClient.Instance.Update();
			base.Update(gameTime);
		}


		public override void Draw(GameTime gameTime)
		{
			//ScreenManager.GraphicsDevice.Clear(Color.LightSeaGreen);
			//_draw_game.Begin(_camera);

			//_draw_game.End();
			base.Draw(gameTime);
			MenuMasterClient.Instance.Draw();
		}
	}
}
