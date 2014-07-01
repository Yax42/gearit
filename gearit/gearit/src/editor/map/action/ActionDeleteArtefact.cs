using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.map;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.map.action
{
	class ActionDeleteArtefact : IAction
	{
		private Artefact _artefact;

		public void init()
		{
			Artefact res = null;
			float lowest = 10;
			Vector2 pos = Input.SimMousePos;
			foreach (Artefact spawn in MapEditor.Instance.Map.Artefacts)
			{
				float distance = (spawn.Position - pos).LengthSquared();
				if (distance < lowest)
				{
					res = spawn;
					lowest = distance;
				}
			}
			if (lowest > 0.15f)
				_artefact = null;
			else
				_artefact = res;
		}

		public bool shortcut()
		{
			if (!ActionSwapEventMode.EventMode)
				return false;
			return Input.justPressed(Keys.R)
				&& Input.ctrlAltShift(false, false, false);
		}

		public bool run()
		{
			if (_artefact != null)
				MapEditor.Instance.Map.Artefacts.Remove(_artefact);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Artefacts.Add(_artefact);
		}

		public bool canBeReverted() { return _artefact != null; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.DELETE_ARTEFACT; }
	}
}
