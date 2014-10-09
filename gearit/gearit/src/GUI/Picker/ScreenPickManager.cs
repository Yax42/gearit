using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using gearit.src.game;

namespace gearit.src.GUI.Picker
{
	class ScreenPickManager : GameScreen, IDemoScreen
	{
		static public string RobotPath { get; private set; }
		static public string MapPath { get; private set; }

		private DrawGame DrawGame;
		private const int PropertiesMenuSize = 40;
		private GameScreen NextScreen;
		private int State;

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

		public static void PickRobot()
		{
		}

		public static void PickMap()
		{
		}

		public ScreenPickManager(GameScreen nextScreen) : base(true)
		{
			NextScreen = nextScreen;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
		}


		public override void LoadContent()
		{
			base.LoadContent();
			DrawGame = new DrawGame(ScreenManager.GraphicsDevice);
			State = 0;
			MenuPickItem.Result = null;

			ScreenManager.Game.ResetElapsedTime();
			HasVirtualStick = true;
		}

		public override void Update(GameTime gameTime)
		{
			switch (State)
			{
				case 0:
					ScreenManager.AddScreen(ScreenPickMap.Instance);
					State++;
					break;
				case 1:
					if (MenuPickItem.Result != null)
					{
						MapPath = MenuPickItem.Result;
						MenuPickItem.Result = null;
						State++;
					}
					break;
				case 2:
					ScreenManager.ReplaceLast(ScreenPickRobot.Instance);
					State++;
					break;
				case 3:
					if (MenuPickItem.Result != null)
					{
						RobotPath = MenuPickItem.Result;
						State++;
					}
					break;
				case 4:
					ScreenManager.ResetTo(new GearitGame(RobotPath, MapPath));
					State++;
					break;
				case 5:
					Debug.Assert(false);
					break;
			}
			
			base.Update(gameTime);
		}


		public override void Draw(GameTime gameTime)
		{
			//ScreenManager.GraphicsDevice.Clear(Color.Red);
		}
	}
}
