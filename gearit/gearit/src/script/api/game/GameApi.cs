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
		private GearitGame _game;

		public GameApi(GearitGame game)
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

		public void FinishGame()
		{
			_game.Finish();
		}
	}
}
