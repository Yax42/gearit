using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using gearit.src.output;

namespace gearit.src.editor.robot.action
{
	class ActionLoadRobot : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			if (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.D)))
				MenuRobotEditor.Instance.loadRobot();
			return false;
		}

		public bool run()
		{
			Debug.Assert(RobotEditor.Instance.NamePath != "");
			Robot robot = (Robot)Serializer.DeserializeItem("robot/" + RobotEditor.Instance.NamePath + ".gir");
			if (robot == null)
				RobotEditor.Instance.resetNamePath();
			else
				RobotEditor.Instance.resetRobot(robot);
			return false;
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.LOAD_ROBOT; }
	}
}