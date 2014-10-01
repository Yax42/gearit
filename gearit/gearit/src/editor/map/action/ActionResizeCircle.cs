using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using gearit.src.map;
using System.Diagnostics;

namespace gearit.src.editor.map.action
{
	class ActionResizeCircle : IAction
	{
		private bool _didRevert;
		private CircleChunk _chunk;
		private float _from;
		private float _to;
		
		public void init()
		{
			Debug.Assert(MapEditor.Instance.SelectChunk.GetType() == typeof(CircleChunk));
			_chunk = (CircleChunk)MapEditor.Instance.SelectChunk;
			_didRevert = false;
			_from = _chunk.Size;
			_to = _from;
		}

		public bool shortcut()
		{
			if (ActionSwapEventMode.EventMode)
				return false;
			return Input.CtrlAltShift(false, false, true) &&
				Input.justPressed(MouseKeys.RIGHT) &&
				MapEditor.Instance.SelectChunk.GetType() == typeof(CircleChunk);
		}

		public bool run()
		{
			if (!_didRevert)
			{
				_to = (_chunk.Position - Input.SimMousePos).Length();
				if (_to == 0)
					_to = 0.1f;
			}
			_chunk.Size = _to;
			if (_didRevert)
				return false;
			return Input.pressed(MouseKeys.RIGHT);
		}

		public void revert()
		{
			_didRevert = true;
			_chunk.Size = _from;
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return true; }

		public ActionTypes type() { return ActionTypes.RESIZE_CIRCLE; }
	}
}
