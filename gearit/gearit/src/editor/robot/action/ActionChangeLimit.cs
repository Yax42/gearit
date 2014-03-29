using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using gearit.src.robot;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace gearit.src.editor.robot.action
{
	class ActionChangeLimit : IAction
	{
		private int _step;
		public void init() { _step = 0; }

		public bool shortcut()
		{
			return false;
			return (Input.ctrlAltShift(false, false, true) && (Input.justPressed(Keys.S)));
		}

		public bool run(ref Robot robot, ref Piece selected1, ref Piece selected2)
		{
			ISpot spot = selected1.getConnection(selected2);

			if (spot == null || spot.GetType() != typeof(RevoluteSpot))
				return (false);
			RevoluteSpot revSpot = (RevoluteSpot)spot;
			if (revSpot.LimitEnabled == false)
				return (false);
			if (_step == 0)
			{
				revSpot.LowerLimit = -(float)MathUtils.VectorAngle(Input.SimMousePos - revSpot.WorldAnchorA, new Vector2(1, 0));
				if (revSpot.LowerLimit > 0)
					revSpot.LowerLimit -= (float)Math.PI * 2;
				if (revSpot.UpperLimit - revSpot.LowerLimit > Math.PI * 2)
					revSpot.UpperLimit = (float)Math.PI * 2 + revSpot.LowerLimit;
			}
			else if (_step == 1)
			{
				revSpot.UpperLimit = -(float)MathUtils.VectorAngle(Input.SimMousePos - revSpot.WorldAnchorA, new Vector2(1, 0));
				if (revSpot.UpperLimit < 0)
					revSpot.UpperLimit += (float)Math.PI * 2;
				if (revSpot.UpperLimit - revSpot.LowerLimit > Math.PI * 2)
					revSpot.LowerLimit = -(float)Math.PI * 2 + revSpot.UpperLimit;
			}
			else
				return (false);
			if (Input.justPressed(MouseKeys.LEFT))
				_step++;
			return (true);
		}

		public bool run() { return false; }

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.CHANGE_LIMIT; }
	}
}
