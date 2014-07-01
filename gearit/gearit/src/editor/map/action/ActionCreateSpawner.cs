﻿
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
		private Artefact _Artefact;
		public void init()
		{
			_Artefact = new Artefact(Input.SimMousePos);
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			return Input.justPressed(Keys.E)
				&& Input.ctrlAltShift(false, false, false);
		}

		public bool run()
		{
			MapEditor.Instance.Map.Artefacts.Add(_Artefact);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Artefacts.Remove(_Artefact);
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.CREATE_WALL; }
	}
}