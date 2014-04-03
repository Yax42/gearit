using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionCreate : IAction
	{
		private MapChunk _chunk;

		public void init()
		{

			if (Input.ctrlAltShift(false, false, true))
				_chunk = new CircleChunk(MapEditor.Instance.World, true, Input.SimMousePos);
			else
				_chunk = new PolygonChunk(MapEditor.Instance.World, false, Input.SimMousePos);
			MapEditor.Instance.Select = _chunk;
		}

		public bool shortcut()
		{
			return Input.justPressed(Keys.W)
				&& (Input.ctrlAltShift(false, false, false)
				|| Input.ctrlAltShift(false, false, true));
		}

		public bool run()
		{
			MapEditor.Instance.Map.addChunk(_chunk);
			return false;
		}

		public void revert()
		{
			MapEditor.Instance.Map.deleteChunk(_chunk);
		}

		public bool canBeReverted() { return true; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.CREATE; }
	}
}
