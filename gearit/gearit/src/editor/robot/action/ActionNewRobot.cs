using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.GUI;
using gearit.src.robot;

namespace gearit.src.editor.robot.action
{
	class ActionNewRobot : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			return (Input.CtrlShift(true, false) && (Input.justPressed(Keys.N)));
		}

		public bool run()
		{
			RobotEditor.Instance.resetRobot(new Robot(RobotEditor.Instance.World, true));
			return false;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.NEW_ROBOT; }
	}
}