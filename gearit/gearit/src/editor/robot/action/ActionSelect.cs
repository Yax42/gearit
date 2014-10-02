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
			if (Input.Alt)
				MirrorAxis.Active = true;
			if (Input.CtrlShift(false, false))
				RobotEditor.Instance.Select1 = RobotEditor.Instance.Robot.GetPiece(Input.SimMousePos);
			else if (Input.CtrlShift(false, true))
				RobotEditor.Instance.Select2 = RobotEditor.Instance.Robot.GetPiece(Input.SimMousePos);
			else if (Input.CtrlShift(true, false))
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

			if (Input.Alt)
				MirrorAxis.Active = false;
		}

		public bool shortcut()
		{
			return (Input.CtrlShift(false, false)
				|| Input.CtrlShift(false, true)
				|| Input.CtrlShift(true, false))
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
