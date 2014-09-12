﻿using System;
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

namespace gearit.src.Network
{
	class NetworkServerGame : IGearitGame
	{
		private World _world;
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

		// Action
		private int _FrameCount = 0;
		public int FrameCount { get { return _FrameCount; } }

		private float _Time = 0;
		public float Time { get { return _Time; } }

		public NetworkServerGame()
		{
			_Robots = new List<Robot>();
			_world = new World(new Vector2(0, 9.8f));
		}

		public void LoadContent()
		{
			_exiting = false;

			_FrameCount = 0;
			_Time = 0;
			_world.Clear();
			_world.Gravity = new Vector2(0f, 9.8f);

			//clearRobot();
			SerializerHelper.World = _world;



			Debug.Assert(Robots != null);
			_Map = (Map)Serializer.DeserializeItem("map/default.gim");
			Debug.Assert(Map != null);
			// Loading may take a while... so prevent the game from "catching up" once we finished loading

			_gameMaster = new GameLuaScript(this, LuaManager.LuaFile("game/default"));

			// I have no idea what this is.
			//HasVirtualStick = true;
		}

		public World GetWorld()
		{
			return (_world);
		}

		public void SetMap(Map map)
		{
			_Map = map;
		}

		public void clearRobot()
		{
			foreach (Robot r in Robots)
				r.ExtractFromWorld();
			Robots.Clear();
		}

		public void AddRobot(Robot robot)
		{
			//Robot robot = (Robot)Serializer.DeserializeItem("robot/default.gir");
			Robots.Add(robot);
			_world.Step(1/30f);
		}

		public void Update(GameTime gameTime)
		{
			//float delta = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds * 2, (2f / 30f));
			float delta = 1 / 30f; // Static delta time for now, yea bitch!
			_Time += delta;

			_world.Step(delta);


			//foreach (Robot r in Robots)
			//MainRobot.Update(Map);

			//_gameMaster.run();
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
			_gameMaster.stop();
		}
	}
}