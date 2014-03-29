using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot.action
{
	class ActionMovePiece : IAction
	{
		private Vector2 From;
		private Vector2 To;
		private Piece P1;
		private bool HasBeenRevert;

		public void init()
		{
			HasBeenRevert = false;
			P1 = RobotEditor.Instance.Select1;
			From = P1.Position;
			To = From;
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && Input.justPressed(MouseKeys.RIGHT));
		}

		public bool run()
		{
			if (!HasBeenRevert)
			{
				To = Input.SimMousePos;
			}

			P1.move(To);
			return (!HasBeenRevert && Input.pressed(MouseKeys.RIGHT));
		}

		public void revert()
		{
			HasBeenRevert = true;
			P1.move(From);
		}

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.MOVE_PIECE; }
	}
}
