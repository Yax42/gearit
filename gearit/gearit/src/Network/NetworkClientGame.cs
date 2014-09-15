using System;
using System.Collections.Generic;
using gearit.xna;
using gearit.src.utility;
using FarseerPhysics.Dynamics;
using gearit.src.map;
using Microsoft.Xna.Framework;
using gearit.src.editor;
using System.Diagnostics;
using GUI;
using FarseerPhysics.DebugViews;
using gearit.src.robot;
using gearit.src.script;
using gearit.src.editor.robot;
using gearit.src.game;
using System.Threading;
using Lidgren.Network;

namespace gearit.src.Network
{
	class NetworkClientGame : GameScreen, IDemoScreen, IGearitGame
	{
		private World _world;
		private Camera2D _camera;
		private GameLuaScript _gameMaster;
		private bool __exiting;
		private bool _exiting
		{
			get
			{
				return __exiting;
			}
			set
			{
				__exiting = value;
			}
		}

		private Map _Map;
		public Map Map { get { return _Map; } }

		private List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }
		private Robot EnnemyRobot;

		public Robot MainRobot
		{
			get;
			set;
		}

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

		public NetworkClientGame()
		{
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
			_Robots = new List<Robot>();
			_world = new World(new Vector2(0, 9.8f));
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
			base.LoadContent();
			_exiting = false;

			_FrameCount = 0;
			_Time = 0;
			_drawGame = new DrawGame(ScreenManager.GraphicsDevice);
			_camera = new Camera2D(ScreenManager.GraphicsDevice);
			_world.Clear();
			_world.Gravity = new Vector2(0f, 9.8f);

			//clearRobot();
			SerializerHelper.World = _world;

			Robot robot2 = (Robot)Serializer.DeserializeItem("robot/default.gir");
			addOpponentRobot(robot2);

			_world.Step(1/30f);

			Robot robot = (Robot)Serializer.DeserializeItem("robot/default.gir");
			addMainRobot(robot);

			Debug.Assert(Robots != null);
			_Map = (Map)Serializer.DeserializeItem("map/default.gim");
			Debug.Assert(Map != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			_gameMaster = new GameLuaScript(this, LuaManager.LuaFile("game/default"));

			NetworkClient.Connect("127.0.0.1", 25552);

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

		public void addOpponentRobot(Robot robot)
		{
			Robots.Add(robot);
			robot.move(new Vector2(Robots.Count * 30, -20));
			EnnemyRobot = robot;
		}

		public void addMainRobot(Robot robot)
		{
			Robots.Add(robot);
			_camera.TrackingBody = robot.Heart;
			robot.InitScript();
			robot.move(new Vector2(Robots.Count * 30, -20));
			MainRobot = robot;
		}

		public override void Update(GameTime gameTime)
		{
			if (NetworkClient.State != NetworkClient.EState.Connected)
				return ;
			//float delta = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds * 2, (2f / 30f));
			float delta = 1 / 30f; // Static delta time for now, yea bitch!
			_Time += delta;
			HandleInput();

			_world.Step(delta);


			NetworkClient.Send(MainRobot.PacketMotor);
			foreach (NetIncomingMessage request in NetworkClient.Requests)
				EnnemyRobot.PacketMotor = request.Data;
			NetworkClient.CleanRequests();

			//foreach (Robot r in Robots)
				MainRobot.Update(Map);

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
