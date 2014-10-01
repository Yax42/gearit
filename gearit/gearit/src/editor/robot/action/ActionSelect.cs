using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
	class ActionSelect : IAction
	{
		public void init()
		{
			if (Input.CtrlAltShift(false, false, false))
				RobotEditor.Instance.Select1 = RobotEditor.Instance.Robot.GetPiece(Input.SimMousePos);
			else if (Input.CtrlAltShift(false, false, true))
				RobotEditor.Instance.Select2 = RobotEditor.Instance.Robot.GetPiece(Input.SimMousePos);
			else if (Input.CtrlAltShift(true, false, false))
			{
				var rev = RobotEditor.Instance.Robot.GetCloseSpot(Input.SimMousePos);
				if (rev != null)
				{
					RobotEditor.Instance.Select1 = (Piece) rev.BodyA;
					RobotEditor.Instance.Select2 = (Piece) rev.BodyB;
				}
			}
			else
				Debug.Assert(false);
		}

		public bool shortcut()
		{
			return (Input.CtrlAltShift(false, false, false)
				|| Input.CtrlAltShift(false, false, true)
				|| Input.CtrlAltShift(true, false, false))
				&& Input.justPressed(MouseKeys.LEFT);
		}

		public bool run()
		{
			return false;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.SELECT; }
	}
}
