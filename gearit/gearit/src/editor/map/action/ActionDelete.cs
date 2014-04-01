using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.map.action
{
	class ActionDelete : IAction
	{
		public void init()
		{
			MapEditor.Instance.Map.getChunks().Remove(MapEditor.Instance.Select);
			MapEditor.Instance.Select = null;
		}

		public bool shortcut()
		{
			return Input.justPressed(Keys.R)
				&& Input.ctrlAltShift(false, false, false);
		}

		public bool run() { return false; }

		public ActionTypes type() { return ActionTypes.DELETE; }
	}
}
