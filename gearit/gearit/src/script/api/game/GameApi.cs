using gearit.src.game;
using gearit.src.script.api.game;
using gearit.src.editor.map;
using System.Collections.Generic;

namespace gearit.src.script
{
	class GameApi
	{
		private IGearitGame _game;
		private GameRobotApi[] _robots;
		private GameArtefactApi[] _arts;
		private List<GameChunkApi> _chunks;

		public GameApi(IGearitGame game)
		{
			_game = game;
			_robots = new GameRobotApi[game.Robots.Count];
			for (int i = 0; i < game.Robots.Count; i++ )
			{
				_robots[i] = new GameRobotApi(game.Robots[i]);
				GameLuaScript.Instance["Robot_" + i] = _robots[i];
			}

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
			get
			{
				return _game.Robots.Count;
			}
		}

		public GameRobotApi Robot(int i)
		{
			return _robots[i];
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
