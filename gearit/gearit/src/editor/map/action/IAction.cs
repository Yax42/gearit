using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.map.action
{
	interface IAction
	{
		bool canBeMirrored { get; }
		bool canBeReverted { get; }
		void init();
		bool shortcut();
		bool run();
		void revert();
		bool actOnSelect();
		ActionTypes Type();
	}
}
