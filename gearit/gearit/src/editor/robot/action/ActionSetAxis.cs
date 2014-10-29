using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using gearit.src.utility;
using Microsoft.Xna.Framework.Input;

namespace gearit.src.editor.robot.action
{
	class ActionSetAxis : IAction
	{
		private bool EditingDirection;
		private bool Mirror;

		public void init()
		{
			Mirror = Input.justPressed(Keys.F);
			EditingDirection = Input.CtrlShift(false, true) || !Mirror;
		}

		public bool shortcut()
		{
			return ((Input.CtrlShift(false, false)
				|| Input.CtrlShift(false, true))
				&& Input.justPressed(Keys.F))
				|| (Input.CtrlShift(false, true)
				&& Input.justPressed(Keys.Z));
		}

		public bool run()
		{
			Vector2 pos = Input.VirtualSimMousePos;
			if (Mirror)
				pos -= EditingDirection ? MirrorAxis.Origin : Vector2.Zero;
			else
				pos -= LockAxis.Origin;
			if (Input.pressed(MouseKeys.RIGHT))
				pos.X = 0;
			if (Input.pressed(MouseKeys.LEFT))
				pos.Y = 0;
			if (!Mirror)
				LockAxis.Dir = pos;
			else if (EditingDirection)
				MirrorAxis.Dir = pos;
			else
				MirrorAxis.Origin = pos;
			return Input.pressed(Keys.F) || Input.pressed(Keys.X);
		}

		public void revert() { }

		public bool canBeReverted { get { return false; } }
		public bool canBeMirrored { get { return false; } }
		public ActionTypes Type() { return ActionTypes.SET_AXIS; }
	}
}