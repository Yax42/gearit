using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace gearit.src.editor.robot.action
{
	class ActionPrisLink : IAction
	{
		private bool IsOk;
		private Piece P1;
		private Piece P2;
		private PrismaticSpot Spot;
		private SleepingPack Pack;

		public void init()
		{
			Pack = new SleepingPack();
			if (P1.isConnected(P2) || P1 == P2)
				IsOk = false;
			else
			{
				IsOk = true;
				P1 = RobotEditor.Instance.Select1;
				P2 = RobotEditor.Instance.Select2;
				Spot = null;
			}
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, true) && Input.justPressed(Keys.W));
		}

		public bool run()
		{
			if (IsOk)
			{
				Debug.Assert(!P1.isConnected(P2));
				Debug.Assert(P1 != P2);
				if (Spot == null)
					Spot = new PrismaticSpot(RobotEditor.Instance.Robot, P1, P2);
				else
					RobotEditor.Instance.Robot.wakeUp(Pack);
			}
			return (false);
		}

		public void revert()
		{
			if (IsOk && Spot != null)
				RobotEditor.Instance.Robot.fallAsleep(Spot, Pack);
		}

		public bool canBeReverted() { return IsOk; }

		public ActionTypes type() { return ActionTypes.PRIS_LINK; }
	}
}
