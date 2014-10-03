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
	class ActionChangeValues : IAction
	{
		private Piece Piece;
		private RevoluteSpot Spot;
		private bool IsPiece;
		private float Backup;

		public void init()
		{
			IsPiece = Input.CtrlShift(false, false);
			if (IsPiece)
				Piece = RobotEditor.Instance.Select1;
			else
				Spot = RobotEditor.Instance.Select1.getConnection(RobotEditor.Instance.Select2);

			if (IsPiece)
				Backup = Piece.Weight;
			else
				Backup = Spot.MaxForce;
		}

		public bool shortcut()
		{
			return Input.justPressed(Keys.V)
				&& (Input.CtrlShift(false, false)
				|| (RobotEditor.Instance.Select1.isConnected(RobotEditor.Instance.Select2)
				&& Input.CtrlShift(false, true)));
		}

		public Vector2 Origin
		{
			get
			{
				if (IsPiece)
					return Piece.Position;
				else
					return Spot.WorldAnchorA;
			}
		}

		public int Unit
		{
			get
			{
				float angle = (float)MathUtils.VectorAngle(Input.VirtualSimMousePos - Origin, new Vector2(0, MirrorAxis.Active ? 1 : -1));
				angle *= -1;
				angle -= (float) Math.PI / 2;
				while (angle < 0)
					angle += (float)(2f * Math.PI);
				return (int)((angle / (Math.PI * 2f)) * 10);
			}
		}
		
		public int Power
		{
			get
			{
				int v = (int) ((Vector2.Distance(Input.VirtualSimMousePos, Origin) - 0.5f) * 3f);
				if (v < 0)
					v = 0;
				return v;
			}
		}


		public bool run()
		{
			float v = Unit;
			int power = Power;
			for (int i = 0; i < power; i++)
				v *= 10f;
			if (IsPiece)
				Piece.Weight = v;
			else
				Spot.MaxForce = v;
			if (Input.justPressed(MouseKeys.RIGHT))
			{
				if (IsPiece)
					Piece.Weight = Backup;
				else
					Spot.MaxForce = Backup;
				return false;
			}

			return (Input.justPressed(MouseKeys.LEFT) == false
					&& Input.justReleased(Keys.V) == false);
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.CHANGE_VALUES; }
	}
}
