using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.GUI;

namespace gearit.src.editor.robot.action
{
	class ActionShowHelp : IAction
	{
		public void init()
		{
			MenuRobotEditor.Instance.swapHelp();
		}

		public bool shortcut()
		{
			return Input.justPressed(Keys.F1);
		}

		public bool run() { return false; }
		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.SHOW_HELP; }
	}
}
