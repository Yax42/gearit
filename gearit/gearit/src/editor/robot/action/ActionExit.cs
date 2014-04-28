﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using GUI;

namespace gearit.src.editor.robot.action
{
	class ActionExit : IAction
	{
		public void init() { }

		public bool shortcut() { return Input.Exit; }

		public bool run()
		{
			MenuRobotEditor.Instance.saveasRobot(true);
			return false;
		}
		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.EXIT; }
	}
}