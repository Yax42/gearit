using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.map;
using System.Diagnostics;
using gearit.src.utility;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;

namespace gearit.src.editor.map.action
{
	class ActionResizePolygon : IAction
	{
		private bool _didRevert;
		private bool _isChunk;
		private Trigger _trigger;
		private PolygonChunk _chunk;
		private int _verticeId;
		private PolygonShape _fromShape;
		private PolygonShape _toShape;
		private Vector2 _from;
		private Vector2 _to;
		
		public void init()
		{
			_isChunk = !ActionSwapEventMode.EventMode;
			if (_isChunk)
			{
				Debug.Assert(MapEditor.Instance.SelectChunk.GetType() == typeof(PolygonChunk));
				_chunk = (PolygonChunk)MapEditor.Instance.SelectChunk;
				_verticeId = _chunk.findVertice(Input.VirtualSimMousePos);
				_fromShape = (PolygonShape)_chunk.FixtureList[0].Shape;
				_toShape = _fromShape;
				_from = Input.VirtualSimMousePos;
			}
			else
			{
				_trigger = MapEditor.Instance.SelectTrigger;
				_verticeId = _trigger.GetCloseCornerId(Input.VirtualSimMousePos);
				_from = _trigger.Corner(_verticeId);
			}
			_didRevert = false;
			_to = _from;
		}

		public bool shortcut()
		{
			return Input.CtrlShift(false, true) &&
				Input.justPressed(MouseKeys.RIGHT) &&
				!MapEditor.Instance.IsSelectDummy() &&
				(ActionSwapEventMode.EventMode ||
				MapEditor.Instance.SelectChunk.GetType() == typeof(PolygonChunk));
		}

		public bool run()
		{
			if (!_didRevert)
			{
				_to = Input.VirtualSimMousePos;
				if (_isChunk)
					_verticeId = _chunk.findVertice(Input.VirtualSimMousePos);
			}
			if (_isChunk)
				_chunk.moveVertice(_to, _verticeId);
			else
				_trigger.MoveCorner(_to, _verticeId);

			if (!_didRevert)
			{
				if (_isChunk)
					_toShape = (PolygonShape)_chunk.FixtureList[0].Shape;
				return Input.pressed(MouseKeys.RIGHT);
			}
			return false;
		}

		public void revert()
		{
			_didRevert = true;
			if (_isChunk)
			{
				_chunk.SetPolygon(_fromShape);
			}
			else
				_trigger.MoveCorner(_from, _verticeId);
		}

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return true; }

		public ActionTypes Type() { return ActionTypes.RESIZE_POLYGON; }
	}
}
