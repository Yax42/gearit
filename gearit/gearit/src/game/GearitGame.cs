﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.xna;
using gearit.src.utility;
using FarseerPhysics.Dynamics;
using gearit.src.map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using gearit.src.editor;
using System.Diagnostics;
using FarseerPhysics.Dynamics.Joints;
using gearit.src.output;
using GUI;
using FarseerPhysics.DebugViews;
using FarseerPhysics;
using gearit.src.robot;

namespace gearit.src.game
{
	class GearitGame : GameScreen, IDemoScreen
	{
		private World _world;
		//private Camera2D _camera;
		private Camera2D _camera;

		public Map _Map;
		public Map Map
		{
			get
			{
				return _Map;
			}
			private set
			{
				_Map = value;
			}
		}

		public List<Robot> _Robots;
		public List<Robot> Robots
		{
			get
			{
				return _Robots;
			}
			private set
			{
				_Robots = value;
			}
		}

		private DrawGame _drawGame;

		// Graphic
		private RectangleOverlay _background;

		private DebugViewXNA _debug;

		// Action
		private int _FrameCount = 0;
		public int FrameCount
		{
			get
			{
				return _FrameCount;
			}
		}

		private float _Time = 0;
		public float Time
		{
			get
			{
				return _FrameCount;
			}
		}

		#region IDemoScreen Members

		public GearitGame()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			Robots = new List<Robot>();
			_world = new World(new Vector2(0, 9.8f));
		}

		public string GetTitle()
		{
			return "Game";
		}

		public string GetDetails()
		{
			return ("");
		}

		#endregion

		public override void LoadContent()
		{
			base.LoadContent();

			_FrameCount = 0;
			_drawGame = new DrawGame(ScreenManager.GraphicsDevice, _debug);
			_camera = new Camera2D(ScreenManager.GraphicsDevice);
			_world.Clear();
			_world.Gravity = new Vector2(0f, 19.8f);

			//clearRobot();
			SerializerHelper.World = _world;

			Robot robot = (Robot)Serializer.DeserializeItem("robot/default.gir");
			addRobot(robot);
			Debug.Assert(Robots != null);
			Map = (Map)Serializer.DeserializeItem("map/default.gim");
			if (Map.Artefacts.Count > 0)
				robot.move(Map.Artefacts[0].Position);
			Debug.Assert(Map != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			// I have no idea what this is.
			//HasVirtualStick = true;
		}


		public World getWorld()
		{
			return (_world);
		}

		public void setMap(Map map)
		{
			Map = map;
		}

		public void clearRobot()
		{
			foreach (Robot r in Robots)
				r.remove();
			Robots.Clear();
		}

		public void addRobot(Robot robot)
		{
			Robots.Add(robot);
			if (Robots.Count == 1)
				_camera.TrackingBody = robot.Heart;
			robot.turnOn();
			robot.move(new Vector2(0, -20));
		}

		public override void Update(GameTime gameTime)
		{
			_FrameCount++;
			_Time = (float) gameTime.TotalGameTime.TotalSeconds;
			HandleInput();

			_world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

			foreach (Robot r in Robots)
				r.Update(Map);
			_camera.Update(gameTime);
		}

		public void Finish()
		{
			clearRobot();
			ScreenMainMenu.GoBack = true;
		}

		private void HandleInput()
		{
			if (Input.Exit)
			{
				clearRobot();
				ScreenMainMenu.GoBack = true;
			}
			if (Input.justPressed(MouseKeys.WHEEL_DOWN))
				_camera.zoomIn();
			if (Input.justPressed(MouseKeys.WHEEL_UP))
				_camera.zoomOut();
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

