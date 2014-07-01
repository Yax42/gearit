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
		private bool _isChunk;
		private Trigger _trigger;
		private PolygonChunk _chunk;
		private int _verticeId;
		private Vector2 _from;
		private Vector2 _to;
		
		public void init()
		{
			_isChunk = !ActionSwapEventMode.EventMode;
			if (_isChunk)
			{
				Debug.Assert(MapEditor.Instance.SelectChunk.GetType() == typeof(PolygonChunk));
				_chunk = (PolygonChunk)MapEditor.Instance.SelectChunk;
				_verticeId = _chunk.findVertice(Input.SimMousePos);
				_from = _chunk.getVertice(_verticeId);
			}
			else
			{
				_trigger = MapEditor.Instance.SelectTrigger;
				_verticeId = _trigger.GetCloseCornerId(Input.SimMousePos);
				_from = _trigger.Corner(_verticeId);
			}
			_didRevert = false;
			_to = _from;
		}

		public bool shortcut()
		{
			return Input.ctrlAltShift(false, false, true) &&
				Input.justPressed(MouseKeys.RIGHT) &&
				MapEditor.Instance.SelectChunk.GetType() == typeof(PolygonChunk);
		}

		public bool run()
		{
			if (!_didRevert)
				_to = Input.SimMousePos;
			if (_isChunk)
				_chunk.moveVertice(_to, _verticeId);
			else
				_trigger.MoveCorner(_to, _verticeId);

			if (_didRevert)
				return false;
			return Input.pressed(MouseKeys.RIGHT);
		}

		public void revert()
		{
			_didRevert = true;
			if (_isChunk)
				_chunk.moveVertice(_from, _verticeId);
			else
				_trigger.MoveCorner(_from, _verticeId);
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return true; }

		public ActionTypes type() { return ActionTypes.RESIZE_POLYGON; }
	}
}
