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
	class ActionLink : IAction
	{
		private bool IsOk;
		private Piece P1;
		private Piece P2;
		private ISpot Spot;
		private SleepingPack Pack;
		private Vector2 From;
		private Vector2 To;
		private bool IsPrismatic;

		public void init()
		{
			IsPrismatic = ActionChooseSet.IsPrismatic;
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
			return (Input.ctrlAltShift(false, false, true) && Input.justPressed(Keys.W));
		}

		public bool run()
		{
			if (IsOk)
			{
				Debug.Assert(!P1.isConnected(P2));
				Debug.Assert(P1 != P2);
				if (Spot == null)
				{
					if (IsPrismatic)
						Spot = new PrismaticSpot(RobotEditor.Instance.Robot, P1, P2);
					else
						Spot = new RevoluteSpot(RobotEditor.Instance.Robot, P1, P2);
				}
				else
				{
					RobotEditor.Instance.Robot.wakeUp(Pack);
					if (!IsPrismatic)
						P2.move(To);
				}
			}
			return (false);
		}

		public void revert()
		{
			if (IsOk && Spot != null)
			{
				RobotEditor.Instance.fallAsleep(Spot, Pack);
				if (!IsPrismatic)
					P2.move(From);
			}
		}

		public bool canBeReverted() { return IsOk; }

		public ActionTypes type() { return ActionTypes.LINK; }
	}
}
