using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;

namespace gearit.src.editor.robot.action
{
	class ActionChangeRodLimit : IAction
	{
		private float From;
		private float To;
		private bool HasBeenRevert;
		private bool IsOk;
		private int _step;
		private float _angle;
		private bool First;
		private Rod ClonePiece;
		private RevoluteSpot RevSpot;
		private int NbCycles;
		private float PrevAngle;
		private float OriginAngle;

		static public Rod P2 { get; private set; }
		private float AngleOrigin;
		private float AngleMax;
		private float AngleMin;

		public void init()
		{
			Piece p1 = RobotEditor.Instance.Select1;
			Piece p2 = RobotEditor.Instance.Select2;
			IsOk = p1.isConnected(p2) && p2.GetType() == typeof(Rod);
			if (!IsOk)
				return;
			_step = 0;
			HasBeenRevert = false;
			RevSpot = p1.getConnection(p2);

			From = RevSpot.MaxAngle;
			To = From;
			RevSpot.MaxAngle = 0;
			RevSpot.MinAngle = 0;
			First = true;
			ActionLaunch.ResetWorld();
			ClonePiece = new Rod(RobotEditor.Instance.Robot, ((Rod)p2).Size);
			P2 = (Rod) p2;
			new RevoluteSpot(RobotEditor.Instance.Robot, p1, ClonePiece,
				p1.getConnection(p2).LocalAnchorA,
				p1.getConnection(p2).LocalAnchorB);
			ClonePiece.IsStatic = false;
			NbCycles = 0;
			PrevAngle = 0;
			OriginAngle = (float) MathUtils.VectorAngle(Vector2.Zero, P2.GetEnd(!P2.CloseEnd(RevSpot.WorldAnchorA)));
		}

		public bool shortcut()
		{
			return false;
			return Input.CtrlShift(false, false)
				&& Input.justPressed(Keys.Q)
				&& RobotEditor.Instance.Select2.GetType() == typeof(Rod);
		}

		private float Angle
		{
			get
			{
				float angle = OriginAngle + _angle + ((float)Math.PI) * 2 * NbCycles;
				if (Input.Shift)
				{
					float factor = 2f * (float) Math.PI / 16f;
					int res = (int) (angle / factor - 0.5f);
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

			PrevAngle = _angle;
			_angle = (float) MathUtils.VectorAngle(P2.GetEnd(!P2.CloseEnd(RevSpot.WorldAnchorA)), currentPos);
			if (_angle - PrevAngle > Math.PI)
				NbCycles--;
			if (_angle - PrevAngle < -Math.PI)
				NbCycles++;

			ClonePiece.Rotation = Angle;
			if (_step == 0)
			{
				if (Angle > 0)
					RevSpot.MaxAngle = Angle;
				else
				{
					RevSpot.MaxAngle = 0;
					if (NbCycles < -1)
						NbCycles = -1;
				}
			}
			else if (_step == 1)
			{
				if (Angle < 0)
					RevSpot.MinAngle = Angle;
				else
				{
					RevSpot.MinAngle = 0;
					if (NbCycles > 1)
						NbCycles = 1;
				}
			}

			if (Input.justPressed(MouseKeys.LEFT))
				_step++;
			if (_step >= 2)
			{
				ClonePiece.DeepDestroy();
				return false;
			}
			return true;
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.CHANGE_ROD_LIMIT; }
	}
}