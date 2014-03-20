﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionChooseSet : IAction
	{
		static public bool value = true;
		public void init() { value = !value;}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && (Input.justPressed(Keys.A)));
		}

		public bool run()
		{
			return (false);
		}

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.CHOOSE_SET; }
	}
}
