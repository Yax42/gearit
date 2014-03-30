using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.map.action
{
	interface IAction
	{
		bool canBeReverted();
		void init();
		bool shortcut();
		bool run();
		void revert();
		ActionTypes type();
	}
}
