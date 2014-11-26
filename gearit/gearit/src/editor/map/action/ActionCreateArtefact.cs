
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionCreateArtefact : IAction
	{
		private Artefact _artefact;
		private int _id;
		public void init()
		{
			_id = 0;
			bool ok  = false;
			_artefact = new Artefact(Input.VirtualSimMousePos, MapEditor.Instance.Map.NextArtefactFreeId(0));
			MapEditor.Instance.SelectVirtualItem = _artefact;
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			return Input.justPressed(Keys.E)
				&& Input.CtrlShift(false, false);
		}

		public bool run()
		{
			MapEditor.Instance.Map.Artefacts.Add(_artefact);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Artefacts.Remove(_artefact);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return false; }

		public ActionTypes Type() { return ActionTypes.CREATE_ARTEFACT; }
	}
}