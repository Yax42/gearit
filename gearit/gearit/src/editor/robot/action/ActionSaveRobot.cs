using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;

namespace gearit.src.editor.robot.action
{
	class ActionSaveRobot : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			return (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.S)));
		}

		public bool run()
		{
			Serializer.SerializeItem("r2d2.gir", RobotEditor.Instance.Robot);
			return (false);
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.SAVE_ROBOT; }
	}
}
