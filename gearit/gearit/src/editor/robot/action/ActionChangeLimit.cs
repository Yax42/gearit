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
		private float angle;
		private bool First;

		public void init()
		{
			Piece p1 = RobotEditor.Instance.Select1;
			Piece p2 = RobotEditor.Instance.Select2;
			IsOk = p1.isConnected(p2);
			if (!IsOk)
				return;
			_step = 0;
			HasBeenRevert = false;
			RevSpot = p1.getConnection(p2);

			From = RevSpot.MaxAngle;
			To = From;
			RevSpot.MaxAngle = 0;
			RevSpot.MinAngle = 0;
			Origin = RevSpot.WorldAnchorA;
			PreviousPos = Input.VirtualSimMousePos - Origin;
			First = true;
		}

		public bool shortcut()
		{
			return (Input.CtrlShift(true, true) && (Input.justPressed(Keys.Q))) || 
					(Input.CtrlShift(false, false) && (Input.justPressed(Keys.Q)));
		}

		private float Angle
		{
			get
			{
				if (Input.Shift)
				{
					float factor = 2f * (float) Math.PI / 16f;
					int res = (int) (angle / factor);
					return res * factor;
				}
				else
					return angle;

			}
		}


		public bool run()
		{
			if (!IsOk)
				return false;
			Vector2 currentPos = Input.VirtualSimMousePos - RevSpot.WorldAnchorA;
			float deltaAngle = (float) MathUtils.VectorAngle(PreviousPos, currentPos);
			PreviousPos = currentPos;
			//(float) MathUtils.VectorAngle(currentPos, PreviousPos);
			angle += deltaAngle;

			if (_step == 0)
			{
				RevSpot.VirtualLimitBegin = -(float)MathUtils.VectorAngle(Input.VirtualSimMousePos - RevSpot.WorldAnchorA, new Vector2(1, 0));
			}
			else if (_step == (MirrorAxis.Active ? 2 : 1))
			{
				if (First)
				{
					angle = RevSpot.MinAngle;
					First = false;
				}
				if (angle > 0)
					angle = 0;
				RevSpot.MinAngle = Angle;
			}
			else if (_step == (MirrorAxis.Active ? 1 : 2))
			{
				if (First)
				{
					angle = RevSpot.MaxAngle;
					First = false;
				}
				//RevSpot.UpperLimit = -(float)MathUtils.VectorAngle(Input.SimMousePos - RevSpot.WorldAnchorA, new Vector2(1, 0)) - RevSpot.VirtualLimitBegin;
				if (angle < 0)
					angle = 0;
				RevSpot.MaxAngle = Angle;
			}
			else
				return (false);
			if (Input.justPressed(MouseKeys.LEFT))
			{
				_step++;
				First = true;
			}
			return (true);
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.CHANGE_LIMIT; }
	}
}
