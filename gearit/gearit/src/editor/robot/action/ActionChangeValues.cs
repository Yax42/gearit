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
			Unit = 1;
			Power = 1;
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
			get;
			private set;
		}

		public int Power
		{
			get;
			private set;
		}


		public bool run()
		{
			if (Input.justPressed(MouseKeys.LEFT))
				Unit++;
			if (Unit > 9)
				Unit = 0;
			if (Input.justPressed(MouseKeys.RIGHT))
				Power++;
			if (Power > 5)
				Power = 1;

			float v = Unit;
			int power = Power;
			for (int i = 1; i < power; i++)
				v *= 10f;
			if (IsPiece)
				Piece.Weight = v;
			else
				Spot.MaxForce = v;
			if (false)
			if (Input.justPressed(MouseKeys.RIGHT))
			{
				if (IsPiece)
					Piece.Weight = Backup;
				else
					Spot.MaxForce = Backup;
				return false;
			}

			return (Input.justReleased(Keys.V) == false);
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return true; } }
		public ActionTypes Type() { return ActionTypes.CHANGE_VALUES; }
	}
}
