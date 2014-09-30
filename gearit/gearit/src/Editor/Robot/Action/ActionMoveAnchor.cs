using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
	class ActionMoveAnchor : IAction
	{
		private Piece P1;
		private Piece P2;
		private Vector2 From;
		private Vector2 To;
		private bool HasBeenRevert;
		private bool IsOk;

		public void init()
		{
			P2 = RobotEditor.Instance.Select1;
			P1 = RobotEditor.Instance.Select2;
			HasBeenRevert = false;

			IsOk = P1.isConnected(P2);
			if (IsOk)
			{
				From = P1.getConnection(P2).getWorldAnchor(P1);
				To = From;
			}
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, true) && Input.justPressed(MouseKeys.RIGHT));
		}

		public bool run()
		{
			if (!IsOk)
				return false;
			Debug.Assert(P1.isConnected(P2));

			if (!HasBeenRevert)
			{
				if (P1.GetType() != typeof(Heart))
				{
					To = P1.ClosestPositionInside(Input.SimMousePos);
				}
				else if (P1.Contain(Input.SimMousePos))
					To = Input.SimMousePos;
			}
			P1.getConnection(P2).moveAnchor(P1, To);
			return (!HasBeenRevert && Input.pressed(MouseKeys.RIGHT));
		}

		public void revert()
		{
			HasBeenRevert = true;
			Debug.Assert(P1.isConnected(P2));
			P1.getConnection(P2).moveAnchor(P1, From);
		}


		public bool canBeReverted() { return IsOk; }

		public ActionTypes type() { return ActionTypes.MOVE_ANCHOR; }
	}
}
