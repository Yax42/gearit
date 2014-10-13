using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using gearit.src.robot;

namespace gearit.src.game
{
	interface IGearitGame
	{
		float Time { get; }
		int FrameCount { get; }
		void Message(string msg, int duration);
		void Finish();
		Map Map { get; }
		List<Robot> Robots { get; }
		int MainRobotId { get; }
	}
}
