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
		private Piece P1;
		private Piece P2;
		private float From;
		private float To;
		private bool HasBeenRevert;
		private bool IsOk;
		private int _step;

		public void init()
		{
			_step = 0;
			P1 = RobotEditor.Instance.Select1;
			P2 = RobotEditor.Instance.Select2;
			HasBeenRevert = false;

			IsOk = P1.isConnected(P2);
			if (IsOk)
			{
				From = P1.getConnection(P2).MaxAngle;
				To = From;
			}
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, true) && (Input.justPressed(Keys.Q)));
		}

		public bool run()
		{
			if (!IsOk)
				return false;
			RevoluteSpot revSpot = (RevoluteSpot)P1.getConnection(P2);;
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

		public void revert() { }

		public bool canBeReverted() { return false; }

		public ActionTypes type() { return ActionTypes.CHANGE_LIMIT; }
	}
}
