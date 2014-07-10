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

namespace GeneticAlgorithm.src
{
	class GeneticGame: IGearitGame
	{
		private World _world;
		private GameLuaScript _gameMaster;
		private bool _exiting;

		public Map _Map;
		public Map Map { get { return _Map; } }

		public List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }

		// Action
		private int _FrameCount = 0;
		public int FrameCount { get { return _FrameCount; } }

		private float _Time = 0;
		public float Time { get { return _Time; } }

		public GeneticGame()
		{
			_Robots = new List<Robot>();
			_world = new World(new Vector2(0, 9.8f));
			SerializerHelper.World = _world;

			_Map = null;
		}

		public void SetMap(string mapPath)
		{
			if (_Map != null)
				_Map.remove();
			_Map = (Map)Serializer.DeserializeItem(mapPath);
			Debug.Assert(Map != null);
		}

		public void SetGameMaster(string scriptPath)
		{
			_gameMaster = new GameLuaScript(this, scriptPath);
		}

		public Score Run(string robotPath)
		{
			_exiting = false;
			_FrameCount = 0;
			_Time = 0;
			Robot robot = (Robot)Serializer.DeserializeItem(robotPath);
			Debug.Assert(robot != null);
			_Robots.Add(robot);
			robot.turnOn();

			while (!_exiting)
			{
				_FrameCount++;
				_Time += 1f / 30f;
				_world.Step(1f / 30f);

				robot.Update(Map);
				_gameMaster.run();
			}
			_gameMaster.stop();
			Score score = robot.Score;
			robot.remove();
			Robots.Clear();
			return score;
		}

		public void Finish()
		{
			_exiting = true;
		}

		public void Exit()
		{
		}
	}
}
