using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionMainSelect : IAction
	{
		public void init()
		{
			RobotEditor.Instance.Select1 = RobotEditor.Instance.Robot.GetPiece(Input.SimMousePos);
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && Input.justPressed(MouseKeys.LEFT));
		}

		public bool run()
		{
			return (false);
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.MAIN_SELECT; }
	}
}
