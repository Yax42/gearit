using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gearit.src.editor.map.action
{
	class ActionDummy : IAction
	{
		public void init() { }

		public bool shortcut() { return false; }

		public bool run() { return false; }

		public ActionTypes type() { return ActionTypes.NONE; }
	}
}
