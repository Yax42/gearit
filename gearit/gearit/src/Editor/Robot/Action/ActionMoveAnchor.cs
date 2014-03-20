using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace gearit.src.editor.robot.action
{
	class ActionMoveAnchor : IAction
	{
		private Piece P1;
		private Piece P2;
		private Vector2 From;
		private Vector2 To;
		bool HasBeenRevert;

		public void init()
		{
			if (P1.isConnected(P2) == false)
				P1 = null;
			else
			{
				P1 = RobotEditor.Instance.Select1;
				P2 = RobotEditor.Instance.Select2;
				HasBeenRevert = false;
			}
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, true) && Input.justPressed(MouseKeys.RIGHT));
		}

		public bool run()
		{
			if (P1 == null)
				return false;
			Debug.Assert(P1.isConnected(P2));

			if (!HasBeenRevert && P1.isOn(Input.SimMousePos))
			{
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


		public bool canBeReverted() { return (P1 != null); }

		public ActionTypes type() { return ActionTypes.MOVE_ANCHOR; }
	}
}
