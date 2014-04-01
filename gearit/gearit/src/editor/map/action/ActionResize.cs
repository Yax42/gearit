using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using gearit.src.map;

namespace gearit.src.editor.map.action
{
	class ActionResize : IAction
	{
		public void init()
		{
			((PolygonChunk)MapEditor.Instance.Select).selectVertice(Input.SimMousePos);
		}

		public bool shortcut()
		{
			return Input.ctrlAltShift(false, false, true) &&
				Input.justPressed(MouseKeys.RIGHT) &&
				MapEditor.Instance.Select.GetType() == typeof(PolygonChunk);
		}

		public bool run()
		{
			MapEditor.Instance.Select.resize(Input.SimMousePos); /*(Input.SimMousePos - ;Select.Position) + Input.SimMousePos*/
			return Input.pressed(MouseKeys.RIGHT);
		}

		public ActionTypes type() { return ActionTypes.RESIZE; }
	}
}
