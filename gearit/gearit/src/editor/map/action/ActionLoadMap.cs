﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using gearit.src.output;

namespace gearit.src.editor.map.action
{
	class ActionLoadMap : IAction
	{
		public void init() { }

		public bool shortcut()
		{
			if (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.D)))
				;// MenuMapEditor.Instance.saveRobot();
			return false;
		}

		public bool run()
		{
			/*
			Debug.Assert(RobotEditor.Instance.NamePath != "");
			Robot robot = (Robot)Serializer.DeserializeItem("robot/" + RobotEditor.Instance.NamePath + ".gir");
			if (robot == null)
			{
				RobotEditor.Instance.resetNamePath();
				return false;
			}
			RobotEditor.Instance.resetRobot(robot);
			RobotEditor.Instance.Select1 = RobotEditor.Instance.Robot.getHeart();
			RobotEditor.Instance.Select2 = RobotEditor.Instance.Robot.getHeart();
			*/
			return false;
		}

		public void revert() { }

		public bool canBeReverted() { return false; }
		
		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.LOAD; }
	}
}