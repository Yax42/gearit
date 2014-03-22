using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot.action
{
	class ActionRevLink : IAction
	{
		private bool IsOk;
		private Piece P1;
		private Piece P2;
		private RevoluteSpot Spot;
		private SleepingPack Pack;
		private Vector2 From;
		private Vector2 To;

		public void init()
		{
			Pack = new SleepingPack();
			P1 = RobotEditor.Instance.Select1;
			P2 = RobotEditor.Instance.Select2;

			if (P1.isConnected(P2) || P1 == P2)
				IsOk = false;
			else
			{
				From = P2.Position;
				To = P1.Position;
				IsOk = true;
				Spot = null;
			}
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, true) && Input.justPressed(Keys.Q));
		}

		public bool run()
		{
			if (IsOk)
			{
				Debug.Assert(!P1.isConnected(P2));
				Debug.Assert(P1 != P2);
				if (Spot == null)
					Spot = new RevoluteSpot(RobotEditor.Instance.Robot, P1, P2);
				else
				{
					RobotEditor.Instance.Robot.wakeUp(Pack);
					P2.move(To);
				}
			}
			return (false);
		}

		public void revert()
		{
			if (IsOk && Spot != null)
			{
				RobotEditor.Instance.Robot.fallAsleep(Spot, Pack);
				P2.move(From);
			}
		}

		public bool canBeReverted() { return IsOk; }

		public ActionTypes type() { return ActionTypes.REV_LINK; }
	}
}
