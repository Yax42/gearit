using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.map.action
{
	class ActionMove : IAction
	{
		private bool _didRevert;
		private MapChunk _chunk;
		private Vector2 _from;
		private Vector2 _to;

		public void init()
		{
			_chunk = MapEditor.Instance.Select;
			_from = _chunk.Position;
			_to = _from;
			_didRevert = false;
		}

		public bool shortcut()
		{
			return Input.pressed(MouseKeys.RIGHT)
				&& Input.ctrlAltShift(false, false, false);
		}

		public bool run()
		{
			if (!_didRevert)
				_to = Input.SimMousePos;
			_chunk.Position = _to;

			if (_didRevert)
				return false;
			return Input.pressed(MouseKeys.RIGHT);
		}

		public void revert()
		{
			_didRevert = true;
			_chunk.Position = _from;
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return true; }

		public ActionTypes type() { return ActionTypes.MOVE; }
	}
}
