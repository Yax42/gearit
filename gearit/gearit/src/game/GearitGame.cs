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
		public Camera2D Camera { get; private set; }
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

		public GearitGame(string robotPath, string mapPath) : base(true, true)
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
			ScreenManager.IsIngame = true;
			_Robots = new List<Robot>();
			World = new World(new Vector2(0, 9.8f));
			base.LoadContent();
			_exiting = false;
			_pause = false;
			_FrameCount = 0;
			_Time = 0;
			_drawGame = DrawGame.Instance;
			Camera = new Camera2D(ScreenManager.GraphicsDevice);

		#region World

			World.Clear();
			World.Gravity = new Vector2(0f, 9.8f);
			SerializerHelper.World = World;

			Map = (Map)Serializer.DeserializeItem(MapPath);
			Debug.Assert(Map != null);
			ScreenManager.Game.ResetElapsedTime();

			_gameMaster = new GameLuaScript(this, Map.LuaFullPath);

			Robot robot = (Robot)Serializer.DeserializeItem(RobotPath);
			Debug.Assert(robot != null);
			if (robot == null)
				Exit();
			else
				AddRobot(robot);

		#endregion

            _back = ScreenManager.Content.Load<Texture2D>("background");
            _effect = ScreenManager.Content.Load<Effect>("infinite");
			DrawPriority = 99999;
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
			Camera.Update(gameTime);
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
			ScreenManager.IsIngame = false;
			//clearRobot();
			//ScreenMainMenu.GoBack = true;
			Robots.Clear();
			World.Clear();
			ScreenManager.Instance.RemoveScreen(this);
			_gameMaster.stop();
		}

		private void HandleInput()
		{
			if (Input.Exit)
				_exiting = true;
			Camera.HandleInput();
			if (Input.justPressed(Keys.P))
				_pause = !_pause;
		}

		public override void Draw(GameTime gameTime)
		{
			ScreenManager.GraphicsDevice.Clear(Color.LightYellow);
            //_drawGame.drawBackground(_back, Camera, _effect); //mmh consomme trop de fps
			_drawGame.Begin(Camera);
			foreach (Robot r in Robots)
				r.draw(_drawGame);
			Map.DrawDebug(_drawGame, true, true);
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
			if (Robots.Count == 1)
				Camera.TrackingBody = robot.Heart;
			robot.InitScript();
			_gameMaster.RobotConnect(robot);
		}
	}
}

