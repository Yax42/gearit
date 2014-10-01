using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
	class ActionDeletePiece : IAction
	{
		private SleepingPack Pack;
		private Piece P1;

		public void init()
		{
			Pack = new SleepingPack();
			P1 = RobotEditor.Instance.Select1;
		}

		public bool shortcut()
		{
			return (Input.CtrlShift(false, false) && (Input.justPressed(Keys.Delete) || Input.justPressed(Keys.Back) || Input.justPressed(Keys.R)));
		}

		public bool run()
		{
			Console.WriteLine("ActionDeletePiece");
			Console.WriteLine("Was connected to heart: " + RobotEditor.Instance.Robot.IsPieceConnectedToHeart(P1));
			if (P1 != RobotEditor.Instance.Robot.Heart)
			{
				RobotEditor.Instance.fallAsleep(P1, Pack); //Select sont checkes dans le fallAsleep
			}
			return (false);
		}

		public void revert()
		{
			RobotEditor.Instance.Robot.wakeUp(Pack);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.DELETE_PIECE; }
	}
}
