using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.map.action
{
	class ActionRedo : IAction
	{
		public void init()
		{
			MapEditor.Instance.redo();
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(true, false, false) && Input.justPressed(Keys.Y));
		}

		public bool run() { return false; }

		public void revert() { }

		public bool canBeReverted() { return false; }

		public bool actOnSelect() { return false; }

		public ActionTypes type() { return ActionTypes.REDO; }
	}
}
