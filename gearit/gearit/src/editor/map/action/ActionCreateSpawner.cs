
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionCreateSpawner : IAction
	{
		private Spawner _Spawner;
		public void init()
		{
			_Spawner = new Spawner(Input.SimMousePos);
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
			MapEditor.Instance.Map.Spawners.Add(_Spawner);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Spawners.Remove(_Spawner);
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.CREATE_WALL; }
	}
}