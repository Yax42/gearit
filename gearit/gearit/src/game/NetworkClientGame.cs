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
using Microsoft.Xna.Framework.Graphics;

namespace gearit.src.Network
{
	class NetworkClientGame : GameScreen, IDemoScreen, IGearitGame
	{
		public World World { get; private set; }
		private Camera2D _camera;
		private GameLuaScript _gameMaster;
		private bool __exiting;
		public NetworkClient NetworkClient { get; private set; }
		private Status Status;

		private string NameRobot;
		private string NameMap;
		private string IpServer;
        private Texture2D _back;
        private Effect _effect;

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

		public Map Map { get; set; }

		private List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }
		private Robot EnnemyRobot;
		public PacketManager PacketManager { get; private set; }

		public Robot MainRobot { get { return RobotFromId(MainRobotId); } }
		public int MainRobotId
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

		public NetworkClientGame(string map, string robot, string ip) : base(true)
		{
			IpServer = ip;
			NameMap = map;
			NameRobot = robot;
			TransitionOnTime = TimeSpan.FromSeconds(0.75);
			TransitionOffTime = TimeSpan.FromSeconds(0.75);
			HasCursor = true;
		}

		public string GetTitle()
		{
			return "GearIt";
		}

		public void Message(string msg, int duration)
		{
			ScreenManager.Message(msg, duration);
		}

		public string GetDetails()
		{
			return ("");
		}

		#endregion

		public override void LoadContent()
		{
			base.LoadContent();

			_Robots = new List<Robot>();
			World = new World(new Vector2(0, 9.8f));
			PacketManager = new PacketManager(this);

			DrawPriority = 9999;
			_exiting = false;

			_FrameCount = 0;
			_Time = 0;
			_drawGame = DrawGame.Instance;
			_camera = new Camera2D(ScreenManager.GraphicsDevice);
			World.Clear();
			World.Gravity = new Vector2(0f, 9.8f);

			//clearRobot();
			SerializerHelper.World = World;
			Robot r = (Robot)Serializer.DeserializeItem(NameRobot);
			AddRobot(r);
			r.Id = 0;
			MainRobotId = 0;
			_camera.TrackingBody = r.Heart;
			r.InitScript(this);

			Debug.Assert(Robots != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
			NetworkClient = new NetworkClient(config, PacketManager);
			PacketManager.Network = NetworkClient;
			//NetworkClient.Connect("85.68.238.220", 25552, PacketManager);
			//NetworkClient.Connect("81.249.189.167", 25552);
			NetworkClient.Connect(IpServer, INetwork.SERVER_PORT);
            _back = ScreenManager.Content.Load<Texture2D>("background");
            _effect = ScreenManager.Content.Load<Effect>("infinite");
			Status = Status.Init;
		}

		public void clearRobot()
		{
			foreach (Robot r in Robots)
				r.ExtractFromWorld();
			Robots.Clear();
			World.Clear();
		}

		public void AddRobot(Robot robot)
		{
			Robots.Add(robot);
			World.Step(0);
			World.Step(1/30f);
			//robot.move(new Vector2(Robots.Count * 30, -20));
		}

		public override void Update(GameTime gameTime)
		{
			if (NetworkClient.State != NetworkClient.EState.Connected)
			{
				if (Status == Status.Run)
					Exit();
				return;
			}
			if (NetworkClient.Requests.Count == 0)
				return ;
			if (Status == Status.Init)
				NetworkClient.ApplyBruteRequests();

			//Console.Out.WriteLine("Client " + NetworkClient.Requests.Count);
			float delta = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds * 2, (3f / 30f));
			//float delta = 2 / 30f; // Static delta time for now, yea bitch!
			_Time += delta;
			HandleInput();

			World.Step(0);


			//for (int i = 0; i < MainRobot.Spots.Count; i++)
			//	NetworkClient.PushRequest(PacketManager.MotorForce(i));
			NetworkClient.SendRequests();

			NetworkClient.ApplyRequests();

			MainRobot.Update(Map);

			//_gameMaster.run();
			_camera.Update(gameTime);
			if (_exiting)
				Exit();
			_FrameCount++;
		}

		public void Finish()
		{
			Status = Status.Finish;
			_exiting = true;
		}

		public void Exit()
		{
			clearRobot();
			ScreenManager.Instance.RemoveScreen(this);
		}

		private void HandleInput()
		{
            /*
			if (Input.justPressed(Microsoft.Xna.Framework.Input.Keys.X))
				NetworkServer.Instance.Stop();
             * */
			if (Input.Exit)
				_exiting = true;
			if (Input.justPressed(MouseKeys.WHEEL_DOWN))
				_camera.zoomIn();
			if (Input.justPressed(MouseKeys.WHEEL_UP))
				_camera.zoomOut();
		}

		public override void Draw(GameTime gameTime)
		{
			if (Status != Status.Run)
				return;
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
		{
			Status = Status.Run;
		}
	}
}
