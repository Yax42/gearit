using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionDeleteVirtualItem : IAction
	{
		private IVirtualItem _item;
		public void init()
		{
			_item = MapEditor.Instance.SelectVirtualItem;
			MapEditor.Instance.SelectVirtualItem = null;
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			if (MapEditor.Instance.SelectVirtualItem == null)
				return false;
			return Input.justPressed(Keys.R)
				&& Input.CtrlShift(false, false);
		}

		public bool run()
		{
			if (_item.GetType() == typeof(Artefact))
				MapEditor.Instance.Map.Artefacts.Remove((Artefact)_item);
			else
				MapEditor.Instance.Map.Triggers.Remove((Trigger)_item);
			return false;
		}

		public void revert()
		{
			if (_item.GetType() == typeof(Artefact))
				MapEditor.Instance.Map.Artefacts.Add((Artefact)_item);
			else
				MapEditor.Instance.Map.Triggers.Add((Trigger)_item);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return true; }

		public ActionTypes Type() { return ActionTypes.DELETE_VIRTUAL_ITEM; }
	}
}