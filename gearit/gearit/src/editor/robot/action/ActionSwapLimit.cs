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
		ISpot Spot;
		public void init()
		{
			Piece p1 = RobotEditor.Instance.Select1;
			Piece p2 = RobotEditor.Instance.Select2;
			if (p1.isConnected(p2))
				Spot = p1.getConnection(p2);
			else
				Spot = null;
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, true) && (Input.justPressed(Keys.F)));
		}

		public bool run()
		{
			if (Spot != null && Spot.GetType() == typeof(RevoluteSpot))
				((RevoluteSpot)Spot).LimitEnabled = !((RevoluteSpot)Spot).LimitEnabled;
			return (false);
		}

		public void revert()
		{
			run();
		}

		public bool canBeReverted()
		{
			return (Spot != null && Spot.GetType() == typeof(RevoluteSpot));
		}

		public ActionTypes type() { return ActionTypes.SWAP_LIMIT; }
	}
}
