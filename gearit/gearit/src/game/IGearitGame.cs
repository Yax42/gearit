using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using gearit.src.robot;
using FarseerPhysics.Dynamics;

namespace gearit.src.game
{
	public enum Status
	{
		Init,
		Run,
		Finish,
	}

	interface IGearitGame
	{
		float Time { get; }
		int FrameCount { get; }
		void Message(string msg, int duration);
		void Finish();
		void Go();
		Map Map { get; set; }
		List<Robot> Robots { get; }
		Robot RobotFromId(int id);
		int MainRobotId { get; set; }
		World World { get; }
		void AddRobot(Robot r);
	}
}
