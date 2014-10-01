﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.map.action
{
	class ActionUndo : IAction
	{
		public void init()
		{
			MapEditor.Instance.undo();
		}

		public bool shortcut()
		{
			return (Input.CtrlAltShift(true, false, false) && Input.justPressed(Keys.Z));
		}

		public bool run() { return false; }
		public void revert() { }

		public bool canBeReverted() { return false; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.UNDO; }
	}
}
