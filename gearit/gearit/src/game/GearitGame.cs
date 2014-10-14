﻿using System;
using System.Collections.Generic;
using gearit.xna;
using gearit.src.utility;
using FarseerPhysics.Dynamics;
using gearit.src.map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using gearit.src.editor;
using System.Diagnostics;
using GUI;
using FarseerPhysics.DebugViews;
using gearit.src.robot;
using gearit.src.script;
using gearit.src.editor.robot;
using System.IO;

namespace gearit.src.game
{
	class GearitGame : GameScreen, IDemoScreen, IGearitGame
	{
		private World _world;
		private Camera2D _camera;
		private GameLuaScript _gameMaster;
		private bool _exiting;
		private bool _pause;

		private Map _Map;
		public Map Map { get { return _Map; } }

		private string RobotPath;
		private string MapPath;
		private List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }

		private DrawGame _drawGame;

		// Graphic
		private RectangleOverlay _background;

		private DebugViewXNA _debug;

		// Action
		private int _FrameCount = 0;
		public int FrameCount { get { return _FrameCount; } }

		private float _Time = 0;
		public float Time { get { return _Time; } }

		#region IDemoScreen Members

		public GearitGame() : this("data/robot/default.gir", "data/map/default.gim")
		{
		}

		public GearitGame(string robotPath, string mapPath) : base(true)
		{
			RobotPath = robotPath;
			MapPath = mapPath;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
		}

		public void Message(string msg, int duration)
		{
			ScreenManager.Message(msg, duration);
		}

		public string GetTitle()
		{
			return "GearIt";
		}

		public string GetDetails()
		{
			return ("");
		}

		#endregion

		public override void LoadContent()
		{
			_Robots = new List<Robot>();
			_world = new World(new Vector2(0, 9.8f));
			base.LoadContent();
			_exiting = false;
			_pause = false;

			_FrameCount = 0;
			_Time = 0;
			_drawGame = new DrawGame(ScreenManager.GraphicsDevice);
			_camera = new Camera2D(ScreenManager.GraphicsDevice);
			_world.Clear();
			_world.Gravity = new Vector2(0f, 9.8f);

			//clearRobot();
			SerializerHelper.World = _world;

			Robot robot = (Robot)Serializer.DeserializeItem(RobotPath);
			addRobot(robot);
			Debug.Assert(Robots != null);
			_Map = (Map)Serializer.DeserializeItem(MapPath);
			Debug.Assert(Map != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			_gameMaster = new GameLuaScript(this, _Map.LuaFullPath);

			// I have no idea what this is.
			//HasVirtualStick = true;
		}

		public World getWorld()
		{
			return (_world);
		}

		public void setMap(Map map)
		{
			_Map = map;
		}

		public void clearRobot()
		{
			foreach (Robot r in Robots)
				r.ExtractFromWorld();
			Robots.Clear();
		}

		public int MainRobotId { get { return 0; } }

		public void addRobot(Robot robot)
		{
			Robots.Add(robot);
			_world.Step(0);
			if (Robots.Count == 1)
				_camera.TrackingBody = robot.Heart;
			robot.InitScript();
			robot.move(new Vector2(0, -20));
		}

		public override void Update(GameTime gameTime)
		{
			HandleInput();
			if (_pause)
				return;
			//float delta = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds * 2, (2f / 30f));
			float delta = 1 / 30f; // Static delta time for now, yea bitch!
			_Time += delta;
			//Input.update();

			_world.Step(delta);

			foreach (Robot r in Robots)
				r.Update(Map);
			_gameMaster.run();
			_camera.Update(gameTime);
			if (_exiting)
				Exit();
			_FrameCount++;
		}

		public void Finish()
		{
				_exiting = true;
		}

		public void Exit()
		{
			clearRobot();
			ScreenMainMenu.GoBack = true;
			_gameMaster.stop();
		}

		private void HandleInput()
		{
			if (Input.Exit)
				_exiting = true;
			if (Input.justPressed(MouseKeys.WHEEL_DOWN))
				_camera.zoomIn();
			if (Input.justPressed(MouseKeys.WHEEL_UP))
				_camera.zoomOut();
			if (Input.justPressed(Keys.P))
				_pause = !_pause;
		}

		public override void Draw(GameTime gameTime)
		{
			ScreenManager.GraphicsDevice.Clear(Color.LightYellow);
			_drawGame.Begin(_camera);

			foreach (Robot r in Robots)
				r.draw(_drawGame);
			Map.DrawDebug(_drawGame, true);
			_drawGame.End();

			base.Draw(gameTime);

		}
	}
}

