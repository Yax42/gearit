using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace gearit.src.editor.robot.action
{
	class ActionSwapLimit : IAction
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
			return (Input.ctrlAltShift(false, false, true) && (Input.justPressed(Keys.Q)));
		}

		public bool run()
		{
			if (_isOK)
				_spot.SpotLimitEnabled = !_spot.SpotLimitEnabled;
			return false;
		}

		public void revert() { run(); }

		public bool canBeReverted() { return true; }

		public ActionTypes type() { return ActionTypes.SWAP_LIMIT; }
	}
}
