using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.game;
using FarseerPhysics.Dynamics;
using gearit.src.script;
using gearit.src.map;
using gearit.src.robot;
using Microsoft.Xna.Framework;
using gearit.src.editor;
using System.Diagnostics;
using gearit.src.editor.robot;
using System.Threading;
using gearit.src.editor.map;

namespace gearit.src.Network
{
	class NetworkServerGame : IGearitGame
	{
		public World World { get; private set; }
		private GameLuaScript _gameMaster;
		public byte[] Events =  new byte[0];

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

		public Map Map { get; set; }

		public int MainRobotId
		{
			get
			{
				Debug.Assert(false);
				return 0;
			}
			set
			{
				Debug.Assert(false);
			}
		}

		private List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }
		private string MapPath;
		public PacketManager PacketManager;

		// Action
		private int _FrameCount = 0;
		public int FrameCount { get { return _FrameCount; } }

		private float _Time = 0;
		public float Time { get { return _Time; } }

		public void Message(string msg, int duration)
		{
		}

		public NetworkServerGame(string mapPath)
		{
			MapPath = mapPath;
			_Robots = new List<Robot>();
			World = new World(new Vector2(0, 9.8f));
			PacketManager = new PacketManager(this);
		}

		public void LoadContent()
		{
			_exiting = false;

			_FrameCount = 0;
			_Time = 0;
			World.Clear();
			World.Gravity = new Vector2(0f, 9.8f);

			//clearRobot();
			SerializerHelper.World = World;
			PacketManager.Network = NetworkServer.Instance;
			Debug.Assert(Robots != null);
			Map = (Map)Serializer.DeserializeItem(MapPath);
			Debug.Assert(Map != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading

			_gameMaster = new GameLuaScript(this, Map.LuaFullPath);

			// I have no idea what this is.
			//HasVirtualStick = true;
		}

		public void AddRobot(Robot robot)
		{
			Robots.Add(robot);
			World.Step(0);
			World.Step(1/30f);
			_gameMaster.RobotConnect(robot);
			//robot.move(new Vector2(Robots.Count * 30, -20));
		}

		public void RemoveRobot(Robot robot)
		{
			Robots.Remove(robot);
			_gameMaster.RobotDisconnect(robot);
			//robot.move(new Vector2(Robots.Count * 30, -20));
		}

		public void clearRobot()
		{
			foreach (Robot r in Robots)
				r.ExtractFromWorld();
			Robots.Clear();
		}

		public void Update(float delta)
		{
			//float delta = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds * 2, (2f / 30f));
			//float delta = 1 / 30f; // Static delta time for now, yea bitch!
			_Time += delta;
			NetworkServer.Instance.ApplyRequests();

			World.Step(delta * 2);

			foreach (Robot r in Robots)
				r.Update(Map);
			foreach (Robot r in Robots)
			{
				//NetworkServer.Instance.PushRequestTransform(PacketManager.RobotTransform(r));
				for (int i = 0; i < r.Pieces.Count; i++) // i = 1 parce qu'on veut ignorer le coeur
					NetworkServer.Instance.PushRequestTransform(PacketManager.RobotPieceTransform(r, i));
			}
			for (int i = 0; i < Map.Chunks.Count; i++)
			{
				if ((Map.Chunks[i].StringId != null
					&& Map.Chunks[i].StringId != "")
					|| !Map.Chunks[i].IsStatic)
					NetworkServer.Instance.PushRequestTransform(PacketManager.ChunkTransform(i));
			}
			//Console.Out.WriteLine("server");

			_gameMaster.run();
			if (_exiting)
				Exit();
			_FrameCount++;
		}

		public void Finish()
		{
			Console.WriteLine("finish game");
			//_exiting = true;
		}

		public Robot RobotFromId(int id)
		{
			return Robot.RobotFromId(Robots, id);
		}

		public void Exit()
		{
			clearRobot();
			_gameMaster.stop();
		}

		public void Go()
		{}
	}
}