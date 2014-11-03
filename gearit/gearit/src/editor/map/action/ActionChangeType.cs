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
			if (MapEditor.Instance.SelectChunk.BodyType == BodyType.Static)
				MapEditor.Instance.SelectChunk.BodyType = BodyType.Dynamic;
			else
				MapEditor.Instance.SelectChunk.BodyType = BodyType.Static;
		}

		public bool shortcut()
		{
			if (ActionSwapEventMode.EventMode)
				return false;
			return Input.CtrlShift(false, false) &&
				Input.justPressed(Keys.A);
		}

		public bool run()
		{
			return false;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return true; } }

		public bool actOnSelect() { return false; }

		public ActionTypes Type() { return ActionTypes.CHANGE_TYPE; }
	}
}
