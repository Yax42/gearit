﻿using System;
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
		private int FrameCount;

		public void init()
		{
			FrameCount = 0;
			Piece select1 = RobotEditor.Instance.Select1;
			Pack = new SleepingPack();
			HasBeenRevert = false;
			Vector2 anchor2 = Vector2.Zero;
			if (ActionChooseSet.IsWheel)
				P1 = new Wheel(RobotEditor.Instance.Robot, 0.5f, Input.VirtualSimMousePos);
			else
			{
				P1 = new Rod(RobotEditor.Instance.Robot, 2, Input.VirtualSimMousePos);
				anchor2 = new Vector2(-2, 0);
			}

			Vector2 anchor1;
			if (Input.CtrlShift(true, false))
				anchor1 = select1.GetLocalPoint(select1.ClosestPositionInside(Input.VirtualSimMousePos));
			else if (select1.Contain(Input.VirtualSimMousePos))
				anchor1 = select1.GetLocalPoint(Input.VirtualSimMousePos);
			else
				anchor1 = select1.ShapeLocalOrigin();

			new RevoluteSpot(RobotEditor.Instance.Robot, select1, P1, anchor1, anchor2);
			RobotEditor.Instance.Select1 = P1;
		}

		public bool shortcut()
		{
			return ((Input.CtrlShift(false, false)
				|| Input.CtrlShift(true, false))
				&& Input.justPressed(Keys.W));
		}

		public bool run()
		{
			if (HasBeenRevert)
			{
				RobotEditor.Instance.Robot.wakeUp(Pack);
			}
			else if (P1.GetType() == typeof(Rod))
			{
				RobotEditor.Instance.Robot.ResetActEnds();
				FrameCount++;
				if (FrameCount < 2)
					return true;
				else if (FrameCount == 2)
					((Rod)P1).GenerateEnds();
				((Rod)P1).SetEnd(Input.VirtualSimMousePos, false);
				return Input.pressed(Keys.W);
			}
			return false;
		}

		public void revert()
		{
			HasBeenRevert = true;
			RobotEditor.Instance.fallAsleep(P1, Pack);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.CREATE_PIECE; }
	}
}
