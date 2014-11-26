using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionSetVirtualItemId : IAction
	{
		private IVirtualItem _item;
		private int _from;
		private int _to;

		public void init()
		{
			_item = MapEditor.Instance.SelectVirtualItem;
			_from = _item.Id;
			int delta = (Input.justPressed(MouseKeys.LEFT) ? 1 : -1);
			_to = _from + delta;
			if (_item.GetType() == typeof(Artefact))
			{
				_to = MapEditor.Instance.Map.GetArtefactFreeId(_to, delta);
				if (_to < 0)
					_to = _from;
			}
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			return (Input.justPressed(MouseKeys.LEFT)
				|| Input.justPressed(MouseKeys.RIGHT))
				&& Input.CtrlShift(true, false);
		}

		public bool run()
		{
			_item.Id = _to;
			return false;
		}

		public void revert()
		{
			_item.Id = _from;
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return true; }

		public ActionTypes Type() { return ActionTypes.SET_VIRTUAL_ITEM_ID; }
	}
}

