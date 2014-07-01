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
	class ActionDeleteSpawner : IAction
	{
		private Spawner _spawner;

		public void init()
		{
			Spawner res = null;
			float lowest = 10;
			Vector2 pos = Input.SimMousePos;
			foreach (Spawner spawn in MapEditor.Instance.Map.Spawners)
			{
				float distance = (spawn.Position - pos).LengthSquared();
				if (distance < lowest)
				{
					res = spawn;
					lowest = distance;
				}
			}
			if (lowest > 0.15f)
				_spawner = null;
			else
				_spawner = res;
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
			if (_spawner != null)
				MapEditor.Instance.Map.Spawners.Remove(_spawner);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Spawners.Add(_spawner);
		}

		public bool canBeReverted() { return _spawner != null; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.DELETE_SPAWNER; }
	}
}
