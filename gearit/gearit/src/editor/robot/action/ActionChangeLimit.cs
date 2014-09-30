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
		private float From;
		private float To;
		private bool HasBeenRevert;
		private bool IsOk;
		private int _step;
		private Vector2 PreviousPos;
		private Vector2 Origin;
		private RevoluteSpot RevSpot;

		public void init()
		{
			Piece p1 = RobotEditor.Instance.Select1;
			Piece p2 = RobotEditor.Instance.Select2;
			IsOk = p1.isConnected(p2);
			if (!IsOk)
				return;
			_step = 0;
			HasBeenRevert = false;
			RevSpot = (RevoluteSpot)p1.getConnection(p2);

			From = RevSpot.MaxAngle;
			To = From;
			RevSpot.MaxAngle = 0;
			RevSpot.MinAngle = 0;
			Origin = RevSpot.WorldAnchorA;
			PreviousPos = Input.SimMousePos - Origin;
		}

		public bool shortcut()
		{
			return (Input.ctrlAltShift(false, false, false) && (Input.justPressed(Keys.Q)));
		}

		public bool run()
		{
			if (!IsOk)
				return false;
			Vector2 currentPos = Input.SimMousePos - RevSpot.WorldAnchorA;
			float deltaAngle = (float) MathUtils.VectorAngle(PreviousPos, currentPos);
			PreviousPos = currentPos;
//(float) MathUtils.VectorAngle(currentPos, PreviousPos);

			if (_step == 0)
			{
				RevSpot.VirtualLimitBegin = -(float)MathUtils.VectorAngle(Input.SimMousePos - RevSpot.WorldAnchorA, new Vector2(1, 0));
			}
			else if (_step == 1)
			{
				RevSpot.MinAngle += deltaAngle;
				if (RevSpot.MinAngle > 0)
					RevSpot.MinAngle = 0;
				//if (RevSpot.UpperLimit < RevSpot.LowerLimit)
				//	RevSpot.UpperLimit = RevSpot.LowerLimit;
			}
			else if (_step == 2)
			{
				//RevSpot.UpperLimit = -(float)MathUtils.VectorAngle(Input.SimMousePos - RevSpot.WorldAnchorA, new Vector2(1, 0)) - RevSpot.VirtualLimitBegin;
				RevSpot.MaxAngle += deltaAngle;
				if (RevSpot.MaxAngle < 0)
					RevSpot.MaxAngle = 0;
				//if (RevSpot.UpperLimit - RevSpot.LowerLimit > Math.PI * 2)
				//	RevSpot.LowerLimit = -(float)Math.PI * 2 + RevSpot.UpperLimit;
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
