using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
	class ActionMovePiece : IAction
	{
		private Vector2 From;
		private Vector2 To;
		private Piece P1;
		private bool HasBeenRevert;
		private bool EndId;
		private bool IsRod;
		private Rod Rod;

		public void init()
		{
			HasBeenRevert = false;
			P1 = RobotEditor.Instance.Select1;
			IsRod = (RobotEditor.Instance.Select1.GetType() == typeof(Rod));
			if (IsRod)
			{
				Rod = (Rod) P1;
				EndId = Rod.CloseEnd(Input.VirtualSimMousePos);
				From = Rod.GetEnd(EndId);
			}
			else
			{
				From = P1.Position;
			}
			To = From;
		}

		public bool shortcut()
		{
			if (RobotEditor.Instance.Select1.GetType() == typeof(Heart))
				return false;
			return (Input.CtrlShift(false, false)
				&& Input.justPressed(MouseKeys.RIGHT));
		}

		public bool run()
		{
			if (!HasBeenRevert)
			{
				To = Input.VirtualSimMousePos;
			}
			RobotEditor.Instance.Robot.ResetActEnds();
			if (IsRod)
				Rod.SetEnd(To, EndId);
			else
				P1.move(To, true, true);
			return (!HasBeenRevert && Input.pressed(MouseKeys.RIGHT));
		}

		public void revert()
		{
			HasBeenRevert = true;
			RobotEditor.Instance.Robot.ResetActEnds();
			if (IsRod)
				Rod.SetEnd(From, EndId);
			else
				P1.move(From, false, true);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public ActionTypes Type() { return ActionTypes.MOVE_PIECE; }
	}
}
