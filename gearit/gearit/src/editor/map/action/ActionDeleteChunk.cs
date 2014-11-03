using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.map.action
{
	class ActionDeleteChunk : IAction
	{
		private MapChunk _chunk;
		public void init()
		{
			_chunk = MapEditor.Instance.SelectChunk;
			MapEditor.Instance.SelectChunk = null;
		}

		public bool shortcut()
		{
			if (ActionSwapEventMode.EventMode)
				return false;
			return Input.justPressed(Keys.R)
				&& Input.CtrlShift(false, false);
		}

		public bool run()
		{
			MapEditor.Instance.Map.Chunks.Remove(_chunk);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Chunks.Add(_chunk);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return true; }

		public ActionTypes Type() { return ActionTypes.DELETE_CHUNK; }
	}
}
