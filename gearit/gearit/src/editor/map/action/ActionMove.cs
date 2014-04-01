using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.map.action
{
	class ActionMove : IAction
	{
		public void init()
		{
		}

		public bool shortcut()
		{
			return Input.pressed(MouseKeys.RIGHT)
				&& Input.ctrlAltShift(false, false, false);
		}

		public bool run()
		{
			MapEditor.Instance.Select.Position = Input.SimMousePos;
			return Input.pressed(MouseKeys.RIGHT);
		}

		public ActionTypes type() { return ActionTypes.MOVE; }
	}
}
