using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace gearit.src.script
{
	public class RobotStateApi
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

		public bool[] TriggerData
		{
			get
			{
				return _robot.TriggersData;
			}
		}
	}
}
