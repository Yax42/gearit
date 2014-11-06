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
			if (Input.Alt)
				MirrorAxis.Active = true;
			if (ActionSwapEventMode.EventMode)
			{
				MapEditor.Instance.SelectTrigger = MapEditor.Instance.Map.GetTrigger(Input.SimMousePos);
			}
			else
			{
				MapEditor.Instance.SelectChunk = MapEditor.Instance.Map.GetChunk(Input.SimMousePos);
			}
			if (Input.Alt)
				MirrorAxis.Active = false;
		}

		public bool shortcut()
		{
			return Input.CtrlShift(false, false)
				&& Input.justPressed(MouseKeys.LEFT);
		}

		public bool run() { return false; }

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }

		public bool actOnSelect() { return false; }

		public ActionTypes Type() { return ActionTypes.SELECT; }
	}
}
