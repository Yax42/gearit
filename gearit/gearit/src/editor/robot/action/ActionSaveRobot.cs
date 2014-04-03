using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using System.Diagnostics;

namespace gearit.src.editor.robot.action
{
	class ActionSaveRobot : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			if (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.S)))
				MenuRobotEditor.Instance.saveRobot();
			if (Input.ctrlAltShift(true, false, true) && (Input.justPressed(Keys.S)))
				MenuRobotEditor.Instance.saveasRobot();
			return false;
		}

		public bool run()
		{
			Debug.Assert(RobotEditor.Instance.NamePath != "");
			RobotEditor.Instance.Robot.Name = RobotEditor.Instance.NamePath;
			if (!Serializer.SerializeItem("robot/" + RobotEditor.Instance.NamePath + ".gir", RobotEditor.Instance.Robot))
				RobotEditor.Instance.resetNamePath();

			return (false);
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.SAVE_ROBOT; }
	}
}
