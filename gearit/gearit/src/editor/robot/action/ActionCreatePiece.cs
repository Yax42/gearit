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

		public void init()
		{
			Piece select1 = RobotEditor.Instance.Select1;
			Pack = new SleepingPack();
			HasBeenRevert = false;
			if (ActionChooseSet.IsWheel)
				P1 = new Wheel(RobotEditor.Instance.Robot, 0.5f, Input.SimMousePos);
			else
				P1 = new Rod(RobotEditor.Instance.Robot, 2, Input.SimMousePos);
			IsPrismatic = ActionChooseSet.IsPrismatic;

			Vector2 anchor = Vector2.Zero;
			if (select1.isOn(Input.SimMousePos))
				anchor = select1.GetLocalPoint(Input.SimMousePos);
			if (IsPrismatic)
				new PrismaticSpot(RobotEditor.Instance.Robot, select1, P1, anchor, Vector2.Zero);
			else
				new RevoluteSpot(RobotEditor.Instance.Robot, select1, P1, anchor, Vector2.Zero);
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
