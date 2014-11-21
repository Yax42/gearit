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
using Squid;
using gearit.src.GUI;

namespace gearit.src.Network
{
	class NetworkClientGame : GameScreen, IDemoScreen, IGearitGame
	{
		public World World { get; private set; }
		public Camera2D Camera { get; private set; }
		private GameLuaScript _gameMaster;
		private bool __exiting;
		public NetworkClient NetworkClient { get; private set; }
		private Status Status;

		private string NameRobot;
		private string IpServer;
        private Texture2D _back;
        private Effect _effect;

		private Desktop _scoring = new Desktop();

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

		public NetworkClientGame(string robot, string ip) : base(true, true)
		{
			IpServer = ip;
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
			ChatBox.LastInput = null;
			ScreenManager.IsIngame = true;
			base.LoadContent();

			_Robots = new List<Robot>();
			World = new World(new Vector2(0, 9.8f));
			PacketManager = new PacketManager(this);

			DrawPriority = 9999;
			_exiting = false;

			_FrameCount = 0;
			_Time = 0;
			_drawGame = DrawGame.Instance;
			Camera = new Camera2D(ScreenManager.GraphicsDevice);
			World.Clear();
			World.Gravity = new Vector2(0f, 9.8f);

            NetPeerConfiguration config = new NetPeerConfiguration("gearit");
			config.ConnectionTimeout = 2000;
			config.ResendHandshakeInterval = 1;
			config.MaximumHandshakeAttempts = 2;
			NetworkClient = new NetworkClient(config, PacketManager);
			PacketManager.Network = NetworkClient;

			//clearRobot();
			SerializerHelper.World = World;
			Robot r = (Robot)Serializer.DeserializeItem(NameRobot);
			r.Label = r.Name;
			r.Owner = GI_Data.Pseudo;
			string backupPath = r.FullPath;
			r.FullPath = NetworkClient.Path + "MyRobot.gir";
			r.Save();
			r.FullPath = backupPath;
			AddRobot(r);
			MainRobotId = 0;
			Camera.TrackingBody = r.Heart;
			r.InitScript(this);

			Debug.Assert(Robots != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading
			ScreenManager.Game.ResetElapsedTime();

			NetworkClient.Connect(IpServer, INetwork.SERVER_PORT);
            _back = ScreenManager.Content.Load<Texture2D>("background");
            _effect = ScreenManager.Content.Load<Effect>("infinite");
			Status = Status.Init;

			// Scoring
			_scoring.Size = new Squid.Point(ScreenManager.Instance.Width, ScreenManager.Instance.Height);
			_scoring.Position = new Squid.Point(0, 0);
			Panel panel = new Panel();
			panel.Parent = _scoring;
			panel.Style = "scoring";
			panel.Size = _scoring.Size * 0.6f;
			panel.Position = _scoring.Size / 2 - panel.Size / 2;

			olv.Position = new Squid.Point(0, 0);
			olv.Dock = DockStyle.Fill;
			panel.Content.Controls.Add(olv);
            olv.Columns.Add(new ListView.Column { Text = "NAME", Aspect = "Name", Width = 20, MinWidth = 400 });
            olv.Columns.Add(new ListView.Column { Text = "ROBOT", Aspect = "Robot", Width = 20, MinWidth = 400 });
            olv.Columns.Add(new ListView.Column { Text = "SCORE", Aspect = "Score", Width = 20, MinWidth = 400 });
            olv.Columns.Add(new ListView.Column { Text = "PING", Aspect = "Ping", Width = 20, MinWidth = 400 });
            olv.Columns[0].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[1].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[2].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.Columns[3].Width = (ScreenManager.Width - ScreenMainMenu.MENU_WIDTH) / 6;
            olv.FullRowSelect = true;
            olv.Style = "scoring";

			olv.CreateCell = delegate(object sender, ListView.FormatCellEventArgs args)
			{
                string text = olv.GetAspectValue(args.Model, args.Column);
                Button cell =  new Button
                {
                    Size = new Squid.Point(26, 26),
                    Style = "itemScoring",
                    Dock = DockStyle.Top,
                    Text = text
                };
				return cell;
			};
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
				else if (NetworkClient.State == NetworkClient.EState.Disconnected)
					Exit();
				return;
			}
			if (NetworkClient.Requests.Count == 0)
				return ;
			if (Status == Status.Init)
				NetworkClient.ApplyBruteRequests();

			ProcessChatboxMessage();
			//Console.Out.WriteLine("Client " + NetworkClient.Requests.Count);
			float delta = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds * 2, (3f / 30f));
			//float delta = 2 / 30f; // Static delta time for now, yea bitch!
			_Time += delta;
			HandleInput();

			World.Step(delta);


			//for (int i = 0; i < MainRobot.Spots.Count; i++)
			//	NetworkClient.PushRequest(PacketManager.MotorForce(i));
			NetworkClient.SendRequests();

			NetworkClient.ApplyRequests();

			MainRobot.Update(Map);

			//_gameMaster.run();
			Camera.Update(gameTime);
			if (_exiting)
				Exit();
			_FrameCount++;
		}

		private void ProcessChatboxMessage()
		{
			if (ChatBox.LastInput != null)
			{
				NetworkClient.BruteSend(null, PacketManager.Message(ChatBox.LastInput, MainRobot.Id));
				ChatBox.LastInput = null;
			}
		}

		public void Finish()
		{
			Status = Status.Finish;
			_exiting = true;
		}

		public void Exit()
		{
			ScreenManager.IsIngame = false;
			clearRobot();
			NetworkClient.Disconnect();
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
			Camera.HandleInput();
		}

		public override void Draw(GameTime gameTime)
		{
			if (Status != Status.Run)
				return;
			ScreenManager.GraphicsDevice.Clear(Color.LightYellow);
            //_drawGame.drawBackground(_back, Camera, _effect); //mmh consomme trop de fps
			_drawGame.Begin(Camera);
			foreach (Robot r in Robots)
				r.draw(_drawGame);
			Map.DrawDebug(_drawGame, true, true);
			_drawGame.End();

			drawScoring();

			base.Draw(gameTime);
		}

        ListView olv = new ListView();
        List<MyData> models = new List<MyData>();
        public class MyData
        {
            public string Name;
            public string Robot;
            public float Score;
            public int Ping;
        }

		private void drawScoring()
		{
			if (!Input.pressed(Microsoft.Xna.Framework.Input.Keys.Tab))
				return;

			models.Clear();
			foreach (Robot r in Robots)
			{
				MyData entry = new MyData();
				entry.Name = r.Owner;
				entry.Robot = r.Label;
				entry.Score = r.Score.IntScore;
				entry.Ping = 0;
				models.Add(entry);
				olv.SetObjects(models);
			}

			_scoring.Update();
			_scoring.Draw();

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
