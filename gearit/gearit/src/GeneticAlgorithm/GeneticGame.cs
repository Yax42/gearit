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

namespace gearit.src.GeneticAlgorithm
{
	class GeneticGame: IGearitGame
	{
		private GameLuaScript _GameMaster;
		private bool _exiting;

		private Map _Map;
		public Map Map { get { return _Map; } }

		private List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }
		private Robot Robot;

		// Action
		private int _FrameCount = 0;
		public int FrameCount { get { return _FrameCount; } }

		private float _Time = 0;
		public float Time { get { return _Time; } }

		public GeneticGame()
		{
			_Robots = new List<Robot>();
			_Map = null;
		}

		public void SetMap(string mapPath)
		{
			if (_Map != null)
				_Map.ExtractFromWorld();
			_Map = (Map)Serializer.DeserializeItem(mapPath);
			Debug.Assert(Map != null);
		}

		public void SetRobot(Robot robot)
		{
			if (_Robots.Count != 0)
			{
				_Robots[0].ExtractFromWorld();
				_Robots.Clear();
			}
			Robot = robot;
			_Robots.Add(Robot);
		}

		public Score Run(string scriptPath, string robotScript)
		{
			_exiting = false;
			_FrameCount = 0;
			_Time = 0;
			_GameMaster = new GameLuaScript(this, scriptPath);
			Robot.InitScript(robotScript);
			while (!_exiting)
			{
				_Time += 1f / 30f;
				LifeManager.World.Step(1f / 30f);

				Robot.Update(Map);
				_GameMaster.run();
				_FrameCount++;
			}
			Robot.StopScript();
			_GameMaster.stop();
			return Robot.Score;
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
