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
#if false
		private bool IsOk;
		private Piece P1;
		private Piece P2;
		private ISpot Spot;
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


		public bool run()
		{
			if (IsOk)
			{
				Debug.Assert(!P1.isConnected(P2));
				Debug.Assert(P1 != P2);
				if (Spot == null)
				{
					Spot = new RevoluteSpot(RobotEditor.Instance.Robot, P1, P2);
				}
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
				RobotEditor.Instance.fallAsleep(Spot, Pack);
				P2.move(From);
			}
		}
#endif
		private Piece P1;
		private bool HasBeenRevert;
		private SleepingPack Pack;
		private int FrameCount;
		private bool IsOk;

		public void init()
		{
			FrameCount = 0;
			Piece select1 = RobotEditor.Instance.Select1;
			Piece select2 = RobotEditor.Instance.Select2;
			Pack = new SleepingPack();
			HasBeenRevert = false;

			if (select1 == select2)
			{
				IsOk = false;
				return;
			}
			else
			{
				IsOk = true;
			}

			Vector2 anchor2;
			Vector2 anchor1;
			P1 = new Rod(RobotEditor.Instance.Robot, 2);
			anchor1 = GetAnchor(select1);
			anchor2 = GetAnchor(select2);
			if (Vector2.Distance(anchor1, anchor2) < 0.1f)
			{
				IsOk = false;
				return;
			}
			var r1 = new RevoluteSpot(RobotEditor.Instance.Robot, select1, P1, select1.GetLocalPoint(anchor1), new Vector2(2, 0));
			var r2 = new RevoluteSpot(RobotEditor.Instance.Robot, select2, P1, select2.GetLocalPoint(anchor2), new Vector2(-2, 0));
			((Rod)P1).GenerateEndFromAnchor(r2, r1);
			RobotEditor.Instance.Select1 = P1;
		}

		private Vector2 GetAnchor(Piece p)
		{
			if (p.GetType() == typeof(Rod))
			{
				Rod r = (Rod)p;
				Vector2 anchor = r.GetEnd(r.CloseEnd(Input.SimMousePos));
				return anchor;
			}
			else
				return Vector2.Zero;

		}


		public bool run()
		{
			if (!IsOk)
				return false;
			if (HasBeenRevert)
			{
				RobotEditor.Instance.Robot.wakeUp(Pack);
			}
			return false;
		}

		public void revert()
		{
			HasBeenRevert = true;
			RobotEditor.Instance.fallAsleep(P1, Pack);
		}

		public bool shortcut()
		{
			return (Input.CtrlShift(false, true) && Input.justPressed(Keys.W));
		}

		public bool canBeReverted { get { return IsOk; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.LINK; }
	}
}
