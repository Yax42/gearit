using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using System.Diagnostics;
using gearit.src.utility;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.map.action
{
	class ActionResizePolygon : IAction
	{
		private bool _didRevert;
		private PolygonChunk _chunk;
		private int _verticeId;
		private Vector2 _from;
		private Vector2 _to;
		
		public void init()
		{
			Debug.Assert(MapEditor.Instance.Select.GetType() == typeof(PolygonChunk));
			_chunk = (PolygonChunk)MapEditor.Instance.Select;
			_verticeId = _chunk.findVertice(Input.SimMousePos);
			_didRevert = false;
			_from = _chunk.getVertice(_verticeId);
			_to = _from;
		}

		public bool shortcut()
		{
			return Input.ctrlAltShift(false, false, true) &&
				Input.justPressed(MouseKeys.RIGHT) &&
				MapEditor.Instance.Select.GetType() == typeof(PolygonChunk);
		}

		public bool run()
		{
			if (!_didRevert)
				_to = Input.SimMousePos;
			_chunk.moveVertice(_to, _verticeId);

			if (_didRevert)
				return false;
			return Input.pressed(MouseKeys.RIGHT);
		}

		public void revert()
		{
			_didRevert = true;
			_chunk.moveVertice(_from, _verticeId);
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return true; }

		public ActionTypes type() { return ActionTypes.RESIZE_POLYGON; }
	}
}
