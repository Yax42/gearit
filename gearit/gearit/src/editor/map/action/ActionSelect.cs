using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;

namespace gearit.src.editor.map.action
{
	class ActionSelect : IAction
	{
		public void init()
		{
			MapChunk chunk = MapEditor.Instance.Map.getChunk(Input.SimMousePos);
			if (chunk != null)
				MapEditor.Instance.Select = chunk;
			else
				MapEditor.Instance.Select = null;
		}

		public bool shortcut()
		{
			return Input.ctrlAltShift(false, false, false)
				&& Input.justPressed(MouseKeys.LEFT);
		}

		public bool run() { return false; }

		public ActionTypes type() { return ActionTypes.SELECT; }
	}
}
