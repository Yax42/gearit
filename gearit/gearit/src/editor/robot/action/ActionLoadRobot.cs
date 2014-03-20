using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionLoadRobot : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			return (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.D)));
		}

		public bool run()
		{
			RobotEditor.Instance.Robot.remove();
			RobotEditor.Instance.clearActionLog();
			//robot = new Robot(RobotEditor._world);
			RobotEditor.Instance.Robot = (Robot)Serializer.DeserializeItem("r2d2.gir");
			RobotEditor.Instance.Select1 = RobotEditor.Instance.Robot.getHeart();
			RobotEditor.Instance.Select2 = RobotEditor.Instance.Robot.getHeart();
			return (false);
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.LOAD_ROBOT; }
	}
}