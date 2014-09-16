using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.game;
using System.Diagnostics;

namespace gearit.src.script
{
	class GameApi
	{
		private IGearitGame _game;

		public GameApi(IGearitGame game)
		{
			_game = game;
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
