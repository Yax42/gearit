using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot.action
{
	class ActionMoveRobot : IAction
	{
		private Vector2 From;
		private Vector2 To;
		bool HasBeenRevert;

		public void init()
		{
			HasBeenRevert = false;
			From = RobotEditor.Instance.Robot.Position;
		}

		public bool shortcut()
		{
			return (Input.CtrlShift(false, false) && (Input.justPressed(Keys.C)));
		}

		public bool run()
		{
			if (!HasBeenRevert)
			{
				To = Input.VirtualSimMousePos;
			}

			RobotEditor.Instance.Robot.move(To);
			return (!HasBeenRevert
					&& Input.justReleased(Keys.C) == false
					&& Input.justPressed(MouseKeys.LEFT) == false);
		}

		public void revert()
		{
			HasBeenRevert = true;
			RobotEditor.Instance.Robot.move(From);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.MOVE_ROBOT; }
	}
}
