using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionHide : IAction
	{
		public void init()
		{
			RobotEditor.Instance.Select1.Shown = !RobotEditor.Instance.Select1.Shown;
		}

		public bool shortcut()
		{
			return (Input.justPressed(Keys.E));
		}

		public bool run() { return false; }

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.HIDE; }
	}
}
