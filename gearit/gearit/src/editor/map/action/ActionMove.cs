using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionMove : IAction
	{
		private bool _didRevert;
		private MapChunk _chunk;
		private IVirtualItem _item;
		private bool _isChunk;
		private Vector2 _from;
		private Vector2 _to;

		public void init()
		{
			_isChunk = !ActionSwapEventMode.EventMode;
			if (_isChunk)
			{
				_chunk = MapEditor.Instance.SelectChunk;
				_from = _chunk.Position;
			}
			else
			{
				_item = MapEditor.Instance.SelectVirtualItem;
				_from = _item.Position;
			}
			_to = _from;
			_didRevert = false;
		}

		public bool shortcut()
		{
			return Input.justPressed(MouseKeys.RIGHT)
				&& Input.CtrlShift(false, false);
		}

		public bool run()
		{
			if (!_didRevert)
				_to = Input.VirtualSimMousePos;
			if (_isChunk)
				_chunk.Position = _to;
			else
				_item.Position = _to;

			if (_didRevert)
				return false;
			return Input.pressed(MouseKeys.RIGHT);
		}

		public void revert()
		{
			_didRevert = true;
			if (_isChunk)
				_chunk.Position = _from;
			else
				_item.Position = _from;
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return true; }

		public ActionTypes Type() { return ActionTypes.MOVE; }
	}
}
