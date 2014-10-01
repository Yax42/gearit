using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
	class ActionDeleteSpot : IAction
	{
		private SleepingPack Pack;
		private RevoluteSpot Spot;
		private Piece P1;
		private Piece P2;

		public void init()
		{
			Pack = new SleepingPack();
			P1 = RobotEditor.Instance.Select1;
			P2 = RobotEditor.Instance.Select2;
			if (P1.isConnected(P2))
				Spot = P1.getConnection(P2);
			else
				Spot = null;
		}

		public bool shortcut()
		{
			return (Input.CtrlAltShift(false, false, true) && (Input.justPressed(Keys.Delete) || Input.justPressed(Keys.Back) || Input.justPressed(Keys.R)));
		}

		public bool run()
		{
			if (Spot != null)
			{
				Debug.Assert(P1.isConnected(P2));
				Debug.Assert(P1.getConnection(P2) == Spot);
				RobotEditor.Instance.fallAsleep(Spot, Pack);
			}
			return (false);
		}

		public void revert()
		{
			Debug.Assert(!P1.isConnected(P2));
			RobotEditor.Instance.Robot.wakeUp(Pack);
		}

		public bool canBeReverted { get { return (Spot != null); } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.DELETE_SPOT; }
	}
}
