using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionSetTriggerId : IAction
	{
		private Trigger _trigger;
		private int _from;
		private int _to;

		public void init()
		{
			_trigger = MapEditor.Instance.SelectTrigger;
			_from = _trigger.Id;
			_to = _from + (Input.justPressed(MouseKeys.LEFT) ? 1 : -1);
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			return (Input.justPressed(MouseKeys.LEFT)
				|| Input.justPressed(MouseKeys.RIGHT))
				&& Input.CtrlShift(true, false);
		}

		public bool run()
		{
			_trigger.Id = _to;
			return false;
		}

		public void revert()
		{
			_trigger.Id = _from;
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return true; }

		public ActionTypes Type() { return ActionTypes.SET_TRIGGER_ID; }
	}
}

