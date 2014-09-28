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
#endif
		private Piece P1;
		private bool HasBeenRevert;
		private SleepingPack Pack;
		private bool IsPrismatic;
		private int FrameCount;
		private bool IsOk;

		public void init()
		{
			FrameCount = 0;
			Piece select1 = RobotEditor.Instance.Select1;
			Piece select2 = RobotEditor.Instance.Select2;
			Pack = new SleepingPack();
			HasBeenRevert = false;
			Vector2 anchor2 = Vector2.Zero;

			if (select2.isConnected(select1) || select1 == select2)
			{
				IsOk = false;
				return;
			}
			else
			{
				IsOk = true;
			}

			if (ActionChooseSet.IsWheel)
				P1 = new Wheel(RobotEditor.Instance.Robot, 0.5f);//, Input.SimMousePos);
			else
			{
				P1 = new Rod(RobotEditor.Instance.Robot, 2);//, Input.SimMousePos);
				anchor2 = new Vector2(-2, 0);
			}
			IsPrismatic = ActionChooseSet.IsPrismatic;

			Vector2 anchor1;
			if (select1.Contain(Input.SimMousePos))
				anchor1 = select1.GetLocalPoint(Input.SimMousePos);
			else
				anchor1 = select1.ShapeLocalOrigin();

			if (IsPrismatic)
			{
				//new PrismaticSpot(RobotEditor.Instance.Robot, select1, P1, anchor1, anchor2);
				//new PrismaticSpot(RobotEditor.Instance.Robot, select2, P1, Vector2.Zero, -anchor2);
			}
			else
			{
				new RevoluteSpot(RobotEditor.Instance.Robot, select1, P1, anchor1, anchor2);
				new RevoluteSpot(RobotEditor.Instance.Robot, select2, P1, Vector2.Zero, -anchor2);
			}
			RobotEditor.Instance.Select1 = P1;
		}

		public bool run()
		{
			if (!IsOk)
				return false;
			if (HasBeenRevert)
			{
				RobotEditor.Instance.Robot.wakeUp(Pack);
			}
			else if (!ActionChooseSet.IsWheel)
			{
				((Rod)P1).GenerateEnds();
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
			return (Input.ctrlAltShift(false, false, true) && Input.justPressed(Keys.W));
		}

		public bool canBeReverted() { return IsOk; }

		public ActionTypes type() { return ActionTypes.LINK; }
	}
}
