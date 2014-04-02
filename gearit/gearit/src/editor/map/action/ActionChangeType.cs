using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using gearit.src.utility;

namespace gearit.src.editor.map.action
{
	class ActionChangeType : IAction
	{
		public void init()
		{
			if (MapEditor.Instance.Select.BodyType == BodyType.Static)
				MapEditor.Instance.Select.BodyType = BodyType.Dynamic;
			else
				MapEditor.Instance.Select.BodyType = BodyType.Static;
		}

		public bool shortcut()
		{
			return Input.ctrlAltShift(false, false, false) &&
				Input.justPressed(Keys.A);
		}

		public bool run()
		{
			return false;
		}

		public ActionTypes type() { return ActionTypes.CHANGE_TYPE; }
	}
}
