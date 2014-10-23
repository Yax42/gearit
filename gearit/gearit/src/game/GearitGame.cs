using System;
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
using Microsoft.Xna.Framework.Graphics;

namespace gearit.src.game
{
	class GearitGame : GameScreen, IDemoScreen, IGearitGame
	{
		public World World {get; private set; }
		private Camera2D _camera;
		private GameLuaScript _gameMaster;
		private bool _exiting;
		private bool _pause;
		public Map Map { get; set; }
		private string RobotPath;
		private string MapPath;
		private List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }
		private DrawGame _drawGame;
		private RectangleOverlay _background;
		private DebugViewXNA _debug;
		private int _FrameCount = 0;
		public int FrameCount { get { return _FrameCount; } }
		private float _Time = 0;
		public float Time { get { return _Time; } }
        private Texture2D _back;
        private Effect _effect;


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
			World = new World(new Vector2(0, 9.8f));
			base.LoadContent();
			_exiting = false;
			_pause = false;
			_FrameCount = 0;
			_Time = 0;
			_drawGame = new DrawGame(ScreenManager.GraphicsDevice);
			_camera = new Camera2D(ScreenManager.GraphicsDevice);
			World.Clear();
			World.Gravity = new Vector2(0f, 9.8f);
			SerializerHelper.World = World;
			Robot robot = (Robot)Serializer.DeserializeItem(RobotPath);
			addRobot(robot);
			Debug.Assert(Robots != null);
			Map = (Map)Serializer.DeserializeItem(MapPath);
			Debug.Assert(Map != null);
			ScreenManager.Game.ResetElapsedTime();
			_gameMaster = new GameLuaScript(this, Map.LuaFullPath);
            _back = ScreenManager.Content.Load<Texture2D>("background");
            _effect = ScreenManager.Content.Load<Effect>("infinite");
		}

		public World getWorld()
		{
			return (World);
		}

		public void setMap(Map map)
		{
			Map = map;
		}

		public void clearRobot()
		{
			foreach (Robot r in Robots)
				r.ExtractFromWorld();
			Robots.Clear();
		}

		public int MainRobotId { get { return 0; } set { } }

		public void addRobot(Robot robot)
		{
			Robots.Add(robot);
			World.Step(0);
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

			World.Step(delta);

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
            _drawGame.drawBackground(_back, _camera, _effect); 
			_drawGame.Begin(_camera);
			foreach (Robot r in Robots)
				r.draw(_drawGame);
			Map.DrawDebug(_drawGame, true);
			_drawGame.End();
			base.Draw(gameTime);
		}

		public Robot RobotFromId(int id)
		{
			return Robot.RobotFromId(Robots, id);
		}

		public void Go()
		{}

		public void AddRobot(Robot robot)
		{
			Robots.Add(robot);
			World.Step(0);
			World.Step(1/30f);
			//robot.move(new Vector2(Robots.Count * 30, -20));
		}
	}
}

