using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.robot.action
{
	class ActionSelect2 : IAction
	{
		public void init()
		{
			RobotEditor.Instance.Select2 = RobotEditor.Instance.Robot.GetPiece(Input.SimMousePos);
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, true) && Input.justPressed(MouseKeys.LEFT));
		}

		public bool run()
		{
			return (false);
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.SELECT2; }
	}
}
