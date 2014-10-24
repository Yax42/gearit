using gearit.src.game;
using gearit.src.script.api.game;
using gearit.src.editor.map;
using System.Collections.Generic;
using gearit.src.robot;
using LuaInterface;
using System;

namespace gearit.src.script
{
    /// <summary>
	/// Lua API for IGearitGame accesible via script
    /// </summary>
	class GameApi
	{
		private IGearitGame _game;
		private List<GameRobotApi> _robotsList;
		private GameRobotApi[] _robotsArray;
		private GameArtefactApi[] _arts;
		private List<GameChunkApi> _chunks;
		private Action<int> CallBackRobot;

		public GameApi(IGearitGame game, Action<int> action)
		{
			CallBackRobot = action;
			_game = game;
			_robotsList = new List<GameRobotApi>();
			for (int i = 0; i < game.Robots.Count; i++ )
			{
				_robotsList.Add(new GameRobotApi(game.Robots[i]));
				//GameLuaScript.Instance["Robot_" + i] = _robots[i];
			}
			_robotsArray = _robotsList.ToArray();
			RobotCount = game.Robots.Count;

			_arts = new GameArtefactApi[game.Map.Artefacts.Count];
			for (int i = 0; i < game.Map.Artefacts.Count; i++ )
			{
				_arts[i] = new GameArtefactApi(game.Map.Artefacts[i]);
				GameLuaScript.Instance["Art_" + i] = _arts[i];
			}

			_chunks = new List<GameChunkApi>();
			foreach (MapChunk c in game.Map.Chunks)
				if (c.StringId != null && c.StringId != "")
				{
					var api = new GameChunkApi(c);
					_chunks.Add(api);
					GameLuaScript.Instance["Object_" + c.StringId] = api;
				}
		}

		public void AddRobot(Robot r)
		{
			_robotsList.Add(new GameRobotApi(r));
			_robotsArray = _robotsList.ToArray();
			RobotCount++;
			CallBackRobot(RobotCount - 1);
		}

		public void RemoveRobot(Robot r)
		{
			_robotsList.RemoveAll(rApi => rApi.__Robot == r);
			_robotsArray = _robotsList.ToArray();
			RobotCount--;
		}

		public float Time
		{
			get
			{
				return _game.Time;
			}
		}

		public float Frame
		{
			get
			{
				return _game.FrameCount;
			}
		}

		public int RobotCount
		{
			get;
			private set;
		}

		public GameRobotApi Robot(int i)
		{
			return _robotsArray[i];
		}

		public GameArtefactApi Art(int i)
		{
			return _arts[i];
		}

		public GameChunkApi Object(string name)
		{
			return _chunks.Find(x => x.__Chunk.StringId == name);
		}

		public void Finish()
		{
			_game.Finish();
		}

		public void Message(string msg, int duration = 2000)
		{
			_game.Message(msg, duration);
		}
	}
}
