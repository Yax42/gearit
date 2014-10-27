﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using gearit.src.game;
using gearit.src.utility;
using GUI;

namespace gearit.src.GUI.Picker
{
	class ScreenPickManager : GameScreen, IDemoScreen
	{
		static public string RobotPath { get; private set; }
		static public string MapPath { get; private set; }
		static public bool Exit = false;
		public bool IsPickRobot;
		public bool IsPickMap;

		private DrawGame DrawGame;
		private const int PropertiesMenuSize = 40;
		private int State;

		public Action Callback = null;

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

		public ScreenPickManager(bool isPickMap, bool isPickRobot, Action callback) : base(true)
		{
			IsPickMap = isPickMap;
			IsPickRobot = isPickRobot;
			Callback = callback;
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
			Exit = false;
			ScreenMainMenu.Instance.CatchExitLock = false;
		}

		public override void Update(GameTime gameTime)
		{
			switch (State)
			{
				case 0:
					if (IsPickMap)
					{
						ScreenManager.AddScreen(ScreenPickMap.Instance);
						State++;
					}
					else
						State = 2;
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
					if (IsPickMap)
						ScreenManager.RemoveScreen(ScreenPickMap.Instance);
					if (IsPickRobot)
					{
						ScreenManager.AddScreen(ScreenPickRobot.Instance);
						State++;
					}
					else
						State = 4;
					break;
				case 3:
					if (MenuPickItem.Result != null)
					{
						RobotPath = MenuPickItem.Result;
						State++;
					}
					break;
				case 4:
					if (IsPickRobot)
						ScreenManager.Instance.RemoveScreen(ScreenPickRobot.Instance);
					if (Callback != null)
						Callback();
					State++;
					break;
				case 5:
					ScreenManager.Instance.RemoveScreen(this);
					break;
			}

			if (Exit || Input.Exit)
			{
				ScreenManager.Instance.RemoveScreen(this);
				ScreenManager.Instance.RemoveScreen(ScreenPickRobot.Instance);
				ScreenManager.RemoveScreen(ScreenPickMap.Instance);
			}

			base.Update(gameTime);
		}

		public override void UnloadContent()
		{
			base.UnloadContent();

			ScreenMainMenu.Instance.CatchExitLock = true;
		}

		public override void Draw(GameTime gameTime)
		{
			//ScreenManager.GraphicsDevice.Clear(Color.Red);
		}
	}
}
