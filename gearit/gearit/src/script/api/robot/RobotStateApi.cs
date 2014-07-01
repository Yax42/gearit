using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;

namespace gearit.src.script
{
	class RobotStateApi
	{
		private Robot _robot;
		public RobotStateApi(Robot robot)
		{
			_robot = robot;
		}

		public int LastTrigger
		{
			get
			{
				return _robot.LastTrigger;
			}
		}

		public float Position_X()
		{
			return _robot.Position.X;
		}

		public float Position_Y()
		{
			return _robot.Position.Y;
		}

	}
}
