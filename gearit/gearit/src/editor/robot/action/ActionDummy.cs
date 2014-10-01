using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.editor.robot.action
{
	class ActionDummy : IAction
	{
		public void init() { }

		public bool shortcut() { return false; }

		public bool run() { return false; }
		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.NONE; }
	}
}
