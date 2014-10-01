using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class MirrorAxis
	{
		static public bool Mirroring = false;
		static public Vector2 Origin { get; internal set; }
		static private Vector2 _Dir = new Vector2(1, 0);
		static public Vector2 Dir {
			get { return _Dir; }
			internal set
			{
				if (value.LengthSquared() != 0)
					_Dir = Vector2.Normalize(value);
			}
		}

		static public Vector2 AxedPosition(Vector2 p)
		{
			return Vector2.Reflect(p - Origin, new Vector2(_Dir.Y, -_Dir.X)) + Origin;
		}
	}

	class ActionSetMirrorAxis : IAction
	{
		private bool EditingDirection;

		public void init()
		{
			EditingDirection = Input.CtrlShift(false, false);
		}

		public bool shortcut()
		{
			return (Input.CtrlShift(false, false)
				|| Input.CtrlShift(false, true))
				&& Input.justPressed(Keys.F);
		}

		public bool run()
		{
			Vector2 pos = Input.VirtualSimMousePos - (EditingDirection ? MirrorAxis.Origin : Vector2.Zero);
			if (Input.pressed(MouseKeys.RIGHT))
				pos.X = 0;
			if (Input.pressed(MouseKeys.LEFT))
				pos.Y = 0;
			if (EditingDirection)
				MirrorAxis.Dir = pos;
			else
				MirrorAxis.Origin = pos;
			return Input.pressed(Keys.F);
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.SET_MIRROR; }
	}
}