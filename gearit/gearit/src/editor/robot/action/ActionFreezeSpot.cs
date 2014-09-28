using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionFreezeSpot : IAction
	{
		private bool _isOK;
		private RevoluteSpot _spot;
		public void init()
		{
			_spot = RobotEditor.Instance.SelectedSpot;
			_isOK = (_spot != null);
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(true, false, false) && (Input.justPressed(Keys.Q)));
		}

		public bool run()
		{
			if (_isOK)
				_spot.Frozen = !_spot.Frozen;
			return false;
		}

		public void revert() { run(); }

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.LIMIT_FROZEN; }
	}
}