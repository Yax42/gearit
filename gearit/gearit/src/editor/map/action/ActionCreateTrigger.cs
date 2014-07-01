﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionCreateTrigger : IAction
	{
		private Trigger _trigger;

		public void init()
		{
			_trigger = new Trigger(Input.SimMousePos);
			MapEditor.Instance.SelectTrigger = _trigger;
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			return Input.justPressed(Keys.W)
				&& Input.ctrlAltShift(false, false, false);
		}

		public bool run()
		{
			MapEditor.Instance.Map.Triggers.Add(_trigger);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Triggers.Remove(_trigger);
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.CREATE_TRIGGER; }
	}
}