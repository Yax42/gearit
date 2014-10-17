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
using gearit.xna;
using gearit.src.output;

namespace gearit.src.GeneticAlgorithm
{
	class GeneticGame: IGearitGame
	{
		private GameLuaScript _GameMaster;
		private bool _exiting;

		public Map Map { get; set; }
		public World World { get { return LifeManager.World; } }

		private List<Robot> _Robots;
		public List<Robot> Robots { get { return _Robots; } }
		public Robot Robot;

		// Action
		private int _FrameCount = 0;
		public int FrameCount { get { return _FrameCount; } }

		private float _Time = 0;
		public float Time { get { return _Time; } }

		public int MainRobotId { get { return 0; } set { } }

		public GeneticGame()
		{
			_Robots = new List<Robot>();
			Map = null;
		}

		public void Message(string msg, int duration)
		{
		}

		public void SetMap(string mapPath)
		{
			if (Map != null)
				Map.ExtractFromWorld();
			Map = (Map)Serializer.DeserializeItem(mapPath);
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

		public void ManualInit(string scriptPath, string robotScript)
		{
			_exiting = false;
			_FrameCount = 0;
			_Time = 0;
			_GameMaster = new GameLuaScript(this, scriptPath);
			Robot.InitScript(robotScript);
			LifeManager.World.Step(0);
		}

		public bool ManualUpdate()
		{
			if (!_exiting)
			{
				_Time += 1f / 30f;
				LifeManager.World.Step(1f / 30f);

				Robot.Update(Map);
				_GameMaster.run();
				_FrameCount++;
				return true;
			}
			else
			{
				Robot.StopScript();
				_GameMaster.stop();
				OutputManager.LogMerge(" ?= " + Robot.Score.FloatScore + "|" + Robot.Score.IntScore);
				return false;
			}
		}


		public void Finish()
		{
			_exiting = true;
		}

		public void Exit()
		{
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
