using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot.action
{
	class ActionCreatePiece : IAction
	{
		private Piece P1;
		private bool HasBeenRevert;
		private SleepingPack Pack;
		private bool IsPrismatic;
		private int FrameCount;

		public void init()
		{
			FrameCount = 0;
			Piece select1 = RobotEditor.Instance.Select1;
			Pack = new SleepingPack();
			HasBeenRevert = false;
			Vector2 anchor2 = Vector2.Zero;
			if (ActionChooseSet.IsWheel)
				P1 = new Wheel(RobotEditor.Instance.Robot, 0.5f, Input.SimMousePos);
			else
			{
				P1 = new Rod(RobotEditor.Instance.Robot, 2, Input.SimMousePos);
				anchor2 = new Vector2(-2, 0);
			}
			IsPrismatic = ActionChooseSet.IsPrismatic;

			Vector2 anchor1;
			if (select1.isOn(Input.SimMousePos))
				anchor1 = new Vector2(select1.GetLocalPoint(Input.SimMousePos).X, 0);
			else
				anchor1 = select1.ShapeLocalOrigin();

			if (IsPrismatic)
				new PrismaticSpot(RobotEditor.Instance.Robot, select1, P1, anchor1, anchor2);
			else
			{
				new RevoluteSpot(RobotEditor.Instance.Robot, select1, P1, anchor1, anchor2);
			}
			RobotEditor.Instance.Select1 = P1;
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && Input.justPressed(Keys.W));
		}

		public bool run()
		{
			if (HasBeenRevert)
			{
				RobotEditor.Instance.Robot.wakeUp(Pack);
			}
			else if (IsPrismatic)
			{
				P1.move(Input.SimMousePos);
				return (Input.pressed(Keys.W));// && !Input.justPressed(MouseKeys.LEFT));
			}
			else if (!ActionChooseSet.IsWheel)
			{
				FrameCount++;
				if (FrameCount < 2)
					return true;
				else if (FrameCount == 2)
					((Rod)P1).GenerateEnds();
				((Rod)P1).setEnd(Input.SimMousePos, false);
				return Input.pressed(Keys.W);
			}
			return false;
		}

		public void revert()
		{
			HasBeenRevert = true;
			RobotEditor.Instance.fallAsleep(P1, Pack);
		}

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.CREATE_PIECE; }
	}
}
