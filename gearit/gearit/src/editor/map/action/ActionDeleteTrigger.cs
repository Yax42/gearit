using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionDeleteTrigger : IAction
	{
		private Trigger _trigger;
		public void init()
		{
			_trigger = MapEditor.Instance.SelectTrigger;
			MapEditor.Instance.SelectTrigger = null;
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			if (MapEditor.Instance.SelectTrigger == null)
				return false;
			return Input.justPressed(Keys.R)
				&& Input.CtrlAltShift(false, false, false)
				&& MapEditor.Instance.SelectTrigger.Contain(Input.SimMousePos);
		}

		public bool run()
		{
			MapEditor.Instance.Map.Triggers.Remove(_trigger);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Triggers.Add(_trigger);
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return true; }

		public ActionTypes type() { return ActionTypes.DELETE_TRIGGER; }
	}
}