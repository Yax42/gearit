using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.map.action
{
	class ActionSwapEventMode : IAction
	{
		public static bool EventMode = false;
		public void init()
		{
			EventMode = !EventMode;
		}

		public bool shortcut()
		{
			return (Input.CtrlShift(false, false) && Input.justPressed(Keys.Space));
		}

		public bool run() { return false; }

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }

		public bool actOnSelect() { return false; }

		public ActionTypes Type() { return ActionTypes.SWAP_EVENT_MODE; }
	}
}
