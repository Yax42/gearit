using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionSwapSelects : IAction
	{
		public void init()
		{
			var tmp = RobotEditor.Instance.Select2;
			RobotEditor.Instance.Select2 = RobotEditor.Instance.Select1;
			RobotEditor.Instance.Select1 = tmp;
		}

		public bool shortcut()
		{
			return (Input.justPressed(Keys.C));
		}

		public bool run()
		{
			return (false);
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.SWAP_SELECT; }
	}
}
