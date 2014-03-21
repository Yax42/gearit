using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionDeletePiece : IAction
	{
		private Piece P1;

		public void init()
		{
			P1 = RobotEditor.Instance.Select1;
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && (Input.justPressed(Keys.Delete) || Input.justPressed(Keys.Back) || Input.justPressed(Keys.R)));
		}

		public bool run()
		{
			Console.WriteLine("ActionDeletePiece");
			Console.WriteLine("Was connected to heart: " + RobotEditor.Instance.Robot.IsPieceConnectedToHeart(P1));
			if (P1 != RobotEditor.Instance.Robot.getHeart())
			{
				//This line is behaves correctly with Ctrl Z but no recursive
				//RobotEditor.Instance.remove(P1);
				//This part has no Ctrl Z but is recursive
				if (RobotEditor.Instance.Select2 == P1)
					RobotEditor.Instance.Select2 = RobotEditor.Instance.Robot.getHeart();
				RobotEditor.Instance.Robot.RecursiveRemove(P1);
				RobotEditor.Instance.Select1 = RobotEditor.Instance.Robot.getHeart();
			}
			return (false);
		}

		public void revert()
		{
			P1.BackIntoWorld(RobotEditor.Instance.Robot);
		}

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.DELETE_PIECE; }
	}
}
