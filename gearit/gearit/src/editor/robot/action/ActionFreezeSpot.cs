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
			return (Input.CtrlShift(true, false) && (Input.justPressed(Keys.Q)));
		}

		public bool run()
		{
			if (_isOK)
				_spot.Frozen = !_spot.Frozen;
			return false;
		}

		public void revert() { run(); }

		public bool canBeReverted { get { return true; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.LIMIT_FROZEN; }
	}
}