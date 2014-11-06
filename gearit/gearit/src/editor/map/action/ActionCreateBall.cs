using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionCreateBall : IAction
	{
		private MapChunk _chunk;

		public void init()
		{
			_chunk = new CircleChunk(MapEditor.Instance.World, true, Input.VirtualSimMousePos);
			MapEditor.Instance.SelectChunk = _chunk;
		}

		public bool shortcut()
		{
			if (ActionSwapEventMode.EventMode)
				return false;
			return Input.justPressed(Keys.W)
				&& Input.CtrlShift(false, true);
		}

		public bool run()
		{
			MapEditor.Instance.Map.Chunks.Add(_chunk);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.Chunks.Remove(_chunk);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return false; }

		public ActionTypes Type() { return ActionTypes.CREATE_BALL; }
	}
}
