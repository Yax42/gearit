using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace gearit.src.script
{
	class RobotStateApi
	{
		private Robot _robot;
		public RobotStateApi(Robot robot)
		{
			_robot = robot;
		}

		public int State
		{
			get
			{
				return _robot.State;
			}
		}

		/*
		// Not sure if I want to give the player access to those
		public int LastTrigger
		{
			get
			{
				return _robot.LastTrigger;
			}
		}

		public bool[] TriggerData
		{
			get
			{
				return _robot.TriggersData;
			}
		}
		*/
	}
}
