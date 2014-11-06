using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.editor.map;

namespace gearit.src.editor.map.action
{
	class ActionShowHelp : IAction
	{
		public void init()
		{
			MenuMapEditor.Instance.swapHelp();
		}

		public bool shortcut()
		{
			return Input.justPressed(Keys.F1);
		}

		public bool run() { return false; }
		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }

		public bool actOnSelect() {return false; }

		public ActionTypes Type() { return ActionTypes.SHOW_HELP; }
	}
}
